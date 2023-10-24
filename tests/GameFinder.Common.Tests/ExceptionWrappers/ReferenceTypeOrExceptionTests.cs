namespace GameFinder.Common.Tests;

public class ReferenceTypeOrExceptionTests : AValueOrExceptionTest<ReferenceTypeOrException<ReferenceTypeOrExceptionTests.TestClass>, ReferenceTypeOrExceptionTests.TestClass>
{
    public record TestClass;

    protected override TestClass TestValue { get; } = new();

    protected override ReferenceTypeOrException<TestClass> CreateWithValue() => new(value: TestValue);

    protected override (ReferenceTypeOrException<TestClass>, Exception) CreateWithException()
    {
        var exception = GetException();
        return (new ReferenceTypeOrException<TestClass>(exception), exception);
    }
}
