/**
 * Handler for finding games installed via Origin
 * Note: Origin has been deprecated in favor of EA Desktop
 */

import { Result, ok } from 'neverthrow';
import type { StoreHandler, GameFinderError } from '../../common/index.js';
import type { OriginGame } from './types.js';

/**
 * Handler for finding games installed via Origin
 * @deprecated Origin has been replaced by EA Desktop
 */
export class OriginHandler implements StoreHandler {
  readonly storeName = 'Origin';

  /**
   * Find all games installed via Origin
   * Note: Origin is deprecated, this handler returns empty results
   */
  async findAllGames(): Promise<Result<OriginGame[], GameFinderError>> {
    // Origin has been replaced by EA Desktop
    // This handler is kept for legacy compatibility but returns empty results
    return ok([]);
  }

  /**
   * Check if Origin is available on this system
   */
  async isAvailable(): Promise<boolean> {
    // Origin is deprecated
    return false;
  }
}
