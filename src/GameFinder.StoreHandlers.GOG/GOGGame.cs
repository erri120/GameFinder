using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Represents a game installed with GOG Galaxy.
/// </summary>
[PublicAPI]
public record GOGGame : IGame<GOGGameId>, IGameName
{
    /// <summary>
    /// Gets the ID of this game.
    /// </summary>
    /// <remarks>
    /// This corresponds to the "Product ID" field found on https://www.gogdb.org
    /// </remarks>
    public required GOGGameId Id { get; init; }

    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <inheritdoc/>
    public required AbsolutePath Path { get; init; }
}
