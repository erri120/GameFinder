using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory, AutoFileSystem]
    public void Test_InstallInfoToGame(InMemoryFileSystem fileSystem, string baseSlug, string installCheck, string baseInstallPathName, string softwareId)
    {
        var baseInstallPath = fileSystem.GetKnownPath(KnownPath.TempDirectory)
            .Combine(baseInstallPathName);

        var installInfo = new InstallInfo(
            baseInstallPath.GetFullPath(),
            baseSlug,
            installCheck,
            softwareId);

        var fs = new InMemoryFileSystem();
        var result = EADesktopHandler.InstallInfoToGame(fs, installInfo, 0, fs.GetKnownPath(KnownPath.TempDirectory));
        result.IsT0.Should().BeTrue();
        result.IsT1.Should().BeFalse();
    }
}
