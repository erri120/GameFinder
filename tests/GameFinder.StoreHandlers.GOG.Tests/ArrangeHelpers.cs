using System.Globalization;
using GameFinder.RegistryUtils;

namespace GameFinder.StoreHandlers.GOG.Tests;

public partial class GOGTests
{
    public static (GOGHandler handler, InMemoryRegistryKey gogKey) SetupHandler(InMemoryRegistry registry)
    {
        var gogKey = registry.AddKey(RegistryHive.LocalMachine, GOGHandler.GOGRegKey);
        var handler =  new GOGHandler(registry);
        return (handler, gogKey);
    }

    public static IEnumerable<GOGGame> SetupGames(InMemoryRegistryKey gogKey)
    {
        var fixture = new Fixture();

        fixture.Customize<GOGGame>(composer => composer
            .FromFactory<long, string>((id, name) =>
            {
                var path = Path.Combine(Path.GetTempPath(), name);

                var gameKey = gogKey.AddSubKey(id.ToString(CultureInfo.InvariantCulture));
                gameKey.AddValue("gameID", id.ToString(CultureInfo.InvariantCulture));
                gameKey.AddValue("gameName", name);
                gameKey.AddValue("path", path);

                return new GOGGame(id, name, path);
            })
            .OmitAutoProperties());

        return fixture.CreateMany<GOGGame>();
    }
}
