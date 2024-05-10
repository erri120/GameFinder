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
}
