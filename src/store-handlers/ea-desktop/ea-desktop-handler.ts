import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games installed via EA Desktop
 */
export class EADesktopHandler implements StoreHandler {
  readonly storeName = 'EA Desktop';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement EA Desktop game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if EA Desktop is installed
    return false;
  }
}
