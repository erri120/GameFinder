using System.Globalization;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingAppId(InMemoryFileSystem fs, int id)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = basePath.CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @"
""AppState""
{
}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have the value \"appid\"");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingName(InMemoryFileSystem fs, int id)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = basePath.CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""AppState""
{{
    ""appid""       ""{id.ToString(CultureInfo.InvariantCulture)}""
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have the value \"name\"");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingInstallDir(InMemoryFileSystem fs, int id, string name)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = basePath.CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""AppState""
{{
    ""appid""       ""{id.ToString(CultureInfo.InvariantCulture)}""
    ""name""        ""{name}""
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have the value \"installdir\"");
    }
}
