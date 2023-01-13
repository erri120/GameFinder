using System.Globalization;
using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldError_InvalidAppId(MockFileSystem fs, int id, string name,
        string s)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = fs.Path.Combine(basePath,$"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""AppState""
{{
    ""appid""       ""{s}""
    ""name""        ""{name}""
    ""installdir""      ""{name}""
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().StartWith($"Exception while parsing file {manifest}:\nSystem.FormatException:");
    }

    [Theory, AutoData]
    public void Test_ShouldError_InvalidFormat(MockFileSystem fs, int id, string format)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = fs.Path.Combine(basePath,$"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""{format}""
{{
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} is not a valid format!");
    }

    [Theory, AutoData]
    public void Test_ShouldError_InvalidManifest(MockFileSystem fs, int id)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var randomBytes = new byte[128];

        var manifest = fs.Path.Combine(basePath,$"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, new MockFileData(randomBytes));

        var error = handler.ShouldOnlyBeOneError();
        error.Should().StartWith($"Exception while parsing file {manifest}:\nValveKeyValue.KeyValueException:");
    }
}
