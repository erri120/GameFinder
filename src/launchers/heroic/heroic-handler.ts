/**
 * Handler for finding games managed by Heroic Games Launcher
 */

import { existsSync, readFileSync } from 'node:fs';
import { join } from 'node:path';
import { homedir, platform } from 'node:os';
import { Result, ok, err } from 'neverthrow';
import type { StoreHandler, GameFinderError } from '../../common/index.js';
import type { HeroicGame, HeroicInstalledJson } from './types.js';
import { createHeroicGame } from './types.js';

/**
 * Get possible Heroic config directories
 */
function getHeroicConfigPaths(): string[] {
  const home = homedir();

  switch (platform()) {
    case 'linux': {
      const xdgConfig = process.env['XDG_CONFIG_HOME'] ?? join(home, '.config');
      return [
        join(xdgConfig, 'heroic'),
        join(home, '.var', 'app', 'com.heroicgameslauncher.hgl', 'config', 'heroic'),
      ];
    }

    case 'darwin':
      return [join(home, 'Library', 'Application Support', 'heroic')];

    case 'win32':
      return [join(home, 'AppData', 'Roaming', 'heroic')];

    default:
      return [];
  }
}

/**
 * Find the Heroic config directory
 */
function findHeroicConfigPath(): string | undefined {
  const paths = getHeroicConfigPaths();

  for (const configPath of paths) {
    if (existsSync(configPath)) {
      return configPath;
    }
  }

  return undefined;
}

/**
 * Parse the installed.json file for a specific store
 */
function parseInstalledJson(filePath: string): Result<HeroicInstalledJson, GameFinderError> {
  try {
    const content = readFileSync(filePath, 'utf8');
    const data = JSON.parse(content) as HeroicInstalledJson;

    if (!Array.isArray(data.installed)) {
      return err({
        code: 'HEROIC_INVALID_JSON',
        message: `Invalid installed.json format: ${filePath}`,
      });
    }

    return ok(data);
  } catch (error) {
    return err({
      code: 'HEROIC_PARSE_ERROR',
      message: `Failed to parse Heroic installed.json: ${filePath}`,
      cause: error instanceof Error ? error : new Error(String(error)),
    });
  }
}

/**
 * Handler for finding games managed by Heroic Games Launcher
 */
export class HeroicHandler implements StoreHandler {
  readonly storeName = 'Heroic';

  private configPath: string | null = null;

  /**
   * Find all games managed by Heroic Games Launcher
   */
  async findAllGames(): Promise<Result<HeroicGame[], GameFinderError>> {
    const configPath = findHeroicConfigPath();
    if (configPath === undefined) {
      return ok([]);
    }

    this.configPath = configPath;
    const games: HeroicGame[] = [];

    // Check for GOG store installed games
    const gogInstalledPath = join(configPath, 'gog_store', 'installed.json');
    if (existsSync(gogInstalledPath)) {
      const gogResult = parseInstalledJson(gogInstalledPath);
      if (gogResult.isOk()) {
        for (const installed of gogResult.value.installed) {
          // Skip DLCs as separate entries (they're tracked in parent's installedDLCs)
          if (installed.is_dlc) {
            continue;
          }

          // Skip if installation doesn't exist
          if (!existsSync(installed.install_path)) {
            continue;
          }

          games.push(
            createHeroicGame(
              installed.appName,
              installed.appName, // Heroic doesn't always store display name in installed.json
              installed.install_path,
              installed.platform,
              installed.is_dlc,
              installed.installedDLCs ?? [],
              installed.buildId
            )
          );
        }
      }
    }

    // Check for Epic store installed games (legendary backend)
    const legendaryInstalledPath = join(
      configPath,
      'legendaryConfig',
      'legendary',
      'installed.json'
    );
    if (existsSync(legendaryInstalledPath)) {
      const epicResult = parseInstalledJson(legendaryInstalledPath);
      if (epicResult.isOk()) {
        for (const installed of epicResult.value.installed) {
          if (installed.is_dlc) {
            continue;
          }

          if (!existsSync(installed.install_path)) {
            continue;
          }

          games.push(
            createHeroicGame(
              installed.appName,
              installed.appName,
              installed.install_path,
              installed.platform,
              installed.is_dlc,
              installed.installedDLCs ?? [],
              installed.buildId
            )
          );
        }
      }
    }

    return ok(games);
  }

  /**
   * Check if Heroic Games Launcher is available on this system
   */
  async isAvailable(): Promise<boolean> {
    return findHeroicConfigPath() !== undefined;
  }

  /**
   * Get the Heroic config directory (available after findAllGames)
   */
  getConfigPath(): string | null {
    return this.configPath;
  }
}
