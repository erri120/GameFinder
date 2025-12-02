import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games installed via Steam
 */
export class SteamHandler implements StoreHandler {
  readonly storeName = 'Steam';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement Steam game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if Steam is installed
    return false;
  }
}
