using GameFinder.StoreHandlers.GOG;
using GameFinder.Wine;
using NexusMods.Paths;

namespace GameFinder.Launcher.Heroic;

public record HeroicGOGGame(
    GOGGameId Id,
    string Name,
    AbsolutePath Path,
    AbsolutePath WinePrefixPath,
    DTOs.WineVersion WineVersion) : GOGGame(Id, Name, Path)
{
    public WinePrefix GetWinePrefix()
    {
        return new WinePrefix
        {
            ConfigurationDirectory = WinePrefixPath.Combine("pfx"),
            UserName = "steamuser",
        };
    }
}
