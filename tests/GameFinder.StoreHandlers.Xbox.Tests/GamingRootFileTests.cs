using GameFinder.StoreHandlers.Xbox.Serialization;
using NexusMods.Paths;
using TestUtils;
using Xunit.Abstractions;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public class GamingRootFileTests : TestWrapper
{
    public GamingRootFileTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task Test_ParseManifest()
    {
        var file = GetTestFile("GamingRoot");
        var bytes = await FileSystem.ReadAllBytesAsync(file);

        var res = GamingRootFile.ParseGamingRootFiles(
            Logger,
            bytes,
            file
        );

        res.Should().NotBeNull();
        res!.FilePath.Should().Be(file);
        res.Folders.Should().Equal(RelativePath.FromUnsanitizedInput("XboxGames"));
        res.GetAbsoluteFolderPaths().Should().Equal(file.Parent.Combine("XboxGames"));
    }
}
