using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace GameFinder.Common;

/// <summary>
/// Builder for creating instances of <see cref="IGameFinder"/>.
/// </summary>
[PublicAPI]
public static class GameFinderBuilder
{
    /// <summary>
    /// Creates an instance of <see cref="IGameFinder"/>.
    /// </summary>
    public static IGameFinder Create(
        ILoggerFactory loggerFactory,
        params IHandler[] handlers)
    {
        return new GameFinder(loggerFactory.CreateLogger<GameFinder>(), handlers);
    }
}

internal class GameFinder : IGameFinder
{
    private readonly ILogger _logger;
    private readonly IHandler[] _handlers;

    public GameFinder(ILogger<GameFinder> logger, IHandler[] handlers)
    {
        _logger = logger;
        _handlers = handlers;
    }

    public IReadOnlyList<IGame> FindAllGames()
    {
        var games = new List<IGame>();

        foreach (var handler in _handlers)
        {
            try
            {
                games.AddRange(handler.Search());
            }
            catch (Exception e)
            {
                // TODO: logging
            }
        }

        return games;
    }

    public IReadOnlyList<TGame> FindAllGames<TGame>() where TGame : IGame
    {
        var games = new List<TGame>();

        foreach (var handler in _handlers)
        {
            if (handler is not IHandler<TGame> gameHandler) continue;

            try
            {
                games.AddRange(gameHandler.Search());
            }
            catch (Exception e)
            {
                // TODO: logging
            }
        }

        return games;
    }

    public bool TryFindGameWithId(IId id, [NotNullWhen(true)] out IGame? game)
    {
        throw new NotImplementedException();
    }

    public bool TryFindGameWithId<TId, TGame>(TId id, [NotNullWhen(true)] out TGame? game) where TId : IId<TId> where TGame : IGame<TId>
    {
        foreach (var handler in _handlers)
        {
            if (handler is not IHandler<TGame> gameHandler) continue;

            try
            {
                foreach (var foundGame in gameHandler.Search())
                {
                    if (!foundGame.Id.Equals(id)) continue;
                    game = foundGame;
                    return true;
                }
            }
            catch (Exception e)
            {
                // TODO: logging
            }
        }

        game = default;
        return false;
    }

    public bool TryFindGameWithManyIds(IId[] ids, [NotNullWhen(true)] out IGame? game)
    {
        foreach (var handler in _handlers)
        {
            try
            {
                foreach (var foundGame in handler.Search())
                {
                    if (Array.IndexOf(ids, foundGame.Id) == -1) continue;
                    if (!ids.Contains(foundGame.Id)) continue;
                    game = foundGame;
                    return true;
                }
            }
            catch (Exception e)
            {
                // TODO: logging
            }
        }

        game = default;
        return false;
    }

    public bool TryFindGameWithManyIds<TId, TGame>(TId[] ids, [NotNullWhen(true)] out TGame? game)
    {
        throw new NotImplementedException();
    }
}
