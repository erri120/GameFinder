using System;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0, EventName = nameof(MissingManifestDirectory),
        Level = LogLevel.Information,
        Message = "The directory where Origin stores its Manifest files `{manifestDirectory}` does not exist. " +
                  "This is no cause for concern if Origin isn't installed or no games have been installed with Origin."
    )]
    public static partial void MissingManifestDirectory(
        ILogger logger,
        AbsolutePath manifestDirectory
    );

    [LoggerMessage(
        EventId = 1, EventName = nameof(NoManifestFiles),
        Level = LogLevel.Information,
        Message = "The directory where Origin stores its Manifest files `{manifestDirectory}` does not contain any Manifest files. " +
                  "This is no cause for concern if Origin isn't installed or no games have been installed with Origin."
    )]
    public static partial void NoManifestFiles(
        ILogger logger,
        AbsolutePath manifestDirectory
    );

    [LoggerMessage(
        EventId = 2, EventName = nameof(ParsingManifestFile),
        Level = LogLevel.Debug,
        Message = "Parsing manifest file `{manifestFile}`"
    )]
    public static partial void ParsingManifestFile(
        ILogger logger,
        AbsolutePath manifestFile
    );

    [LoggerMessage(
        EventId = 3, EventName = nameof(NoValuesForKey),
        Level = LogLevel.Warning,
        Message = "Manifest file `{manifestFile}` does not contain any values for key `{key}`"
    )]
    public static partial void NoValuesForKey(
        ILogger logger,
        AbsolutePath manifestFile,
        string key
    );

    [LoggerMessage(
        EventId = 4, EventName = nameof(ValuesForKey),
        Level = LogLevel.Trace,
        Message = "Found {count} value(s) for key `{key}` in Manifest file `{manifestFile}`: `{values}`"
    )]
    public static partial void ValuesForKey(
        ILogger logger,
        AbsolutePath manifestFile,
        string key,
        int count,
        string[] values
    );

    [LoggerMessage(
        EventId = 5, EventName = nameof(SkipSteamGame),
        Level = LogLevel.Information,
        Message = "Skipping Manifest file `{manifestFile}` because it indicates that the game comes from Steam: `{steamId}`"
    )]
    public static partial void SkipSteamGame(
        ILogger logger,
        AbsolutePath manifestFile,
        string steamId
    );

    [LoggerMessage(
        EventId = 6, EventName = nameof(FoundValueForKey),
        Level = LogLevel.Trace,
        Message = "Found value `{value}` for key `{key}` in Manifest file `{manifestFile}`"
    )]
    public static partial void FoundValueForKey(
        ILogger logger,
        AbsolutePath manifestFile,
        string key,
        string value
    );

    [LoggerMessage(
        EventId = 7, EventName = nameof(ExceptionWhileParsingManifest),
        Level = LogLevel.Warning,
        Message = "Exception while parsing Manifest file `{manifestFile}`"
    )]
    public static partial void ExceptionWhileParsingManifest(
        ILogger logger,
        Exception e,
        AbsolutePath manifestFile
    );

    [LoggerMessage(
        EventId = 8, EventName = nameof(ParsedManifestFile),
        Level = LogLevel.Information,
        Message = "Parsed Manifest file `{manifestFile}` to `{game}`"
    )]
    public static partial void ParsedManifestFile(
        ILogger logger,
        AbsolutePath manifestFile,
        OriginGame game
    );

    [LoggerMessage(
        EventId = 9, EventName = nameof(ExceptionWhileReadingManifest),
        Level = LogLevel.Warning,
        Message = "Exception while reading Manifest file `{manifestFile}`"
    )]
    public static partial void ExceptionWhileReadingManifest(
        ILogger logger,
        Exception e,
        AbsolutePath manifestFile
    );
}
