using System.Collections.Generic;
using System.Runtime.InteropServices;
using GameFinder.Common;
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
    ulong BuildId,
    WineData? WineData,
    OSPlatform Platform,
    IReadOnlyList<GOGGameId> InstalledDLCs) : IGame
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
public record WineData(AbsolutePath WinePrefixPath, IReadOnlyDictionary<string, string> EnvironmentVariables, DTOs.WineVersion WineVersion);
