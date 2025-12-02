import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games installed via Epic Games Store
 */
export class EpicHandler implements StoreHandler {
  readonly storeName = 'Epic Games Store';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement Epic Games Store game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if Epic Games Store is installed
    return false;
  }
}
