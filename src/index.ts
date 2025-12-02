/**
 * GameFinder - A library for finding games installed on your system
 */

// Main entry point
export { findAllGames } from './game-finder.js';
export type { FindAllGamesOptions, FindAllGamesResult } from './game-finder.js';

// Common types and utilities
export * from './common/index.js';

// Store handlers (for fine-grained control)
export { SteamHandler } from './store-handlers/steam/index.js';
export { GOGHandler } from './store-handlers/gog/index.js';
export { EpicHandler } from './store-handlers/epic/index.js';
export { XboxHandler } from './store-handlers/xbox/index.js';
