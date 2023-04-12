using System.Globalization;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_InvalidAppId(InMemoryFileSystem fs, int id, string name,
        string s)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = basePath.CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""AppState""
{{
    ""appid""       ""{s}""
    ""name""        ""{name}""
    ""installdir""      ""{name}""
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.ToString().Should().StartWith($"Exception while parsing file {manifest}:\nSystem.FormatException:");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_InvalidFormat(InMemoryFileSystem fs, int id, string format)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var manifest = basePath.CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, @$"
""{format}""
{{
}}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Manifest {manifest} is not a valid format!");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_InvalidManifest(InMemoryFileSystem fs, int id)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var randomBytes = new byte[128];

        var manifest = basePath.CombineUnchecked($"{id.ToString(CultureInfo.InvariantCulture)}.acf");
        fs.AddFile(manifest, randomBytes);

        var error = handler.ShouldOnlyBeOneError();
        error.ToString().Should().StartWith($"Exception while parsing file {manifest}:\nValveKeyValue.KeyValueException:");
    }
}
