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
    /// Uses all registered handlers to find all installed games.
    /// </summary>
    /// <seealso cref="FindAllGames{TGame}"/>
    [MustUseReturnValue]
    IReadOnlyList<IGame> FindAllGames();

    /// <summary>
    /// Uses all registered handlers that implement <see cref="IHandler{TGame}"/> with <typeparamref name="TGame"/>
    /// to find all installed games of type <typeparamref name="TGame"/>.
    /// </summary>
    [MustUseReturnValue]
    IReadOnlyList<TGame> FindAllGames<TGame>()
        where TGame : IGame;

    /// <summary>
    /// Tries to find a single game with the provided ID.
    /// </summary>
    /// <seealso cref="TryFindGameWithId{TId, TGame}"/>
    bool TryFindGameWithId(IId id, [NotNullWhen(true)] out IGame? game);

    /// <summary>
    /// Tries to find a single game of type <typeparamref name="TGame"/> with the provided ID of type <typeparamref name="TId"/>.
    /// </summary>
    /// <remarks>
    /// Similar to <see cref="FindAllGames{TGame}"/>, this method only uses registered handlers that implement
    /// <see cref="IHandler{TGame}"/> with <typeparamref name="TGame"/>.
    /// </remarks>
    /// <seealso cref="TryFindGameWithId"/>
    bool TryFindGameWithId<TId, TGame>(TId id, [NotNullWhen(true)] out TGame? game)
        where TGame : IGame<TId>
        where TId : IId;

    /// <summary>
    /// Tries to find a single game which ID is contained in <paramref name="ids"/>.
    /// </summary>
    bool TryFindGameWithManyIds(IId[] ids, [NotNullWhen(true)] out IGame? game);

    /// <summary>
    /// Tries to find a single game of type <typeparamref name="TGame"/> which ID of type
    /// <typeparamref name="TId"/> is contained in <paramref name="ids"/>.
    /// </summary>
    bool TryFindGameWithManyIds<TId, TGame>(TId[] ids, [NotNullWhen(true)] out TGame? game)
        where TGame : IGame<TId>
        where TId : IId;
}
