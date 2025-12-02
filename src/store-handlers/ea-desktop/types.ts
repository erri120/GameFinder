/**
 * EA Desktop-specific types and data structures
 */

import type { Game } from '../../common/index.js';

/**
 * EA Desktop content ID
 */
export type EADesktopContentId = string;

/**
 * Represents a game found via EA Desktop
 */
export interface EADesktopGame extends Game {
  store: 'ea-desktop';

  /**
   * EA Desktop content ID
   */
  contentId: EADesktopContentId;

  /**
   * Base game content ID (for DLC)
   */
  baseGameContentId?: string | undefined;
}
