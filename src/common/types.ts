/**
 * Common types used throughout GameFinder
 */

/**
 * Supported game stores/platforms
 */
export type GameStore = 'steam' | 'gog' | 'epic' | 'origin' | 'xbox' | 'heroic';

/**
 * Result type for operations that can fail
 * Uses neverthrow for type-safe error handling
 */
export { Result, Ok, Err, ok, err } from 'neverthrow';
