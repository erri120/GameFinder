namespace GameFinder.StoreHandlers.EADesktop.Tests;

public partial class EADesktopTests
{
    [Theory]
    [InlineData(null, null, null, null, true)]
    [InlineData("", "", "", "", true)]
    [InlineData(nameof(baseSlug), nameof(installCheck), nameof(baseInstallPath), nameof(softwareID), false)]
    public void Test_InstallInfoToGame(string? baseSlug, string? installCheck, string? baseInstallPath, string? softwareID, bool shouldBeError)
    {
        var installInfo = new InstallInfo
        {
            BaseSlug = baseSlug,
            InstallCheck = installCheck,
            BaseInstallPath = baseInstallPath,
            SoftwareID = softwareID,
        };

        var (game, error) = EADesktopHandler.InstallInfoToGame(installInfo, 0, "");
        if (shouldBeError)
        {
            error.Should().NotBeNull();
            game.Should().BeNull();
        }
        else
        {
            error.Should().BeNull();
            game.Should().NotBeNull();
        }
    }
}
