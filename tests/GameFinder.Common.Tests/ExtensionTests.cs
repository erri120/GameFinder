using Moq;
using OneOf;

namespace GameFinder.Common.Tests;

public class ExtensionTests
{
    private static readonly IGame Game = new Mock<IGame>().Object;
    private static readonly LogMessage Message = new(string.Empty);

    private static readonly OneOf<IGame, LogMessage> GameResult = OneOf<IGame, LogMessage>.FromT0(Game);
    private static readonly OneOf<IGame, LogMessage> MessageResult = OneOf<IGame, LogMessage>.FromT1(Message);

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
        var result = MessageResult;
        result.IsGame().Should().BeFalse();
    }

    [Fact]
    public void Test_IsMessage_True()
    {
        var result = MessageResult;
        result.IsMessage().Should().BeTrue();
    }

    [Fact]
    public void Test_IsMessage_False()
    {
        var result = GameResult;
        result.IsMessage().Should().BeFalse();
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
        var result = MessageResult;
        result
            .Invoking(x => x.AsGame())
            .Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Test_AsMessage()
    {
        var result = MessageResult;
        result.AsMessage().Should().Be(string.Empty);
    }

    [Fact]
    public void Test_AsMessage_InvalidOperationException()
    {
        var result = GameResult;
        result
            .Invoking(x => x.AsMessage())
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
        var result = MessageResult;
        result.TryGetGame(out var game).Should().BeFalse();
        game.Should().BeNull();
    }

    [Fact]
    public void Test_TryGetMessage_True()
    {
        var result = MessageResult;
        result.TryGetMessage(out _).Should().BeTrue();
    }

    [Fact]
    public void Test_TryGetMessage_False()
    {
        var result = GameResult;
        result.TryGetMessage(out var message).Should().BeFalse();
        message.Should().Be(default(LogMessage));
    }
}
