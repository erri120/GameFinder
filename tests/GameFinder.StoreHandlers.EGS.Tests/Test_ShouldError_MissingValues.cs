using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_Missing_CatalogItemId(InMemoryFileSystem fs,
        InMemoryRegistry registry, string manifestItemName)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var manifest = manifestDir.CombineUnchecked($"{manifestItemName}.item");
        fs.AddFile(manifest, "{}");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"CatalogItemId\"");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_Missing_DisplayName(InMemoryFileSystem fs,
        InMemoryRegistry registry, string manifestItemName, string value)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var manifest = manifestDir.CombineUnchecked($"{manifestItemName}.item");
        fs.AddFile(manifest, $@"{{ ""CatalogItemId"": ""{value}"" }}");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"DisplayName\"");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_Missing_InstallLocation(InMemoryFileSystem fs,
        InMemoryRegistry registry, string manifestItemName, string value)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);

        var manifest = manifestDir.CombineUnchecked($"{manifestItemName}.item");
        fs.AddFile(manifest, $@"{{ ""CatalogItemId"": ""{value}"", ""DisplayName"": ""{value}"" }}");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have a value \"InstallLocation\"");
    }
}
