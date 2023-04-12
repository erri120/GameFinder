using OneOf;

namespace GameFinder.Common.Tests;

public class ExtensionTests
{
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
        OneOf<string, ErrorMessage> result = string.Empty;
        result.IsGame().Should().BeTrue();
    }

    [Fact]
    public void Test_IsGame_False()
    {
        OneOf<string, ErrorMessage> result = new ErrorMessage(string.Empty);
        result.IsGame().Should().BeFalse();
    }

    [Fact]
    public void Test_IsError_True()
    {
        OneOf<string, ErrorMessage> result = new ErrorMessage(string.Empty);
        result.IsError().Should().BeTrue();
    }

    [Fact]
    public void Test_IsError_False()
    {
        OneOf<string, ErrorMessage> result = string.Empty;
        result.IsError().Should().BeFalse();
    }

    [Fact]
    public void Test_AsGame()
    {
        OneOf<string, ErrorMessage> result = string.Empty;
        result.AsGame().Should().Be(string.Empty);
    }

    [Fact]
    public void Test_AsGame_InvalidOperationException()
    {
        OneOf<string, ErrorMessage> result = new ErrorMessage(string.Empty);
        result
            .Invoking(x => x.AsGame())
            .Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Test_AsError()
    {
        OneOf<string, ErrorMessage> result = new ErrorMessage(string.Empty);
        result.AsError().Should().Be(string.Empty);
    }

    [Fact]
    public void Test_AsError_InvalidOperationException()
    {
        OneOf<string, ErrorMessage> result = string.Empty;
        result
            .Invoking(x => x.AsError())
            .Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Test_TryGetGame_True()
    {
        OneOf<string, ErrorMessage> result = string.Empty;
        result.TryGetGame(out _).Should().BeTrue();
    }

    [Fact]
    public void Test_TryGetGame_False()
    {
        OneOf<string, ErrorMessage> result = new ErrorMessage(string.Empty);
        result.TryGetGame(out var game).Should().BeFalse();
        game.Should().BeNull();
    }

    [Fact]
    public void Test_TryGetError_True()
    {
        OneOf<string, ErrorMessage> result = new ErrorMessage(string.Empty);
        result.TryGetError(out _).Should().BeTrue();
    }

    [Fact]
    public void Test_TryGetError_False()
    {
        OneOf<string, ErrorMessage> result = string.Empty;
        result.TryGetError(out var error).Should().BeFalse();
        error.Should().Be(default(ErrorMessage));
    }
}
