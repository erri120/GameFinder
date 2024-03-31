using System;
using GameFinder.Common;
using Microsoft.Extensions.Logging;

namespace GameFinder.StoreHandlers.GOG;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0, EventName = nameof(UnableToOpenGOGSubKey),
        Level = LogLevel.Information,
        Message = "Unable to open the GOG Registry Key `{baseKeyName}\\\\{gogKeyName}`." +
                  "This is no cause for concern if GOG Galaxy isn't installed or no games have been installed with GOG Galaxy."
    )]
    public static partial void UnableToOpenGOGSubKey(
        ILogger logger,
        string baseKeyName,
        string gogKeyName
    );

    [LoggerMessage(
        EventId = 1, EventName = nameof(FoundSubKeyNames),
        Level = LogLevel.Trace,
        Message = "Found {count} sub key name(s) for key `{key}`: `{names}`"
    )]
    public static partial void FoundSubKeyNames(
        ILogger logger,
        string key,
        int count,
        string[] names
    );

    [LoggerMessage(
        EventId = 2, EventName = nameof(UnableToOpenSubKey),
        Level = LogLevel.Warning,
        Message = "Unable to open the Registry Key `{gogKeyName}\\\\{subKeyName}`"
    )]
    public static partial void UnableToOpenSubKey(
        ILogger logger,
        string gogKeyName,
        string subKeyName
    );

    [LoggerMessage(
        EventId = 3, EventName = nameof(NoValueForKey),
        Level = LogLevel.Warning,
        Message = "Found no value with name `{valueName}` in `{keyName}`"
    )]
    public static partial void NoValueForKey(
        ILogger logger,
        string valueName,
        string keyName
    );

    [LoggerMessage(
        EventId = 4, EventName = nameof(ValueForKey),
        Level = LogLevel.Trace,
        Message = "Found value with name `{valueName}` in `{keyName}`: `{value}`"
    )]
    public static partial void ValueForKey(
        ILogger logger,
        string valueName,
        string keyName,
        string value
    );

    [LoggerMessage(
        EventId = 5, EventName = nameof(FailedToParse),
        Level = LogLevel.Warning,
        Message = "Failed to parse value `{value}` with name `{valueName}` in `{keyName}` as `{typeName}`"
    )]
    public static partial void FailedToParse(
        ILogger logger,
        string keyName,
        string value,
        string valueName,
        string typeName
    );

    [LoggerMessage(
        EventId = 6, EventName = nameof(NoId),
        Level = LogLevel.Warning,
        Message = "Unable to find any IDs in `{keyName}`"
    )]
    public static partial void NoId(
        ILogger logger,
        string keyName
    );

    [LoggerMessage(
        EventId = 7, EventName = nameof(NoGameName),
        Level = LogLevel.Warning,
        Message = "Unable to find any game names in `{keyName}`"
    )]
    public static partial void NoGameName(
        ILogger logger,
        string keyName
    );

    [LoggerMessage(
        EventId = 8, EventName = nameof(NoPath),
        Level = LogLevel.Warning,
        Message = "Unable to find any paths in `{keyName}`"
    )]
    public static partial void NoPath(
        ILogger logger,
        string keyName
    );

    [LoggerMessage(
        EventId = 9, EventName = nameof(ParsedRegistryKey),
        Level = LogLevel.Information,
        Message = "Parsed Registry Key `{keyName}` to `{game}`"
    )]
    public static partial void ParsedRegistryKey(
        ILogger logger,
        string keyName,
        IGame game
    );

    [LoggerMessage(
        EventId = 10, EventName = nameof(ExceptionWhileParsingRegistryKey),
        Level = LogLevel.Warning,
        Message = "Exception while parsing Registry Key `{keyName}`"
    )]
    public static partial void ExceptionWhileParsingRegistryKey(
        ILogger logger,
        Exception e,
        string keyName
    );
}
