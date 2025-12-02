/**
 * GameFinder - A library for finding games installed on your system
 */

// Main entry point
export { findAllGames } from './game-finder';
export type { FindAllGamesOptions, FindAllGamesResult } from './game-finder';

// Common types and utilities
export * from './common/index';

// Store handlers (for fine-grained control)
export { SteamHandler } from './store-handlers/steam/index';
export { GOGHandler } from './store-handlers/gog/index';
export { EpicHandler } from './store-handlers/epic/index';
export { XboxHandler } from './store-handlers/xbox/index';
