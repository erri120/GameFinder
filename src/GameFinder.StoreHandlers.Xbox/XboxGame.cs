using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Xbox;

/// <summary>
/// Represents a game installed with the Xbox App.
/// </summary>
[PublicAPI]
public record XboxGame : IGame<XboxGameId>, IGameName
{
    /// <inheritdoc/>
    public required XboxGameId Id { get; init; }

    /// <inheritdoc/>
    public required string Name { get; init; }

    /// <inheritdoc/>
    public required AbsolutePath Path { get; init; }
}
