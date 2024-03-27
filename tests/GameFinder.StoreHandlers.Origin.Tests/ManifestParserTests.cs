using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;
using Xunit.Abstractions;

namespace GameFinder.StoreHandlers.Origin.Tests;

public class ManifestParserTests
{
    private readonly ILogger _logger;
    public ManifestParserTests(ITestOutputHelper output)
    {
        _logger = new XunitLogger(output);
    }

    [Theory, AutoFileSystem]
    public void Test_ParseManifestFile(IFileSystem fileSystem, AbsolutePath manifestFile, AbsolutePath installPath)
    {
        var res = ManifestParser.ParseManifestFile(
            _logger,
            fileSystem,
            contents: $"iD=foo&id=&dipInstallPatH=&dipInstallPath={installPath.ToString()}",
            manifestFile
        );

        res.Should().NotBeNull();
        res.Should().Be(new OriginGame(OriginGameId.From("foo"), installPath));
    }

    [Theory, AutoFileSystem]
    public void Test_SkipSteamGame(IFileSystem fileSystem, AbsolutePath manifestFile)
    {
        var res = ManifestParser.ParseManifestFile(
            _logger,
            fileSystem,
            contents: "id=foo@steam&dipInstallPath=bar",
            manifestFile
        );

        res.Should().BeNull();
    }
}
