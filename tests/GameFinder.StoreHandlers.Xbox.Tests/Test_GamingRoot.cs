using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public partial class XboxTests
{
    [Theory, AutoFileSystem]
    public void Test_ParseGamingRootFile1(InMemoryFileSystem fs, AbsolutePath gamingRootFile, string[] folderNames)
    {
        var expectedFolders = folderNames
            .Select(folderName => gamingRootFile.Parent.Combine(folderName))
            .ToList();

        var bytes = CreateGamingRootFile(expectedFolders);
        fs.AddFile(gamingRootFile, bytes);

        var results = XboxHandler.ParseGamingRootFile(fs, gamingRootFile);
        results.IsT0.Should().BeTrue();
        results.IsT1.Should().BeFalse();

        var actualFolders = results.AsT0;
        actualFolders!.Should().BeEquivalentTo(expectedFolders);
    }

    [Theory, AutoFileSystem]
    public void Test_ParseGamingRootFile2(InMemoryFileSystem fs, AbsolutePath gamingRootFilePath)
    {
        var bytes = new byte[]
        {
            0x52, 0x47, 0x42, 0x58, 0x01, 0x00, 0x00, 0x00, 0x58, 0x00, 0x62,
            0x00, 0x6f, 0x00, 0x78, 0x00, 0x47, 0x00, 0x61, 0x00, 0x6d, 0x00,
            0x65, 0x00, 0x73, 0x00, 0x00, 0x00,
        };

        fs.AddFile(gamingRootFilePath, bytes);

        var results = XboxHandler.ParseGamingRootFile(fs, gamingRootFilePath);
        results.IsT0.Should().BeTrue();
        results.IsT1.Should().BeFalse();

        var actualFolders = results.AsT0;
        actualFolders.Should().ContainSingle(x => x.FileName.Equals("XboxGames", StringComparison.Ordinal));
    }
}
