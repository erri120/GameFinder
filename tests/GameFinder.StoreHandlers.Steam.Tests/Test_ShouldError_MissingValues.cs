using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingAppId(MockFileSystem fs, int id)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = fs.Path.Combine(basePath, $"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @"
""AppState""
{
}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have the value \"appid\"");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingName(MockFileSystem fs, int id)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = fs.Path.Combine(basePath, $"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""AppState""
{{
    ""appid""       ""{id.ToString(CultureInfo.InvariantCulture)}""
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} does not have the value \"name\"");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingInstallDir(MockFileSystem fs, int id, string name)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = fs.Path.Combine(basePath, $"{id.ToString(CultureInfo.InvariantCulture)}.acf");
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
