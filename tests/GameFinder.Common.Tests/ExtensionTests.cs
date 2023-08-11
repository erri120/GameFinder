using NSubstitute;
using OneOf;

namespace GameFinder.Common.Tests;

public class ExtensionTests
{
    private static readonly IGame Game = Substitute.For<IGame>();
    private static readonly ErrorMessage Error = new(string.Empty);

    private static readonly OneOf<IGame, ErrorMessage> GameResult = OneOf<IGame, ErrorMessage>.FromT0(Game);
    private static readonly OneOf<IGame, ErrorMessage> ErrorResult = OneOf<IGame, ErrorMessage>.FromT1(Error);

    [Fact]
    public void Test_CustomToDictionary()
    {
        var input = new[] { "a", "b", "ab" };
        var output = input.CustomToDictionary(x => x.Length, x => x);
        output.Should().ContainKeys(1, 2);
        output.Should().ContainValues("a", "ab");
    }

    [Fact]
    public void Test_IsGame_True()
    {
        var result = GameResult;
        result.IsGame().Should().BeTrue();
    }

    [Fact]
    public void Test_IsGame_False()
    {
        var result = ErrorResult;
        result.IsGame().Should().BeFalse();
    }

    [Fact]
    public void Test_IsError_True()
    {
        var result = ErrorResult;
        result.IsError().Should().BeTrue();
    }

    [Fact]
    public void Test_IsError_False()
    {
        var result = GameResult;
        result.IsError().Should().BeFalse();
    }

    [Fact]
    public void Test_AsGame()
    {
        var result = GameResult;
        result.AsGame().Should().Be(Game);
    }

    [Fact]
    public void Test_AsGame_InvalidOperationException()
    {
        var result = ErrorResult;
        result
            .Invoking(x => x.AsGame())
            .Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Test_AsError()
    {
        var result = ErrorResult;
        result.AsError().Should().Be(string.Empty);
    }

    [Fact]
    public void Test_AsError_InvalidOperationException()
    {
        var result = GameResult;
        result
            .Invoking(x => x.AsError())
            .Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Test_TryGetGame_True()
    {
        var result = GameResult;
        result.TryGetGame(out _).Should().BeTrue();
    }

    [Fact]
    public void Test_TryGetGame_False()
    {
        var result = ErrorResult;
        result.TryGetGame(out var game).Should().BeFalse();
        game.Should().BeNull();
    }

    [Fact]
    public void Test_TryGetError_True()
    {
        var result = ErrorResult;
        result.TryGetError(out _).Should().BeTrue();
    }

    [Fact]
    public void Test_TryGetError_False()
    {
        var result = GameResult;
        result.TryGetError(out var error).Should().BeFalse();
        error.Should().Be(default(ErrorMessage));
    }
}
