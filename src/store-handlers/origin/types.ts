/**
 * Origin-specific types and data structures
 */

import type { Game } from '../../common/index.js';

/**
 * Represents a game found via Origin
 */
export interface OriginGame extends Game {
  store: 'origin';
}
