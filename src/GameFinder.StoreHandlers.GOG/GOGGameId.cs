using JetBrains.Annotations;
using TransparentValueObjects;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Represents an ID for games installed with GOG Galaxy.
/// </summary>
[PublicAPI]
[ValueObject<ulong>]
public readonly partial struct GOGGameId { }
