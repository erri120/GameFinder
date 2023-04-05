using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoFileSystem]
    public void Test_InstallInfoToGame(InMemoryFileSystem fileSystem, string baseSlug, string installCheck, string baseInstallPathName, string softwareId)
    {
        var baseInstallPath = fileSystem.GetKnownPath(KnownPath.TempDirectory)
            .CombineUnchecked(baseInstallPathName);

        var installInfo = new InstallInfo
        {
            BaseSlug = baseSlug,
            InstallCheck = installCheck,
            BaseInstallPath = baseInstallPath.GetFullPath(),
            SoftwareId = softwareId,
        };

        var fs = new InMemoryFileSystem();
        var (game, error) = EADesktopHandler.InstallInfoToGame(fs, installInfo, 0, fs.GetKnownPath(KnownPath.TempDirectory));
        error.Should().BeNull();
        game.Should().NotBeNull();
    }
}
