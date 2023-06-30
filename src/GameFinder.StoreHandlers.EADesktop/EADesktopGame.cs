using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Represents a game installed with the EA Desktop app.
/// </summary>
/// <param name="EADesktopGameId">Id of the game.</param>
/// <param name="BaseSlug">Slug name of the game.</param>
/// <param name="BaseInstallPath">Absolute path to the game folder.</param>
[PublicAPI]
public record EADesktopGame(EADesktopGameId EADesktopGameId, string BaseSlug, AbsolutePath BaseInstallPath) : IGame
{
    /// <summary>
    /// Returns the absolute path to the <c>installerdata.xml</c> file inside the <c>__Installer</c> folder
    /// of the game folder.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetInstallerDataFile()
    {
        return BaseInstallPath
            .Combine("__Installer")
            .Combine("installerdata.xml");
    }
}
