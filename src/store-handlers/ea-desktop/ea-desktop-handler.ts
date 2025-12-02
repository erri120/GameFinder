/**
 * Handler for finding games installed via EA Desktop
 */

import { existsSync, readFileSync } from 'node:fs';
import { join } from 'node:path';
import { platform } from 'node:os';
import { Result, ok, err } from 'neverthrow';
import type { StoreHandler, GameFinderError } from '../../common/index.js';
import type { EADesktopGame, EAInstallInfoFile } from './types.js';
import { createEADesktopGame, isInstalled } from './types.js';
import { DATA_DIR_HASH, generateDecryptionKey, decryptISFile } from './crypto.js';
import { collectHardwareInfo } from './hardware-info.js';

/**
 * Get the EA Desktop data directory path
 */
function getEADesktopDataPath(): string {
  const programData = process.env['ProgramData'] ?? 'C:\\ProgramData';
  return join(programData, 'EA Desktop', DATA_DIR_HASH, 'IS');
}

/**
 * Parse the decrypted JSON data
 */
function parseInstallInfoFile(
  jsonData: string
): Result<EAInstallInfoFile, GameFinderError> {
  try {
    const data = JSON.parse(jsonData) as EAInstallInfoFile;

    if (!Array.isArray(data.installInfos)) {
      return err({
        code: 'EA_INVALID_FORMAT',
        message: 'Invalid EA Desktop IS file: missing installInfos array',
      });
    }

    // Warn about schema version mismatch but continue
    if (data.schema?.version !== 21) {
      console.warn(
        `EA Desktop schema version ${data.schema?.version} differs from expected 21`
      );
    }

    return ok(data);
  } catch (error) {
    return err({
      code: 'EA_PARSE_ERROR',
      message: 'Failed to parse EA Desktop IS file',
      cause: error instanceof Error ? error : new Error(String(error)),
    });
  }
}

/**
 * Handler for finding games installed via EA Desktop
 */
export class EADesktopHandler implements StoreHandler {
  readonly storeName = 'EA Desktop';

  /**
   * Find all games installed via EA Desktop
   */
  async findAllGames(): Promise<Result<EADesktopGame[], GameFinderError>> {
    // EA Desktop is Windows-only
    if (platform() !== 'win32') {
      return ok([]);
    }

    const isFilePath = getEADesktopDataPath();

    if (!existsSync(isFilePath)) {
      // EA Desktop not installed or no games
      return ok([]);
    }

    // Collect hardware info for decryption
    const hardwareResult = await collectHardwareInfo();
    if (hardwareResult.isErr()) {
      return err(hardwareResult.error);
    }

    // Read and decrypt the IS file
    let encryptedData: Buffer;
    try {
      encryptedData = readFileSync(isFilePath);
    } catch (error) {
      return err({
        code: 'EA_READ_ERROR',
        message: `Failed to read EA Desktop IS file: ${isFilePath}`,
        cause: error instanceof Error ? error : new Error(String(error)),
      });
    }

    const decryptionKey = generateDecryptionKey(hardwareResult.value);
    const decryptResult = decryptISFile(encryptedData, decryptionKey);
    if (decryptResult.isErr()) {
      return err(decryptResult.error);
    }

    // Parse the JSON
    const parseResult = parseInstallInfoFile(decryptResult.value);
    if (parseResult.isErr()) {
      return err(parseResult.error);
    }

    // Filter to installed games and convert
    const games: EADesktopGame[] = [];
    for (const installInfo of parseResult.value.installInfos) {
      if (!isInstalled(installInfo)) {
        continue;
      }

      // Verify installation path exists
      if (!existsSync(installInfo.baseInstallPath)) {
        continue;
      }

      games.push(createEADesktopGame(installInfo));
    }

    return ok(games);
  }

  /**
   * Check if EA Desktop is available on this system
   */
  async isAvailable(): Promise<boolean> {
    if (platform() !== 'win32') {
      return false;
    }

    return existsSync(getEADesktopDataPath());
  }
}
