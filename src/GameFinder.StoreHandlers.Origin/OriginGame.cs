using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Represents a game installed with Origin.
/// </summary>
[PublicAPI]
public record OriginGame : IGame, IGameId<OriginGameId>
{
    /// <inheritdoc/>
    public required OriginGameId Id { get; init; }

    /// <inheritdoc/>
    public required AbsolutePath Path { get; init; }
}
