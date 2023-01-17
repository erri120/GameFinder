using System.IO.Abstractions;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Represents a game installed with the EA Desktop app.
/// </summary>
/// <param name="SoftwareID">ID of the game.</param>
/// <param name="BaseSlug">Slug name of the game.</param>
/// <param name="BaseInstallPath">Absolute path to the game folder.</param>
[PublicAPI]
public record EADesktopGame(string SoftwareID, string BaseSlug, string BaseInstallPath)
{
    /// <summary>
    /// Returns the absolute path to the installerdata.xml file inside the __Installer folder
    /// of the game folder.
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <returns></returns>
    public string GetInstallerDataFile(IFileSystem fileSystem)
    {
        return fileSystem.Path.Combine(BaseInstallPath, "__Installer", "installerdata.xml");
    }
}
