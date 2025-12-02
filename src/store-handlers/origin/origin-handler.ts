import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games installed via Origin
 */
export class OriginHandler implements StoreHandler {
  readonly storeName = 'Origin';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement Origin game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if Origin is installed
    return false;
  }
}
