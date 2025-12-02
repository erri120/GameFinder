import { SteamHandler } from '../../../src/store-handlers/steam/steam-handler.js';

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
    expect(result.isOk()).toBe(true);
  });

  it('should implement isAvailable', async () => {
    const available = await handler.isAvailable();
    expect(typeof available).toBe('boolean');
  });
});
