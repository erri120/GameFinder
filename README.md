# GameFinder

[![CI](https://github.com/erri120/GameFinder/actions/workflows/ci.yml/badge.svg)](https://github.com/erri120/GameFinder/actions/workflows/ci.yml) [![codecov](https://codecov.io/gh/erri120/GameFinder/branch/master/graph/badge.svg?token=10PVRFWH39)](https://codecov.io/gh/erri120/GameFinder)

.NET library for finding games. The following launchers are supported:

- [Steam](#steam)
- [GOG Galaxy](#gog-galaxy)
- [Epic Games Store](#epic-games-store)
- [Origin](#origin)

The following launchers are not yet supported or support has been dropped:

- [EA Desktop](#ea-desktop)
- [Bethesda.net](#bethesdanet)
- [Xbox Game Pass](#xbox-game-pass)

If you are interested in understanding _how_ GameFinder finds these games, check [the wiki](https://github.com/erri120/GameFinder/wiki) for more information.

## Supported Launchers

### Steam

Steam is supported on Windows and Linux. Visit [SteamDB](https://steamdb.info/) if you need to find the Id of a game.

**Usage:**

```csharp
// use the Windows registry on Windows
// Linux doesn't have a registry
var handler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? new SteamHandler(new WindowsRegistry())
    : new SteamHandler(null);

// method 1: iterate over the game-error result
foreach (var (game, error) in handler.FindAllGames())
{
    if (game is not null)
    {
        Console.WriteLine($"Found {game}");
    }
    else
    {
        Console.WriteLine($"Error: {error}");
    }
}

// method 2: use the dictionary if you need to find games by id
Dictionary<SteamGame, int> games = handler.FindAllGamesById(out string[] errors);

// method 3: find a single game by id
SteamGame? game = handler.FindOneGameById(570940, out string[] errors);
```

### GOG Galaxy

GOG Galaxy is only supported on Windows. Visit [GOG Database](https://www.gogdb.org/) if you need to find the Id of a game.

**Usage:**

```csharp
var handler = new GOGHandler();

// method 1: iterate over the game-error result
foreach (var (game, error) in handler.FindAllGames())
{
    if (game is not null)
    {
        Console.WriteLine($"Found {game}");
    }
    else
    {
        Console.WriteLine($"Error: {error}");
    }
}

// method 2: use the dictionary if you need to find games by id
Dictionary<GOGGame, long> games = handler.FindAllGamesById(out string[] errors);

// method 3: find a single game by id
GOGGame? game = handler.FindOneGameById(1971477531, out string[] errors);
```

### Epic Games Store

Epic Games Store is only supported on Windows.

**Usage:**

```csharp
var handler = new EGSHandler();

// method 1: iterate over the game-error result
foreach (var (game, error) in handler.FindAllGames())
{
    if (game is not null)
    {
        Console.WriteLine($"Found {game}");
    }
    else
    {
        Console.WriteLine($"Error: {error}");
    }
}

// method 2: use the dictionary if you need to find games by id
Dictionary<EGSGame, string> games = handler.FindAllGamesById(out string[] errors);

// method 3: find a single game by id
EGSGame? game = handler.FindOneGameById("3257e06c28764231acd93049f3774ed6", out string[] errors);
```

### Origin

Origin is only supported on Windows. **Note:** [EA is deprecating Origin](https://www.ea.com/en-gb/news/ea-app) and will replace it with [EA Desktop](#ea-desktop).

**Usage:**

```csharp
var handler = new OriginHandler();

// method 1: iterate over the game-error result
foreach (var (game, error) in handler.FindAllGames())
{
    if (game is not null)
    {
        Console.WriteLine($"Found {game}");
    }
    else
    {
        Console.WriteLine($"Error: {error}");
    }
}

// method 2: use the dictionary if you need to find games by id
Dictionary<OriginGame, string> games = handler.FindAllGamesById(out string[] errors);

// method 3: find a single game by id
OriginGame? game = handler.FindOneGameById("Origin.OFR.50.0001456", out string[] errors);
```

### EA Desktop

[EA is deprecating Origin](https://www.ea.com/en-gb/news/ea-app) and replacing it with EA Desktop or EA App or whatever you want to call this new fancy launcher. The "old way" of finding games for Origin does not work for EA Desktop. In fact, there is no clean way of finding any games installed with the new launcher. See [the wiki](https://github.com/erri120/GameFinder/wiki/EA-Desktop) for more information.

## Bethesda.net

[As of May 11, 2022, the Bethesda.net launcher is no longer in use](https://bethesda.net/en/article/2RXxG1y000NWupPalzLblG/sunsetting-the-bethesda-net-launcher-and-migrating-to-steam). The package [GameFinder.StoreHandlers.BethNet](https://www.nuget.org/packages/GameFinder.StoreHandlers.BethNet/) has been deprecated and marked as _legacy_.

## Xbox Game Pass

The package [GameFinder.StoreHandlers.Xbox](https://www.nuget.org/packages/GameFinder.StoreHandlers.Xbox/) has been deprecated and marked as _legacy_. I no longer maintain this package because it never got used. I initially made GameFinder for [Wabbajack](https://github.com/wabbajack-tools/wabbajack) and other modding tools however you can't mod games installed with the Xbox App on Windows. The games are installed as UWP apps which makes them protected and hard to modify. Another issue is the fact that you can't distinguish between normal UWP apps and Xbox games meaning your calculator will show up as an Xbox game.

The final issue is related to actual code: in order to find all UWP apps I used the Windows SDK which was a pain to integrate. The CI had to be on Windows, the .NET target framework had to be `net6.0-windows-XXXXXXXXXX` which a specific SDK version and it was not nice to use.

The package is of course still available on [NuGet](https://www.nuget.org/packages/GameFinder.StoreHandlers.BethNet/) and should still work but it's marked as deprecated so don't expect any updates.

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for more information.

## License

See [LICENSE](LICENSE) for more information.
