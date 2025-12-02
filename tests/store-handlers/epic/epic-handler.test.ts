import { EpicHandler } from '../../../src/store-handlers/epic/epic-handler';
import { createEGSGame } from '../../../src/store-handlers/epic/types';

describe('EpicHandler', () => {
  let handler: EpicHandler;

  beforeEach(() => {
    handler = new EpicHandler();
  });

  it('should have the correct store name', () => {
    expect(handler.storeName).toBe('Epic Games Store');
  });

  it('should implement findAllGames', async () => {
    const result = await handler.findAllGames();

    // Result should be Ok (even if no games found)
    expect(result.isOk()).toBe(true);

    if (result.isOk()) {
      for (const game of result.value) {
        expect(game.id).toBeDefined();
        expect(game.name).toBeDefined();
        expect(game.path).toBeDefined();
        expect(game.store).toBe('epic');
        expect(game.catalogItemId).toBeDefined();
        expect(Array.isArray(game.manifestHashes)).toBe(true);
      }
    }
  });

  it('should implement isAvailable', async () => {
    const available = await handler.isAvailable();
    expect(typeof available).toBe('boolean');
  });
});

describe('createEGSGame', () => {
  it('should create a game correctly', () => {
    const game = createEGSGame('catalog-123', 'Test Game', '/path/to/game', ['hash1', 'hash2']);

    expect(game.id).toBe('catalog-123');
    expect(game.name).toBe('Test Game');
    expect(game.path).toBe('/path/to/game');
    expect(game.store).toBe('epic');
    expect(game.catalogItemId).toBe('catalog-123');
    expect(game.manifestHashes).toEqual(['hash1', 'hash2']);
  });
});
