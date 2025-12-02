#!/usr/bin/env node
/**
 * GameFinder CLI - Find games installed on your system
 */

import { SteamHandler } from './store-handlers/steam/index.js';
import { GOGHandler } from './store-handlers/gog/index.js';
import { EpicHandler } from './store-handlers/epic/index.js';
import { EADesktopHandler } from './store-handlers/ea-desktop/index.js';
import { XboxHandler } from './store-handlers/xbox/index.js';
import type { StoreHandler, Game } from './common/index.js';

interface HandlerInfo {
  name: string;
  handler: StoreHandler;
}

const handlers: HandlerInfo[] = [
  { name: 'Steam', handler: new SteamHandler() },
  { name: 'GOG', handler: new GOGHandler() },
  { name: 'Epic Games', handler: new EpicHandler() },
  { name: 'EA Desktop', handler: new EADesktopHandler() },
  { name: 'Xbox', handler: new XboxHandler() },
];

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

async function main(): Promise<void> {
  console.log('🎮 GameFinder - Scanning for installed games...\n');

  let totalGames = 0;

  for (const { name, handler } of handlers) {
    const available = await handler.isAvailable();

    if (!available) {
      console.log(`❌ ${name}: Not available on this system`);
      continue;
    }

    console.log(`🔍 ${name}: Scanning...`);

    const result = await handler.findAllGames();

    if (result.isErr()) {
      console.log(`   ⚠️  Error: ${result.error.message}`);
      continue;
    }

    const games = result.value;

    if (games.length === 0) {
      console.log(`   No games found`);
      continue;
    }

    console.log(`   Found ${games.length} game(s):\n`);

    for (const game of games) {
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

    totalGames += games.length;
  }

  console.log('━'.repeat(50));
  console.log(`\n✅ Total games found: ${totalGames}`);
}

main().catch((error: unknown) => {
  console.error('Fatal error:', error);
  process.exit(1);
});
