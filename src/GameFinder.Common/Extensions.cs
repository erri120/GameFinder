using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Utility extensions for GameFinder.
/// </summary>
[PublicAPI]
public static class Extensions
{
    /// <summary>
    /// Only returns non-null games from the <see cref="Result{TGame}"/> and discards
    /// the error messages.
    /// </summary>
    /// <param name="results"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static IEnumerable<TGame> OnlyGames<TGame>(
        this IEnumerable<Result<TGame>> results)
        where TGame : class
    {
        foreach (var result in results)
        {
            if (result.Game is not null)
            {
                yield return result.Game;
            }
        }
    }

    /// <summary>
    /// Only returns non-null error messages from the <see cref="Result{TGame}"/> and
    /// discards the games.
    /// </summary>
    /// <param name="results"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static IEnumerable<string> OnlyErrors<TGame>(
        this IEnumerable<Result<TGame>> results)
        where TGame : class
    {
        foreach (var result in results)
        {
            if (result.Error is not null)
            {
                yield return result.Error;
            }
        }
    }

    /// <summary>
    /// Fully enumerates <paramref name="results"/> and splits the results into
    /// two separate arrays.
    /// </summary>
    /// <param name="results"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static (TGame[] games, string[] errors) SplitResults<TGame>(
        [InstantHandle] this IEnumerable<Result<TGame>> results)
        where TGame : class
    {
        var allResults = results.ToArray();

        var games = allResults.OnlyGames().ToArray();
        var errors = allResults.OnlyErrors().ToArray();

        return (games, errors);
    }

    /// <summary>
    /// Custom <see cref="Enumerable.ToDictionary{TSource,TKey}(System.Collections.Generic.IEnumerable{TSource},System.Func{TSource,TKey})"/>
    /// function that skips duplicate keys.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="keySelector"></param>
    /// <param name="elementSelector"></param>
    /// <param name="comparer"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TElement> CustomToDictionary<TSource, TKey, TElement>(
        [InstantHandle] this IEnumerable<TSource> source, Func<TSource, TKey> keySelector,
        Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer = null)
        where TKey : notnull
    {
        var dictionary = new Dictionary<TKey, TElement>(comparer);

        foreach (var element in source)
        {
            var key = keySelector(element);
            if (dictionary.ContainsKey(key)) continue;

            dictionary.Add(key, elementSelector(element));
        }

        return dictionary;
    }
}
