using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

[PublicAPI]
public record EADesktopGame(string SoftwareID, string BaseSlug, string BaseInstallPath, string InstallCheck)
{
    // TODO: add helper method to run install check
    // TODO: add helper method to get installerdata.xml file
}
