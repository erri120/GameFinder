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

    private IReadOnlyList<IGame>? UseHandler(IHandler handler)
    {
        try
        {
            LogMessages.UsingHandler(_logger, handler.GetType());
            var foundGames = handler.Search();
            LogMessages.HandlerFoundGames(_logger, handler.GetType(), foundGames.Count);

            return foundGames;
        }
        catch (Exception e)
        {
            LogMessages.ExceptionWhileUsingHandler(_logger, e, handler.GetType());
            return null;
        }
    }

    private IReadOnlyList<TGame>? UseHandler<TGame>(IHandler<TGame> handler) where TGame : IGame
    {
        try
        {
            LogMessages.UsingHandler(_logger, handler.GetType());
            var foundGames = handler.Search();
            LogMessages.HandlerFoundGames(_logger, handler.GetType(), foundGames.Count);

            return foundGames;
        }
        catch (Exception e)
        {
            LogMessages.ExceptionWhileUsingHandler(_logger, e, handler.GetType());
            return null;
        }
    }

    public IReadOnlyList<IGame> FindAllGames()
    {
        LogMessages.FindingAllGames(_logger, _handlers.Length);
        var games = new List<IGame>();

        foreach (var handler in _handlers)
        {
            var foundGames = UseHandler(handler);
            if (foundGames is null) continue;
            games.AddRange(foundGames);
        }

        LogMessages.FoundGames(_logger, games.Count);
        return games;
    }

    public IReadOnlyList<TGame> FindAllGames<TGame>() where TGame : IGame
    {
        LogMessages.FindingAllGames(_logger, _handlers.Length);
        var games = new List<TGame>();

        foreach (var handler in _handlers)
        {
            if (handler is not IHandler<TGame> gameHandler) continue;

            var foundGames = UseHandler(gameHandler);
            if (foundGames is null) continue;
            games.AddRange(foundGames);
        }

        LogMessages.FoundGames(_logger, games.Count);
        return games;
    }

    public bool TryFindGameWithId(IId id, [NotNullWhen(true)] out IGame? game)
    {
        LogMessages.FindingGameWithId(_logger, id, _handlers.Length);

        foreach (var handler in _handlers)
        {
            var foundGames = UseHandler(handler);
            if (foundGames is null) continue;

            foreach (var foundGame in foundGames)
            {
                if (!foundGame.Id.Equals(id)) continue;

                LogMessages.FoundGameWithId(_logger, id, foundGame);
                game = foundGame;
                return true;
            }
        }

        LogMessages.UnableToFindGameWithId(_logger, id);
        game = default;
        return false;
    }

    public bool TryFindGameWithId<TId, TGame>(TId id, [NotNullWhen(true)] out TGame? game)
        where TGame : IGame<TId>
        where TId : IId
    {
        LogMessages.FindingGameWithId(_logger, id, _handlers.Length);

        foreach (var handler in _handlers)
        {
            if (handler is not IHandler<TGame> gameHandler) continue;

            var foundGames = UseHandler(gameHandler);
            if (foundGames is null) continue;

            foreach (var foundGame in foundGames)
            {
                if (!foundGame.Id.Equals(id)) continue;

                LogMessages.FoundGameWithId(_logger, id, foundGame);
                game = foundGame;
                return true;
            }
        }

        LogMessages.UnableToFindGameWithId(_logger, id);
        game = default;
        return false;
    }

    public bool TryFindGameWithManyIds(IId[] ids, [NotNullWhen(true)] out IGame? game)
    {
        LogMessages.FindingGameWithIds(_logger, ids, _handlers.Length);

        foreach (var handler in _handlers)
        {
            var foundGames = UseHandler(handler);
            if (foundGames is null) continue;

            foreach (var foundGame in foundGames)
            {
                if (Array.IndexOf(ids, foundGame.Id) == -1) continue;

                LogMessages.FoundGameWithId(_logger, foundGame.Id, foundGame);
                game = foundGame;
                return true;
            }
        }

        LogMessages.UnableToFindGameWithIds(_logger, ids);
        game = default;
        return false;
    }

    public bool TryFindGameWithManyIds<TId, TGame>(TId[] ids, [NotNullWhen(true)] out TGame? game)
        where TGame : IGame<TId>
        where TId : IId
    {
        var logIds = ids.Cast<IId>().ToArray();
        LogMessages.FindingGameWithIds(_logger, logIds, _handlers.Length);

        foreach (var handler in _handlers)
        {
            if (handler is not IHandler<TGame> gameHandler) continue;

            var foundGames = UseHandler(gameHandler);
            if (foundGames is null) continue;

            foreach (var foundGame in foundGames)
            {
                if (Array.IndexOf(ids, foundGame.Id) == -1) continue;

                LogMessages.FoundGameWithId(_logger, foundGame.Id, foundGame);
                game = foundGame;
                return true;
            }
        }

        LogMessages.UnableToFindGameWithIds(_logger, logIds);
        game = default;
        return false;
    }
}
