namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory]
    [InlineData(SchemaPolicy.Error, EADesktopHandler.SupportedSchemaVersion, true, false)]
    [InlineData(SchemaPolicy.Ignore, EADesktopHandler.SupportedSchemaVersion + 1, true, false)]
    [InlineData(SchemaPolicy.Warn, EADesktopHandler.SupportedSchemaVersion + 1, false, false)]
    [InlineData(SchemaPolicy.Error, EADesktopHandler.SupportedSchemaVersion + 1, false, true)]
    public void Test_CreateSchemaVersionMessage(
        SchemaPolicy schemaPolicy, int schemaVersion, bool shouldMessageBeNull,
        bool expectedIsErrorValue)
    {
        var (message, isError) = EADesktopHandler.CreateSchemaVersionMessage(
            schemaPolicy,
            schemaVersion,
            "");

        if (shouldMessageBeNull)
            message.Should().BeNull();
        else
            message.Should().NotBeNull();

        isError.Should().Be(expectedIsErrorValue);
    }
}
