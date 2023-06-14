using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests.Models;

public class AppManifestTests
{
    [Theory, AutoFileSystem]
    public void Test_Equality(AbsolutePath manifestPath)
    {
        var value = ArrangeHelper.CreateAppManifest(manifestPath);
        value.Equals(value).Should().BeTrue();
        value.GetHashCode().Should().Be(value.GetHashCode());
    }
}
