#!/usr/bin/env node
/**
 * GameFinder CLI - Find games installed on your system
 */

import { findAllGames } from './game-finder';
import type { Game } from './common/index';

function formatSize(bytes: bigint): string {
  const units = ['B', 'KB', 'MB', 'GB', 'TB'];
  let size = Number(bytes);
  let unitIndex = 0;

  while (size >= 1024 && unitIndex < units.length - 1) {
    size /= 1024;
    unitIndex++;
  }

  return `${size.toFixed(1)} ${units[unitIndex]}`;
}

function printGame(game: Game): void {
  console.log(`   📦 ${game.name}`);
  console.log(`      ID: ${game.id}`);
  console.log(`      Path: ${game.path}`);

  // Show extra info for Steam games
  if (game.store === 'steam' && 'appManifest' in game) {
    const steamGame = game as Game & { appManifest: { sizeOnDisk: bigint } };
    if (steamGame.appManifest.sizeOnDisk > 0n) {
      console.log(`      Size: ${formatSize(steamGame.appManifest.sizeOnDisk)}`);
    }
  }

  console.log('');
}

async function main(): Promise<void> {
  console.log('🎮 GameFinder - Scanning for installed games...\n');

  const { games, errors, skipped } = await findAllGames();

  // Group games by store
  const gamesByStore = new Map<string, Game[]>();
  for (const game of games) {
    const storeGames = gamesByStore.get(game.store) ?? [];
    storeGames.push(game);
    gamesByStore.set(game.store, storeGames);
  }

  // Print results by store
  const storeNames: Record<string, string> = {
    steam: 'Steam',
    gog: 'GOG',
    epic: 'Epic Games',
    xbox: 'Xbox',
  };

  for (const [store, storeGames] of gamesByStore) {
    console.log(`🔍 ${storeNames[store] ?? store}: Found ${storeGames.length} game(s)\n`);
    for (const game of storeGames) {
      printGame(game);
    }
  }

  // Print errors
  for (const [store, error] of errors) {
    console.log(`⚠️  ${storeNames[store] ?? store}: ${error.message}`);
  }

  // Print skipped stores
  for (const store of skipped) {
    console.log(`❌ ${storeNames[store] ?? store}: Not available on this system`);
  }

  console.log('━'.repeat(50));
  console.log(`\n✅ Total games found: ${games.length}`);
}

main().catch((error: unknown) => {
  console.error('Fatal error:', error);
  process.exit(1);
});
