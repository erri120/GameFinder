using System.Globalization;
using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam;

/// <summary>
/// Represents a game installed with Steam.
/// </summary>
/// <param name="AppId">ID of the game.</param>
/// <param name="Name">Name of the game.</param>
/// <param name="Path">Absolute path to the game installation directory.</param>
/// <param name="CloudSavesDirectory">Absolute path to the cloud saves directory.</param>
[PublicAPI]
public record SteamGame(SteamGameId AppId, string Name, AbsolutePath Path, AbsolutePath? CloudSavesDirectory) : IGame
{
    /// <summary>
    /// Returns the absolute path of the manifest for this game.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetManifestPath()
    {
        var manifestName = $"{AppId.Value.ToString(CultureInfo.InvariantCulture)}.acf";
        return Path.Parent.Parent.CombineUnchecked(manifestName);
    }

    /// <summary>
    /// Returns the absolute path to the Wine prefix directory, managed by Proton.
    /// </summary>
    /// <returns></returns>
    public ProtonWinePrefix GetProtonPrefix()
    {
        var protonDirectory = Path
            .Parent
            .Parent
            .CombineUnchecked("compatdata")
            .CombineUnchecked(AppId.Value.ToString(CultureInfo.InvariantCulture));

        var configurationDirectory = protonDirectory.CombineUnchecked("pfx");
        return new ProtonWinePrefix
        {
            ConfigurationDirectory = configurationDirectory,
            ProtonDirectory = protonDirectory,
        };
    }
}
