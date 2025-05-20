using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Represents a game installed with GOG Galaxy.
/// </summary>
[PublicAPI]
public record GOGGame(GOGGameId Id, string Name, AbsolutePath Path, ulong BuildId) : IGame;
