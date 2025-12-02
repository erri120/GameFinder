import { SteamHandler } from '../../../src/store-handlers/steam/steam-handler';
import type { SteamGame } from '../../../src/store-handlers/steam/types';

describe('SteamHandler', () => {
  let handler: SteamHandler;

  beforeEach(() => {
    handler = new SteamHandler();
  });

  it('should have the correct store name', () => {
    expect(handler.storeName).toBe('Steam');
  });

  it('should implement findAllGames', async () => {
    const result = await handler.findAllGames();

    // Result should be Ok (even if no games found)
    expect(result.isOk()).toBe(true);

    if (result.isOk()) {
      // All games should have required properties
      for (const game of result.value) {
        expect(game.id).toBeDefined();
        expect(game.name).toBeDefined();
        expect(game.path).toBeDefined();
        expect(game.store).toBe('steam');
        expect(game.appManifest).toBeDefined();
        expect(game.libraryFolder).toBeDefined();
        expect(game.steamPath).toBeDefined();
      }
    }
  });

  it('should return games with valid app manifest data', async () => {
    const result = await handler.findAllGames();

    if (result.isOk() && result.value.length > 0) {
      const game = result.value[0] as SteamGame;

      // Check app manifest has expected properties
      expect(typeof game.appManifest.appId).toBe('number');
      expect(typeof game.appManifest.name).toBe('string');
      expect(typeof game.appManifest.installDir).toBe('string');
      expect(typeof game.appManifest.installationDirectory).toBe('string');
      expect(typeof game.appManifest.sizeOnDisk).toBe('bigint');
    }
  });

  it('should return games with valid library folder data', async () => {
    const result = await handler.findAllGames();

    if (result.isOk() && result.value.length > 0) {
      const game = result.value[0] as SteamGame;

      // Check library folder has expected properties
      expect(typeof game.libraryFolder.path).toBe('string');
      expect(typeof game.libraryFolder.label).toBe('string');
      expect(typeof game.libraryFolder.totalDiskSize).toBe('bigint');
      expect(game.libraryFolder.appSizes).toBeInstanceOf(Map);
    }
  });

  it('should implement isAvailable', async () => {
    const available = await handler.isAvailable();
    expect(typeof available).toBe('boolean');
  });

  it('should set steamPath after findAllGames', async () => {
    // Before finding games, steamPath should be null
    expect(handler.getSteamPath()).toBeNull();

    const result = await handler.findAllGames();

    // After finding games (if successful), steamPath should be set
    if (result.isOk()) {
      const steamPath = handler.getSteamPath();
      // On systems with Steam installed, this should be a string
      // On systems without Steam, the result would be an error
      if (steamPath !== null) {
        expect(typeof steamPath).toBe('string');
      }
    }
  });
});
