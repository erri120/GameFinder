/**
 * Heroic Games Launcher-specific types and data structures
 */

import type { Game } from '../../common/index.js';

/**
 * Platform a game is installed for
 */
export type HeroicPlatform = 'windows' | 'linux';

/**
 * Raw installed game entry from Heroic JSON
 */
export interface HeroicInstalledGame {
  platform: HeroicPlatform;
  executable?: string;
  install_path: string;
  install_size?: string;
  is_dlc: boolean;
  version?: string | null;
  appName: string;
  installedDLCs?: string[];
  language?: string;
  buildId?: string;
}

/**
 * Structure of installed.json file
 */
export interface HeroicInstalledJson {
  installed: HeroicInstalledGame[];
}

/**
 * Represents a game managed by Heroic Games Launcher
 */
export interface HeroicGame extends Game {
  store: 'heroic';

  /**
   * App name / ID from the store
   */
  appName: string;

  /**
   * Platform the game is installed for
   */
  platform: HeroicPlatform;

  /**
   * Whether this is DLC
   */
  isDlc: boolean;

  /**
   * List of installed DLC app names
   */
  installedDlcs: string[];

  /**
   * Build ID if available
   */
  buildId?: string | undefined;
}

/**
 * Creates a HeroicGame from its components
 */
export function createHeroicGame(
  appName: string,
  name: string,
  installPath: string,
  platform: HeroicPlatform,
  isDlc: boolean,
  installedDlcs: string[],
  buildId?: string
): HeroicGame {
  return {
    id: appName,
    name,
    path: installPath,
    store: 'heroic',
    appName,
    platform,
    isDlc,
    installedDlcs,
    buildId,
  };
}
