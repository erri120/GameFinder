namespace GameFinder.Common.Tests;

public class UnmanagedTypeOrExceptionTests : AValueOrExceptionTest<UnmanagedTypeOrException<int>, int>
{
    protected override int TestValue => 1;

    protected override UnmanagedTypeOrException<int> CreateWithValue() => new(value: TestValue);

    protected override (UnmanagedTypeOrException<int>, Exception) CreateWithException()
    {
        var exception = GetException();
        return (new UnmanagedTypeOrException<int>(exception), exception);
    }
}
