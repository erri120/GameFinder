using System;
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
    where TGame : class
    where TId : notnull
{
    /// <summary>
    /// Method that accepts a <typeparamref name="TGame"/> and returns the
    /// <typeparamref name="TId"/> of it. This is useful for constructing
    /// key-based data types like <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public abstract Func<TGame, TId> IdSelector { get; }

    /// <summary>
    /// Custom equality comparer for <typeparamref name="TId"/>. This is useful
    /// for constructing key-based data types like <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    protected abstract IEqualityComparer<TId>? IdEqualityComparer { get; }

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
    public IDictionary<TId, TGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(IdSelector, game => game, IdEqualityComparer ?? EqualityComparer<TId>.Default);
    }

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
