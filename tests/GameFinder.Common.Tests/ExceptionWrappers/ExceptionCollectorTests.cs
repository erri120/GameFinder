namespace GameFinder.Common.Tests;

public class ExceptionCollectorTests
{
    private record TestClass;

    private const int State = 0;
    private const int TestValueType = 1;
    private const int TestUnmanagedType = 1;
    private static readonly TestClass TestReferenceType = new();

    private static int GetValueType() => TestValueType;
    private static int GetUnmanagedType() => TestUnmanagedType;
    private static TestClass GetReferenceType() => TestReferenceType;
    private static T ThrowException<T>() => throw new NotSupportedException("Testing");

    private static int GetValueType(int _) => TestValueType;
    private static int GetUnmanagedType(int _) => TestUnmanagedType;
    private static TestClass GetReferenceType(int _) => TestReferenceType;
    private static T ThrowException<T>(int _) => throw new NotSupportedException("Testing");

    private static void AssertNoException(ExceptionCollector collector)
    {
        collector.HasExceptions().Should().BeFalse();
        collector.GetExceptions().Should().BeEmpty();
    }

    private static void AssertHasException(ExceptionCollector collector, Exception exception)
    {
        collector.HasExceptions().Should().BeTrue();
        collector.GetExceptions().Should().ContainSingle().Which.Should().Be(exception);
    }

    [Fact]
    public void Test_WrapReferenceType_WithValue()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapReferenceType(GetReferenceType);
        AssertNoException(collector);
    }

    [Fact]
    public void Test_WrapReferenceType_WithException()
    {
        var collector = new ExceptionCollector();

        var res = collector.WrapReferenceType(ThrowException<TestClass>);
        AssertHasException(collector, res.GetException());
    }

    [Fact]
    public void Test_WrapReferenceType_WithState_WithValue()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapReferenceType(State, GetReferenceType);
        AssertNoException(collector);
    }

    [Fact]
    public void Test_WrapReferenceType_WithState_WithException()
    {
        var collector = new ExceptionCollector();

        var res = collector.WrapReferenceType(State, ThrowException<TestClass>);
        AssertHasException(collector, res.GetException());
    }

    [Fact]
    public void Test_WrapValueType_WithValue()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapValueType(GetValueType);
        AssertNoException(collector);
    }

    [Fact]
    public void Test_WrapValueType_WithException()
    {
        var collector = new ExceptionCollector();

        var res = collector.WrapValueType(ThrowException<int>);
        AssertHasException(collector, res.GetException());
    }

    [Fact]
    public void Test_WrapValueType_WithState_WithValue()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapValueType(State, GetValueType);
        AssertNoException(collector);
    }

    [Fact]
    public void Test_WrapValueType_WithState_WithException()
    {
        var collector = new ExceptionCollector();

        var res = collector.WrapValueType(State, ThrowException<int>);
        AssertHasException(collector, res.GetException());
    }

    [Fact]
    public void Test_WrapUnmanaged_WithValue()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapUnmanagedType(GetUnmanagedType);
        AssertNoException(collector);
    }

    [Fact]
    public void Test_WrapUnmanaged_WithException()
    {
        var collector = new ExceptionCollector();

        var res = collector.WrapUnmanagedType(ThrowException<int>);
        AssertHasException(collector, res.GetException());
    }

    [Fact]
    public void Test_WrapUnmanaged_WithState_WithValue()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapUnmanagedType(State, GetUnmanagedType);
        AssertNoException(collector);
    }

    [Fact]
    public void Test_WrapUnmanaged_WithState_WithException()
    {
        var collector = new ExceptionCollector();

        var res = collector.WrapUnmanagedType(State, ThrowException<int>);
        AssertHasException(collector, res.GetException());
    }

    [Fact]
    public void Test_ThrowAllExceptions()
    {
        var collector = new ExceptionCollector();

        _ = collector.WrapReferenceType(ThrowException<TestClass>);
        _ = collector.WrapReferenceType(ThrowException<TestClass>);
        _ = collector.WrapReferenceType(ThrowException<TestClass>);
        _ = collector.WrapReferenceType(ThrowException<TestClass>);

        var act = () => collector.ThrowAllExceptions();
        act.Should().ThrowExactly<ExceptionCollectorAggregatedException>().And.Message.Should().NotBeEmpty();
    }
}
