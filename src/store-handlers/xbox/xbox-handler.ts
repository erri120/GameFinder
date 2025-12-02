import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games installed via Xbox Game Pass
 */
export class XboxHandler implements StoreHandler {
  readonly storeName = 'Xbox Game Pass';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement Xbox Game Pass game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if Xbox Game Pass is available
    return false;
  }
}
