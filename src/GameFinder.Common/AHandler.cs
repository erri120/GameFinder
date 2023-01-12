using System.Collections.Generic;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Interface of store handlers.
/// </summary>
/// <typeparam name="TGame"></typeparam>
/// <typeparam name="TId"></typeparam>
[PublicAPI]
public abstract class AHandler<TGame, TId>
    where TGame: class
{
    /// <summary>
    /// Finds all games installed with this store. The return type <see cref="Result{TGame}"/>
    /// will always be a non-null game or a non-null error message.
    /// </summary>
    /// <returns></returns>
    [MustUseReturnValue]
    public abstract IEnumerable<Result<TGame>> FindAllGames();

    /// <summary>
    /// Calls <see cref="FindAllGames"/> and converts the result into a dictionary where
    /// the key is the id of the game.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    [MustUseReturnValue]
    public abstract IDictionary<TId, TGame> FindAllGamesById(out string[] errors);

    /// <summary>
    /// Wrapper around <see cref="FindAllGamesById"/> if you just need to find one game.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="errors"></param>
    /// <returns></returns>
    [MustUseReturnValue]
    public TGame? FindOneGameById(TId id, out string[] errors)
    {
        var allGames = FindAllGamesById(out errors);
        return allGames.TryGetValue(id, out var game) ? game : null;
    }
}
