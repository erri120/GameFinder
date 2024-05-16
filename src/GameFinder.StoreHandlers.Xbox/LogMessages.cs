using System;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Xbox;

internal static partial class LogMessages
{
    [LoggerMessage(
        EventId = 0, EventName = nameof(ExceptionWhileParsingManifest),
        Level = LogLevel.Warning,
        Message = "Exception while parsing Manifest file `{manifestFilePath}`"
    )]
    public static partial void ExceptionWhileParsingManifest(
        ILogger logger,
        Exception e,
        AbsolutePath manifestFilePath
    );

    [LoggerMessage(
        EventId = 1, EventName = nameof(ManifestDeserializationFailed),
        Level = LogLevel.Warning,
        Message = "Deserialization of Manifest file `{manifestFilePath}` failed. Return value is `{returnValue} ({returnType})`"
    )]
    public static partial void ManifestDeserializationFailed(
        ILogger logger,
        AbsolutePath manifestFilePath,
        object? returnValue,
        Type? returnType
    );

    [LoggerMessage(
        EventId = 2, EventName = nameof(ExceptionWhileParsingGamingRootFile),
        Level = LogLevel.Warning,
        Message = "Exception while parsing GamingRoot file `{filePath}`"
    )]
    public static partial void ExceptionWhileParsingGamingRootFile(
        ILogger logger,
        Exception e,
        AbsolutePath filePath
    );

    [LoggerMessage(
        EventId = 3, EventName = nameof(MagicMismatch),
        Level = LogLevel.Warning,
        Message = "Magic mismatch in GamingRoot file `{filePath}`: expected `{expected}`, found `{actual}`"
    )]
    public static partial void MagicMismatch(
        ILogger logger,
        uint expected,
        uint actual,
        AbsolutePath filePath
    );

    [LoggerMessage(
        EventId = 4, EventName = nameof(CountTooHigh),
        Level = LogLevel.Warning,
        Message = "Found count too high in GamingRoot file `{filePath}`: max is `{max}`, found `{actual}`"
    )]
    public static partial void CountTooHigh(
        ILogger logger,
        uint max,
        uint actual,
        AbsolutePath filePath
    );

    [LoggerMessage(
        EventId = 5, EventName = nameof(MissingNullTerminator),
        Level = LogLevel.Warning,
        Message = "Missing null terminator in GamingRoot file `{filePath}`"
    )]
    public static partial void MissingNullTerminator(
        ILogger logger,
        AbsolutePath filePath
    );
}
