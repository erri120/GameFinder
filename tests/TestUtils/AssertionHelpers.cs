using FluentAssertions;
using GameFinder.Common;

namespace TestUtils;

public static class AssertionHelpers
{
    public static IEnumerable<TGame> ShouldOnlyBeGames<TGame>(this ICollection<Result<TGame>> results)
        where TGame : class
    {
        results.Should().AllSatisfy(result =>
        {
            result.Error.Should().BeNull();
            result.Game.Should().NotBeNull();
        });

        return results.Select(result => result.Game!);
    }

    private static string ShouldOnlyBeOneError<TGame>(
        this ICollection<Result<TGame>> results) where TGame : class
    {
        results.Should().ContainSingle();

        var result = results.First();
        result.Game.Should().BeNull();
        result.Error.Should().NotBeNull();

        return result.Error!;
    }

    public static string ShouldOnlyBeOneError<TGame, TId>(
        this AHandler<TGame, TId> handler) where TGame : class
    {
        var results = handler.FindAllGames().ToArray();
        return results.ShouldOnlyBeOneError();
    }

    public static void ShouldFindAllGames<TGame, TId>(this AHandler<TGame, TId> handler,
        IEnumerable<TGame> expectedGames) where TGame : class
    {
        var results = handler.FindAllGames().ToArray();
        var games = results.ShouldOnlyBeGames();

        games.Should().Equal(expectedGames);
    }

    public static void ShouldFindAllGamesById<TGame, TId>(
        this AHandler<TGame, TId> handler,
        ICollection<TGame> expectedGames,
        Func<TGame, TId> keySelector) where TGame : class
    {
        var results = handler.FindAllGamesById(out var errors);
        errors.Should().BeEmpty();

        results.Should().ContainKeys(expectedGames.Select(keySelector));
        results.Should().ContainValues(expectedGames);
    }
}
