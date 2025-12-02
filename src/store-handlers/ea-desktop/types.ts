/**
 * EA Desktop-specific types and data structures
 */

import type { Game } from '../../common/index.js';

/**
 * EA Desktop software ID
 */
export type EASoftwareId = string;

/**
 * Launcher information from EA Desktop
 */
export interface EALauncher {
  exePath: string;
  cmdArgs: string;
  executeElevated: boolean;
  isTimedTrial: boolean;
  requires64BitOs: boolean;
}

/**
 * Version information
 */
export interface EAVersion {
  major: number;
  minor: number;
  build: number;
  rev: number;
}

/**
 * Detailed installation state
 */
export interface EADetailedState {
  installPhase: number;
  installReason: number;
  installStatus: number;
  previousInstallStatus: number;
}

/**
 * Local install properties
 */
export interface EALocalInstallProperties {
  launchers: EALauncher[];
  localManifestVersion: EAVersion;
  useGameVersionFromManifest: boolean;
}

/**
 * Install info from EA Desktop's IS file
 */
export interface EAInstallInfo {
  baseInstallPath: string;
  baseSlug: string;
  softwareId: string;
  installCheck: string;
  contentManifestLaunchers: string;
  executableCheck: string;
  installedLocale: string;
  installedVersion: string;
  dlcSubPath: string;
  detailedState: EADetailedState;
  localInstallProperties: EALocalInstallProperties;
  localUninstallProperties: {
    uninstallCommand: string;
    uninstallParameters: string;
  };
}

/**
 * Root structure of the decrypted IS file
 */
export interface EAInstallInfoFile {
  installInfos: EAInstallInfo[];
  schema: {
    version: number;
  };
}

/**
 * Represents a game found via EA Desktop
 */
export interface EADesktopGame extends Game {
  store: 'ea-desktop';

  /**
   * EA Desktop software ID
   */
  softwareId: EASoftwareId;

  /**
   * Base slug (URL-friendly name)
   */
  baseSlug: string;

  /**
   * Installed version
   */
  version: string;

  /**
   * Available launchers
   */
  launchers: EALauncher[];
}

/**
 * Creates an EADesktopGame from install info
 */
export function createEADesktopGame(installInfo: EAInstallInfo): EADesktopGame {
  return {
    id: installInfo.softwareId,
    name: installInfo.baseSlug,
    path: installInfo.baseInstallPath,
    store: 'ea-desktop',
    softwareId: installInfo.softwareId,
    baseSlug: installInfo.baseSlug,
    version: installInfo.installedVersion,
    launchers: installInfo.localInstallProperties.launchers,
  };
}

/**
 * Check if an install info represents an installed game
 */
export function isInstalled(installInfo: EAInstallInfo): boolean {
  // installStatus 0 = not installed, 3-5 = various installed states
  return (
    installInfo.detailedState.installStatus >= 3 &&
    installInfo.baseInstallPath !== ''
  );
}
