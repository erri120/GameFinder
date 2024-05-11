using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.Common;

/// <summary>
/// Represents a game found by GameFinder.
/// </summary>
/// <seealso cref="IGame{TId}"/>
/// <seealso cref="IGameName"/>
[PublicAPI]
public interface IGame
{
    /// <summary>
    /// Gets the ID of the game.
    /// </summary>
    /// <seealso cref="IGame{TId}"/>
    IId Id { get; }

    /// <summary>
    /// Gets the path to the game.
    /// </summary>
    /// <remarks>
    /// This is not guaranteed to point to a directory. Whether this
    /// points to a file or a directory is up to the implementation.
    /// </remarks>
    AbsolutePath Path { get; }
}

/// <summary>
/// Represents a game with a concrete ID type.
/// </summary>
public interface IGame<out TId> : IGame
    where TId : IId<TId>
{
    /// <inheritdoc/>
    IId IGame.Id => Id;

    /// <summary>
    /// Gets the ID of the game.
    /// </summary>
    new TId Id { get; }
}
