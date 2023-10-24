namespace GameFinder.Common.Tests;

public class ReferenceTypeOrExceptionTests
{
    private record TestClass;

    private static readonly TestClass TestValue = new();
    private static ReferenceTypeOrException<TestClass> CreateWithValue() => new(value: TestValue);

    private static (ReferenceTypeOrException<TestClass>, Exception) CreateWithException()
    {
        var exception = new NotSupportedException("Testing");
        return (new ReferenceTypeOrException<TestClass>(exception), exception);
    }

    [Fact]
    public void Test_GetValue_WithValue()
    {
        var value = CreateWithValue();

        var act = () => value.GetValue();
        act.Should().NotThrow<InvalidOperationException>();

        var res = act();
        res.Should().Be(TestValue);
    }

    [Fact]
    public void Test_GetValue_WithException()
    {
        var (value, _) = CreateWithException();

        var act = () => value.GetValue();
        act
            .Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("This instance doesn't contain a value but an exception: \"System.NotSupportedException: Testing\"");
    }

    [Fact]
    public void Test_TryGetValue_WithValue()
    {
        var value = CreateWithValue();

        var res = value.TryGetValue(out var actual);
        res.Should().BeTrue();
        actual.Should().Be(TestValue);
    }

    [Fact]
    public void Test_TryGetValue_WithException()
    {
        var (value, _) = CreateWithException();

        var res = value.TryGetValue(out var actual);
        res.Should().BeFalse();
        actual.Should().BeNull();
    }

    [Fact]
    public void Test_GetException_WithValue()
    {
        var value = CreateWithValue();

        var act = () => value.GetException();
        act
            .Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("This instance doesn't contain an exception but a value: \"TestClass { }\"");
    }

    [Fact]
    public void Test_GetException_WithException()
    {
        var (value, exception) = CreateWithException();

        var act = () => value.GetException();
        act.Should().NotThrow<InvalidOperationException>();

        var res = act();
        res.Should().Be(exception);
    }

    [Fact]
    public void Test_ThrowException_WithValue()
    {
        var value = CreateWithValue();

        var act = () => value.ThrowException();
        act
            .Should().ThrowExactly<InvalidOperationException>()
            .WithMessage("This instance doesn't contain an exception but a value: \"TestClass { }\"");
    }

    [Fact]
    public void Test_ThrowException_WithException()
    {
        var (value, exception) = CreateWithException();

        var act = () => value.ThrowException();
        act.Should().ThrowExactly<NotSupportedException>().And.Should().Be(exception);
    }

    [Fact]
    public void HasValue_WithValue()
    {
        var value = CreateWithValue();

        var res = value.HasValue();
        res.Should().BeTrue();
    }

    [Fact]
    public void HasValue_WithException()
    {
        var (value, _) = CreateWithException();

        var res = value.HasValue();
        res.Should().BeFalse();
    }
}
