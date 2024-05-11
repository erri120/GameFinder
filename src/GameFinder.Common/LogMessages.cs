using System;
using Microsoft.Extensions.Logging;

namespace GameFinder.Common;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0, EventName = nameof(FindingAllGames),
        Level = LogLevel.Trace,
        Message = "Finding all installed games using `{numHandlers}` Handler(s)"
    )]
    public static partial void FindingAllGames(
        ILogger logger,
        int numHandlers
    );

    [LoggerMessage(
        EventId = 1, EventName = nameof(UsingHandler),
        Level = LogLevel.Trace,
        Message = "Using Handler `{handlerType}`"
    )]
    public static partial void UsingHandler(
        ILogger logger,
        Type handlerType
    );

    [LoggerMessage(
        EventId = 2, EventName = nameof(HandlerFoundGames),
        Level = LogLevel.Information,
        Message = "Handler `{handlerType}` found `{numFoundGames}` game(s)"
    )]
    public static partial void HandlerFoundGames(
        ILogger logger,
        Type handlerType,
        int numFoundGames
    );

    [LoggerMessage(
        EventId = 3, EventName = nameof(ExceptionWhileUsingHandler),
        Level = LogLevel.Warning,
        Message = "Handler `{handlerType}` threw an exception while searching for games"
    )]
    public static partial void ExceptionWhileUsingHandler(
        ILogger logger,
        Exception e,
        Type handlerType
    );

    [LoggerMessage(
        EventId = 4, EventName = nameof(FoundGames),
        Level = LogLevel.Information,
        Message = "Found a total of `{numGames}` game(s)"
    )]
    public static partial void FoundGames(
        ILogger logger,
        int numGames
    );

    [LoggerMessage(
        EventId = 5, EventName = nameof(FindingGameWithId),
        Level = LogLevel.Trace,
        Message = "Finding a game with ID `{gameId}` using `{numHandlers}` Handler(s)"
    )]
    public static partial void FindingGameWithId(
        ILogger logger,
        IId gameId,
        int numHandlers
    );

    [LoggerMessage(
        EventId = 6, EventName = nameof(FoundGameWithId),
        Level = LogLevel.Information,
        Message = "Found a game with ID `{gameId}`: `{game}`"
    )]
    public static partial void FoundGameWithId(
        ILogger logger,
        IId gameId,
        IGame game
    );

    [LoggerMessage(
        EventId = 7, EventName = nameof(UnableToFindGameWithId),
        Level = LogLevel.Warning,
        Message = "Didn't find a game with ID `{gameId}`"
    )]
    public static partial void UnableToFindGameWithId(
        ILogger logger,
        IId gameId
    );

    [LoggerMessage(
        EventId = 8, EventName = nameof(FindingGameWithIds),
        Level = LogLevel.Trace,
        Message = "Finding a game with any ID matching `{ids}` using `{numHandlers}` Handler(s)"
    )]
    public static partial void FindingGameWithIds(
        ILogger logger,
        IId[] ids,
        int numHandlers
    );

    [LoggerMessage(
        EventId = 9, EventName = nameof(UnableToFindGameWithIds),
        Level = LogLevel.Warning,
        Message = "Didn't find a game with IDs `{ids}`"
    )]
    public static partial void UnableToFindGameWithIds(
        ILogger logger,
        IId[] ids
    );
}
