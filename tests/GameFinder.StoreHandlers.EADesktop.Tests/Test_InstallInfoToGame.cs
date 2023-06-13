using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoFileSystem]
    public void Test_InstallInfoToGame(InMemoryRegistry registry, InMemoryFileSystem fileSystem, string baseSlug, string dlcSubPath, string installCheck, string baseInstallPathName, string softwareId, string executableCheck)
    {
        var baseInstallPath = fileSystem.GetKnownPath(KnownPath.TempDirectory)
            .CombineUnchecked(baseInstallPathName);

        var installInfo = new InstallInfo(
            baseInstallPath.GetFullPath(),
            baseSlug,
            dlcSubPath,
            installCheck,
            softwareId,
            executableCheck);

        var fs = new InMemoryFileSystem();
        var result = EADesktopHandler.InstallInfoToGame(registry, fs, installInfo, 0, fs.GetKnownPath(KnownPath.TempDirectory));
        result.IsT0.Should().BeTrue();
        result.IsT1.Should().BeFalse();
    }
}
