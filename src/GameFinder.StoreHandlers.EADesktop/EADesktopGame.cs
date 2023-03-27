using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Represents a game installed with the EA Desktop app.
/// </summary>
/// <param name="SoftwareID">ID of the game.</param>
/// <param name="BaseSlug">Slug name of the game.</param>
/// <param name="BaseInstallPath">Absolute path to the game folder.</param>
[PublicAPI]
public record EADesktopGame(string SoftwareID, string BaseSlug, AbsolutePath BaseInstallPath)
{
    /// <summary>
    /// Returns the absolute path to the installerdata.xml file inside the __Installer folder
    /// of the game folder.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetInstallerDataFile()
    {
        return BaseInstallPath
            .CombineUnchecked("__Installer")
            .CombineUnchecked("installerdata.xml");
    }
}
