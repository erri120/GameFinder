using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Represents a game installed with Origin.
/// </summary>
[PublicAPI]
public record OriginGame(OriginGameId Id, AbsolutePath InstallPath) : IGame;
