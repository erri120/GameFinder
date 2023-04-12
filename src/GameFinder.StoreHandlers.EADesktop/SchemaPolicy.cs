using GameFinder.Common;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Policy to employ when the schema version doesn't match the supported schema version.
/// See <see cref="EADesktopHandler.SchemaPolicy"/> for more information.
/// </summary>
[PublicAPI]
public enum SchemaPolicy
{
    /// <summary>
    /// Completely ignores the new schema version.
    /// </summary>
    Ignore,

    /// <summary>
    /// Creates a warning about the new schema version. Note that this is represented as
    /// an error using <see cref="ErrorMessage"/>. This does not abort the
    /// parsing.
    /// </summary>
    Warn,

    /// <summary>
    /// Creates an error and aborts the parsing.
    /// </summary>
    Error,
}
