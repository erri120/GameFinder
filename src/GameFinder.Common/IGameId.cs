using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a game with an ID.
/// </summary>
/// <seealso cref="IGame"/>
/// <seealso cref="IGameName"/>
[PublicAPI]
public interface IGameId<out TId> where TId : notnull
{
    /// <summary>
    /// Gets the ID of the game.
    /// </summary>
    TId Id { get; }
}
