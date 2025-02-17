using System.Runtime.InteropServices;
using GameFinder.StoreHandlers.GOG;
using GameFinder.Wine;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.Launcher.Heroic;

/// <summary>
/// Represents a GOG game installed via Heroic.
/// </summary>
[PublicAPI]
public record HeroicGOGGame(
    GOGGameId Id,
    string Name,
    AbsolutePath Path,
    string BuildId,
    WineData? WineData,
    OSPlatform Platform) : GOGGame(Id, Name, Path, BuildId)
{
    /// <summary>
    /// Gets the wine prefix, if any.
    /// </summary>
    public WinePrefix? GetWinePrefix()
    {
        if (WineData is null) return null;

        return new WinePrefix
        {
            ConfigurationDirectory = WineData.WinePrefixPath.Combine("pfx"),
            UserName = "steamuser",
        };
    }
}

[PublicAPI]
public record WineData(AbsolutePath WinePrefixPath, DTOs.WineVersion WineVersion);
