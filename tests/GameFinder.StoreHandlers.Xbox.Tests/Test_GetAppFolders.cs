using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public partial class XboxTests
{
    [Theory, AutoFileSystem]
    public void Test_GetAppFolders(InMemoryFileSystem fs)
    {
        var expectedPaths = fs
            .EnumerateRootDirectories()
            .Select(rootDirectory =>
            {
                var modifiableWindowsAppsPath = rootDirectory
                    .Combine("Program Files")
                    .Combine("ModifiableWindowsApps");
                fs.AddDirectory(modifiableWindowsAppsPath);
                return modifiableWindowsAppsPath;
            })
            .ToArray();

        var (paths, errors) = XboxHandler.GetAppFolders(fs);
        errors.Should().BeEmpty();
        paths.Should().BeEquivalentTo(expectedPaths);
    }
}
