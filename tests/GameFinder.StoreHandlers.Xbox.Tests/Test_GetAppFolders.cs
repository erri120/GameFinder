using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public partial class XboxTests
{
    [Theory, AutoFileSystem]
    public void Test_GetAppFolders(InMemoryFileSystem fs, AbsolutePath programFilesDirectory)
    {
        var overlayFileSystem = fs.CreateOverlayFileSystem(new Dictionary<AbsolutePath, AbsolutePath>(),
            new Dictionary<KnownPath, AbsolutePath>
            {
                { KnownPath.ProgramFilesDirectory, programFilesDirectory },
            });

        var expectedPath = programFilesDirectory.CombineUnchecked("ModifiableWindowsApps");
        overlayFileSystem.CreateDirectory(expectedPath);

        var (paths, errors) = XboxHandler.GetAppFolders(overlayFileSystem);
        errors.Should().BeEmpty();
        paths.Should().ContainSingle(x => x == expectedPath);
    }
}
