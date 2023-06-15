using GameFinder.Common;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory]
    [InlineData(SchemaPolicy.Error, EADesktopHandler.SupportedSchemaVersion, true, MessageLevel.None)]
    [InlineData(SchemaPolicy.Ignore, EADesktopHandler.SupportedSchemaVersion + 1, true, MessageLevel.None)]
    [InlineData(SchemaPolicy.Warn, EADesktopHandler.SupportedSchemaVersion + 1, false, MessageLevel.Warn)]
    [InlineData(SchemaPolicy.Error, EADesktopHandler.SupportedSchemaVersion + 1, false, MessageLevel.Error)]
    public void Test_CreateSchemaVersionMessage(
        SchemaPolicy schemaPolicy, int schemaVersion, bool shouldMessageBeNull,
        MessageLevel expectedMessageType)
    {
        var (message, errType) = EADesktopHandler.CreateSchemaVersionMessage(
            schemaPolicy,
            schemaVersion,
            new InMemoryFileSystem().GetKnownPath(KnownPath.TempDirectory));

        if (shouldMessageBeNull)
            message.Should().BeNull();
        else
            message.Should().NotBeNull();

        errType.Should().Be(expectedMessageType);
    }
}
