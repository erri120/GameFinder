import { createGame } from '../../src/common/game';
import type { Game } from '../../src/common/game';

describe('createGame', () => {
  it('should create a game object with all properties', () => {
    const game = createGame('123', 'Test Game', '/path/to/game', 'steam');

    expect(game).toEqual({
      id: '123',
      name: 'Test Game',
      path: '/path/to/game',
      store: 'steam',
    });
  });

  it('should create games for different stores', () => {
    const steamGame = createGame('1', 'Steam Game', '/steam/path', 'steam');
    const gogGame = createGame('2', 'GOG Game', '/gog/path', 'gog');
    const epicGame = createGame('3', 'Epic Game', '/epic/path', 'epic');

    expect(steamGame.store).toBe('steam');
    expect(gogGame.store).toBe('gog');
    expect(epicGame.store).toBe('epic');
  });
});
