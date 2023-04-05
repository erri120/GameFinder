using Vogen;

namespace GameFinder.StoreHandlers.Steam;

/// <summary>
/// Represents an id for games installed with Steam.
/// </summary>
[ValueObject<int>]
public readonly partial struct SteamGameId { }
