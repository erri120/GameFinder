using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using OneOf;

namespace GameFinder.Common;

/// <summary>
/// Interface of store handlers.
/// </summary>
/// <typeparam name="TGame"></typeparam>
/// <typeparam name="TId"></typeparam>
[PublicAPI]
public abstract class AHandler<TGame, TId>
    where TGame : class, IGame
    where TId : notnull
{
    /// <summary>
    /// Method that accepts a <typeparamref name="TGame"/> and returns the
    /// <typeparamref name="TId"/> of it. This is useful for constructing
    /// key-based data types like <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    public abstract Func<TGame, TId> IdSelector { get; }

    /// <summary>
    /// Custom equality comparer for <typeparamref name="TId"/>. This is useful
    /// for constructing key-based data types like <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    public abstract IEqualityComparer<TId>? IdEqualityComparer { get; }

    /// <summary>
    /// Finds all games installed with this store.
    /// </summary>
    /// <returns></returns>
    [MustUseReturnValue]
    public abstract IEnumerable<OneOf<TGame, ErrorMessage>> FindAllGames();

    /// <summary>
    /// Calls <see cref="FindAllGames"/> and converts the result into a dictionary where
    /// the key is the id of the game.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    [MustUseReturnValue]
    public IReadOnlyDictionary<TId, TGame> FindAllGamesById(out ErrorMessage[] errors)
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
    public TGame? FindOneGameById(TId id, out ErrorMessage[] errors)
    {
        var allGames = FindAllGamesById(out errors);
        return allGames.TryGetValue(id, out var game) ? game : null;
    }

    /// <inheritdoc cref="Utils.SanitizeInputPath"/>
    protected static string SanitizeInputPath(string input) => Utils.SanitizeInputPath(input);
}
