/**
 * Handler for finding games installed via EA Desktop
 * Note: EA Desktop uses encrypted databases which require special handling
 */

import { Result, ok } from 'neverthrow';
import type { StoreHandler, GameFinderError } from '../../common/index.js';
import type { EADesktopGame } from './types.js';

/**
 * Handler for finding games installed via EA Desktop
 * Note: Full implementation requires decryption of EA's encrypted SQLite databases
 */
export class EADesktopHandler implements StoreHandler {
  readonly storeName = 'EA Desktop';

  /**
   * Find all games installed via EA Desktop
   * Note: This requires decryption of encrypted databases, not yet implemented
   */
  async findAllGames(): Promise<Result<EADesktopGame[], GameFinderError>> {
    // EA Desktop uses encrypted SQLite databases
    // Full implementation would require:
    // 1. Finding the encrypted database at %LOCALAPPDATA%\Electronic Arts\EA Desktop\IS\...
    // 2. Decrypting the database using DPAPI
    // 3. Parsing the SQLite content for installed games
    // This is complex and platform-specific, so returning empty for now
    return ok([]);
  }

  /**
   * Check if EA Desktop is available on this system
   */
  async isAvailable(): Promise<boolean> {
    // Not fully implemented
    return false;
  }
}
