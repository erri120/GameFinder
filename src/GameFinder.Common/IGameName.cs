using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a game with a name.
/// </summary>
/// <seealso cref="IGame"/>
/// <seealso cref="IGameId{TId}"/>
[PublicAPI]
public interface IGameName
{
    /// <summary>
    /// Gets the name of the game.
    /// </summary>
    /// <remarks>
    /// This can be used as a display string.
    /// </remarks>
    string Name { get; }
}
