/**
 * GameFinder - A library for finding games installed on your system
 */

// Common types and utilities
export * from './common/index.js';

// Store handlers
export { SteamHandler } from './store-handlers/steam/index.js';
export { GOGHandler } from './store-handlers/gog/index.js';
export { EpicHandler } from './store-handlers/epic/index.js';
export { EADesktopHandler } from './store-handlers/ea-desktop/index.js';
export { XboxHandler } from './store-handlers/xbox/index.js';
