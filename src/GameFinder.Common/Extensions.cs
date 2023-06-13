using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using OneOf;

namespace GameFinder.Common;

/// <summary>
/// Utility extensions for GameFinder.
/// </summary>
[PublicAPI]
public static class Extensions
{
    /// <summary>
    /// Fully enumerates <paramref name="results"/> and splits the results into
    /// two separate arrays.
    /// </summary>
    /// <param name="results"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static (TGame[] games, LogMessage[] messages) SplitResults<TGame>(
        [InstantHandle] this IEnumerable<OneOf<TGame, LogMessage>> results)
        where TGame : class, IGame
    {
        var allResults = results.ToArray();

        var games = allResults
            .Where(x => x.IsT0)
            .Select(x => x.AsT0)
            .ToArray();

        var messages = allResults
            .Where(x => x.IsT1)
            .Select(x => x.AsT1)
            .ToArray();

        return (games, messages);
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
    public static IReadOnlyDictionary<TKey, TElement> CustomToDictionary<TSource, TKey, TElement>(
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

    /// <summary>
    /// Returns <c>true</c> if the result is of type <typeparamref name="TGame"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static bool IsGame<TGame>(this OneOf<TGame, LogMessage> result)
        where TGame : class, IGame
    {
        return result.IsT0;
    }

    /// <summary>
    /// Returns <c>true</c> if the result is of type <see cref="LogMessage"/>.
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsMessage<T>(this OneOf<T, LogMessage> result)
    {
        return result.IsT1;
    }

    /// <summary>
    /// Returns the <typeparamref name="TGame"/> part of the result. This can throw if
    /// the result is not of type <typeparamref name="TGame"/>. Use <see cref="TryGetGame{TGame}"/>
    /// instead.
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the result is not of type <typeparamref name="TGame"/>.
    /// </exception>
    public static TGame AsGame<TGame>(this OneOf<TGame, LogMessage> result)
        where TGame : class, IGame
    {
        return result.AsT0;
    }

    /// <summary>
    /// Returns the <see cref="LogMessage"/> part of the result. This can
    /// throw if the result is not of type <see cref="LogMessage"/>. Use
    /// <see cref="TryGetMessage{T}"/> instead.
    /// </summary>
    /// <param name="result"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the result is not of type <see cref="LogMessage"/>.
    /// </exception>
    public static LogMessage AsMessage<T>(this OneOf<T, LogMessage> result)
    {
        return result.AsT1;
    }

    /// <summary>
    /// Returns the <typeparamref name="TGame"/> part of the result using the try-get
    /// pattern.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="game"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static bool TryGetGame<TGame>(this OneOf<TGame, LogMessage> result,
        [MaybeNullWhen(false)] out TGame game)
        where TGame : class, IGame
    {
        game = null;
        if (!result.IsGame()) return false;

        game = result.AsGame();
        return true;
    }

    /// <summary>
    /// Returns the <see cref="LogMessage"/> part of the result using the
    /// try-get pattern.
    /// </summary>
    /// <param name="result"></param>
    /// <param name="message"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool TryGetMessage<T>(this OneOf<T, LogMessage> result, out LogMessage message)
    {
        message = default;
        if (!result.IsMessage()) return false;

        message = result.AsMessage();
        return true;
    }
}
