import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games installed via GOG Galaxy
 */
export class GOGHandler implements StoreHandler {
  readonly storeName = 'GOG';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement GOG game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if GOG Galaxy is installed
    return false;
  }
}
