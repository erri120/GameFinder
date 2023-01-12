using FluentAssertions;
using GameFinder.Common;

namespace TestUtils;

public static class AssertionHelpers
{
    public static IEnumerable<TGame> ShouldOnlyBeGames<TGame>(this ICollection<Result<TGame>> results)
        where TGame: class
    {
        results.Should().AllSatisfy(result =>
        {
            result.Error.Should().BeNull();
            result.Game.Should().NotBeNull();
        });

        return results.Select(result => result.Game!);
    }

    public static string ShouldOnlyBeOneError<TGame>(
        this ICollection<Result<TGame>> results) where TGame: class
    {
        results.Should().ContainSingle();

        var result = results.First();
        result.Game.Should().BeNull();
        result.Error.Should().NotBeNull();

        return result.Error!;
    }
}
