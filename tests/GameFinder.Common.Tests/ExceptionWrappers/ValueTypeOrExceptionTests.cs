namespace GameFinder.Common.Tests;

public class ValueTypeOrExceptionTests : AValueOrExceptionTest<ValueTypeOrException<int>, int>
{
    protected override int TestValue => 1;

    protected override ValueTypeOrException<int> CreateWithValue() => new(value: TestValue);

    protected override (ValueTypeOrException<int>, Exception) CreateWithException()
    {
        var exception = GetException();
        return (new ValueTypeOrException<int>(exception), exception);
    }
}
