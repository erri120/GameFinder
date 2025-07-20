using System.Globalization;
using GameFinder.RegistryUtils;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    public static (GOGHandler handler, InMemoryRegistryKey gogKey) SetupHandler(IFileSystem fileSystem, InMemoryRegistry registry)
    {
        var gogKey = registry.AddKey(RegistryHive.LocalMachine, GOGHandler.GOGRegKey);
        var handler = new GOGHandler(registry, fileSystem);
        return (handler, gogKey);
    }

    public static IEnumerable<GOGGame> SetupGames(IFileSystem fileSystem, InMemoryRegistryKey gogKey)
    {
        var fixture = new Fixture();

        fixture.Customize<GOGGame>(composer => composer
            .FromFactory<long, string, ulong, long>((id, name, buildId, parentId) =>
            {
                var path = fileSystem
                    .GetKnownPath(KnownPath.TempDirectory)
                    .Combine(name);

                var gameKey = gogKey.AddSubKey(id.ToString(CultureInfo.InvariantCulture));
                gameKey.AddValue("gameID", id.ToString(CultureInfo.InvariantCulture));
                gameKey.AddValue("gameName", name);
                gameKey.AddValue("path", path.GetFullPath());
                gameKey.AddValue("buildId", $"{buildId}");
                gameKey.AddValue("dependsOn", parentId.ToString(CultureInfo.InvariantCulture));

                return new GOGGame(GOGGameId.From(id), name, path, buildId, GOGGameId.From(parentId));
            })
            .OmitAutoProperties());

        return fixture.CreateMany<GOGGame>();
    }
}
