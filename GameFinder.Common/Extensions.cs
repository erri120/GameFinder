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
        return results
            .Select(result => result.Game)
            .Where(game => game is not null)
            .Select(game => game!);
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
        return results
            .Select(result => result.Error)
            .Where(error => error is not null)
            .Select(error => error!);
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
}
