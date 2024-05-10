using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Finds games.
/// </summary>
[PublicAPI]
public interface IGameFinder
{
    /// <summary>
    /// Uses all registered handlers to find all games.
    /// </summary>
    IReadOnlyList<IGame> FindAllGames();

    /// <summary>
    /// Finds all games with the provided type.
    /// </summary>
    IReadOnlyList<TGame> FindAllGames<TGame>()
        where TGame : IGame;

    /// <summary>
    /// Tries to find a single game with the provided ID.
    /// </summary>
    bool TryFindGameWithId<TId, TGame>([NotNullWhen(true)] out TGame? game)
        where TGame : IGame, IGameId<TId>
        where TId : IId<TId>;

    /// <summary>
    /// Tries to find a single game using many IDs as reference.
    /// </summary>
    bool TryFindGameWithManyIds([NotNullWhen(true)] out IGame? game, IId[] ids);
}
