import { SteamHandler } from './store-handlers/steam/index';
import { GOGHandler } from './store-handlers/gog/index';
import { EpicHandler } from './store-handlers/epic/index';
import { XboxHandler } from './store-handlers/xbox/index';
import type { Game, GameFinderError, StoreHandler } from './common/index';
import type { GameStore } from './common/types';

/**
 * Options for findAllGames
 */
export interface FindAllGamesOptions {
  /**
   * Only search these stores. If not provided, searches all stores.
   */
  stores?: GameStore[];

  /**
   * If true, includes results from stores that aren't available on this system.
   * Default: false (skips unavailable stores silently)
   */
  includeUnavailable?: boolean;
}

/**
 * Result from findAllGames
 */
export interface FindAllGamesResult {
  /**
   * All games found across all searched stores
   */
  games: Game[];

  /**
   * Errors that occurred while searching specific stores
   */
  errors: Map<GameStore, GameFinderError>;

  /**
   * Stores that were skipped because they aren't available on this system
   */
  skipped: GameStore[];
}

/**
 * All available store handlers
 */
const ALL_HANDLERS: Record<GameStore, () => StoreHandler> = {
  steam: () => new SteamHandler(),
  gog: () => new GOGHandler(),
  epic: () => new EpicHandler(),
  xbox: () => new XboxHandler(),
};

/**
 * All supported stores
 */
const ALL_STORES: GameStore[] = ['steam', 'gog', 'epic', 'xbox'];

/**
 * Find all games installed on the system across multiple game stores.
 *
 * @example
 * ```typescript
 * // Find all games from all available stores
 * const { games, errors, skipped } = await findAllGames();
 *
 * // Find games only from specific stores
 * const { games } = await findAllGames({ stores: ['steam', 'gog'] });
 * ```
 */
export async function findAllGames(
  options: FindAllGamesOptions = {}
): Promise<FindAllGamesResult> {
  const { stores = ALL_STORES, includeUnavailable = false } = options;

  const games: Game[] = [];
  const errors = new Map<GameStore, GameFinderError>();
  const skipped: GameStore[] = [];

  // Process stores in parallel
  const results = await Promise.all(
    stores.map(async (store) => {
      const handler = ALL_HANDLERS[store]();

      // Check availability first
      const isAvailable = await handler.isAvailable();
      if (!isAvailable) {
        if (includeUnavailable) {
          return {
            store,
            error: {
              code: 'STORE_UNAVAILABLE',
              message: `${handler.storeName} is not available on this system`,
            } as GameFinderError,
          };
        }
        return { store, skipped: true };
      }

      // Find games
      const result = await handler.findAllGames();

      if (result.isErr()) {
        return { store, error: result.error };
      }

      return { store, games: result.value };
    })
  );

  // Collect results
  for (const result of results) {
    if ('skipped' in result && result.skipped) {
      skipped.push(result.store);
    } else if ('error' in result && result.error) {
      errors.set(result.store, result.error);
    } else if ('games' in result && result.games) {
      games.push(...result.games);
    }
  }

  return { games, errors, skipped };
}
