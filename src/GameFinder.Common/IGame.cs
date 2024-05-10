using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.Common;

/// <summary>
/// Represents a game found by GameFinder.
/// </summary>
/// <seealso cref="IGameId{TId}"/>
/// <seealso cref="IGameName"/>
[PublicAPI]
public interface IGame
{
    /// <summary>
    /// Gets the path to the game.
    /// </summary>
    /// <remarks>
    /// This is not guaranteed to point to a directory. Whether this
    /// points to a file or a directory is up to the implementation.
    /// </remarks>
    AbsolutePath Path { get; }
}
