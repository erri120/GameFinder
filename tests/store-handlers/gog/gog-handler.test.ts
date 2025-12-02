import { platform } from 'node:os';
import { GOGHandler } from '../../../src/store-handlers/gog/gog-handler';
import { createGOGGame } from '../../../src/store-handlers/gog/types';

describe('GOGHandler', () => {
  let handler: GOGHandler;

  beforeEach(() => {
    handler = new GOGHandler();
  });

  it('should have the correct store name', () => {
    expect(handler.storeName).toBe('GOG');
  });

  it('should implement findAllGames', async () => {
    const result = await handler.findAllGames();

    // Result should be Ok (even if no games found on non-Windows)
    expect(result.isOk()).toBe(true);

    if (result.isOk()) {
      // On non-Windows, should return empty array
      if (platform() !== 'win32') {
        expect(result.value).toEqual([]);
      } else {
        // On Windows, all games should have required properties
        for (const game of result.value) {
          expect(game.id).toBeDefined();
          expect(game.name).toBeDefined();
          expect(game.path).toBeDefined();
          expect(game.store).toBe('gog');
          expect(game.gogId).toBeDefined();
          expect(game.buildId).toBeDefined();
          expect(typeof game.isDlc).toBe('boolean');
        }
      }
    }
  });

  it('should implement isAvailable', async () => {
    const available = await handler.isAvailable();
    expect(typeof available).toBe('boolean');

    // On non-Windows, should return false
    if (platform() !== 'win32') {
      expect(available).toBe(false);
    }
  });
});

describe('createGOGGame', () => {
  it('should create a base game correctly', () => {
    const game = createGOGGame(1234567890n, 'Test Game', '/path/to/game', 98765n, undefined);

    expect(game.id).toBe('1234567890');
    expect(game.name).toBe('Test Game');
    expect(game.path).toBe('/path/to/game');
    expect(game.store).toBe('gog');
    expect(game.gogId).toBe(1234567890n);
    expect(game.buildId).toBe(98765n);
    expect(game.parentGameId).toBeUndefined();
    expect(game.isDlc).toBe(false);
  });

  it('should create a DLC game correctly', () => {
    const game = createGOGGame(1111111111n, 'Test DLC', '/path/to/game', 55555n, 1234567890n);

    expect(game.id).toBe('1111111111');
    expect(game.name).toBe('Test DLC');
    expect(game.parentGameId).toBe(1234567890n);
    expect(game.isDlc).toBe(true);
  });
});
