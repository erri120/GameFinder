import { Result, ok } from 'neverthrow';
import type { Game, StoreHandler, GameFinderError } from '../../common/index.js';

/**
 * Handler for finding games managed by Heroic Games Launcher
 */
export class HeroicHandler implements StoreHandler {
  readonly storeName = 'Heroic';

  async findAllGames(): Promise<Result<Game[], GameFinderError>> {
    // TODO: Implement Heroic game discovery
    return ok([]);
  }

  async isAvailable(): Promise<boolean> {
    // TODO: Check if Heroic is installed
    return false;
  }
}
