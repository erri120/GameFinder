# GameFinder

[![CI](https://github.com/erri120/GameFinder/actions/workflows/ci.yml/badge.svg)](https://github.com/erri120/GameFinder/actions/workflows/ci.yml) [![codecov](https://codecov.io/gh/erri120/GameFinder/branch/master/graph/badge.svg?token=10PVRFWH39)](https://codecov.io/gh/erri120/GameFinder)

.NET library for finding games. The following launchers are supported:

- [Steam](#steam) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.Steam)](https://www.nuget.org/packages/GameFinder.StoreHandlers.Steam)
- [GOG Galaxy](#gog-galaxy) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.GOG)](https://www.nuget.org/packages/GameFinder.StoreHandlers.GOG)
- [Epic Games Store](#epic-games-store) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.EGS)](https://www.nuget.org/packages/GameFinder.StoreHandlers.EGS)
- [Origin](#origin) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.Origin)](https://www.nuget.org/packages/GameFinder.StoreHandlers.Origin)
- [EA Desktop](#ea-desktop) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.EADesktop)](https://www.nuget.org/packages/GameFinder.StoreHandlers.EADesktop)
- [Xbox Game Pass](#xbox-game-pass) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.Xbox?color=red&label=deprecated)](https://www.nuget.org/packages/GameFinder.StoreHandlers.Xbox)

The following launchers are not yet supported or support has been dropped:

- [Bethesda.net](#bethesdanet) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.BethNet?color=red&label=deprecated)](https://www.nuget.org/packages/GameFinder.StoreHandlers.BethNet)

If you are interested in understanding _how_ GameFinder finds these games, check [the wiki](https://github.com/erri120/GameFinder/wiki) for more information.

Additionally, the following Linux tools are supported:

- [Wine](#wine) [![Nuget](https://img.shields.io/nuget/v/GameFinder.Wine)](https://www.nuget.org/packages/GameFinder.Wine)

## Supported Launchers

### Steam

Steam is supported natively on Windows and Linux. Use [SteamDB](https://steamdb.info/) to find the ID of a game.

**Usage (cross-platform):**

```csharp
var handler = new SteamHandler(FileSystem.Shared, OperatingSystem.IsWindows() ? new WindowsRegistry() : null);
```

**Usage (regardless of platform):**

```csharp
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

GOG Galaxy is supported natively on Windows, and with [Wine](#wine) on Linux. Use the [GOG Database](https://www.gogdb.org/) to find the ID of a game.

**Usage (native on Windows):**

```csharp
var handler = new GOGHandler(new WindowsRegistry(), FileSystem.Shared);
```

**Usage (Wine on Linux):**

See [Wine](#wine) for more information.

```csharp
// requires a valid prefix
var wineFileSystem = winePrefix.CreateOverlayFileSystem(FileSystem.Shared);
var wineRegistry = winePrefix.CreateRegistry(FileSystem.Shared);

var handler = new GOGHandler(wineRegistry, wineFileSystem);
```

**Usage (regardless of platform):**

```csharp
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

The Epic Games Store is supported natively on Windows, and with [Wine](#wine) on Linux.

**Usage (native on Windows):**

```csharp
var handler = new EGSHandler(new WindowsRegistry(), FileSystem.Shared);
```

**Usage (Wine on Linux):**

See [Wine](#wine) for more information.

```csharp
// requires a valid prefix
var wineFileSystem = winePrefix.CreateOverlayFileSystem(FileSystem.Shared);
var wineRegistry = winePrefix.CreateRegistry(FileSystem.Shared);

var handler = new EGSHandler(wineRegistry, wineFileSystem);
```

**Usage (regardless of platform):**

```csharp
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

Origin is supported natively on Windows, and with [Wine](#wine) on Linux. **Note:** [EA is deprecating Origin](https://www.ea.com/en-gb/news/ea-app) and will replace it with [EA Desktop](#ea-desktop).

**Usage (native on Windows):**

```csharp
var handler = new OriginHandler(FileSystem.Shared);
```

**Usage (Wine on Linux):**

See [Wine](#wine) for more information.

```csharp
// requires a valid prefix
var wineFileSystem = winePrefix.CreateOverlayFileSystem(FileSystem.Shared);

var handler = new OriginHandler(wineFileSystem);
```

**Usage (regardless of platform):**

```csharp
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

EA Desktop is the replacement for [Origin](#origin): See [EA is deprecating Origin](https://www.ea.com/en-gb/news/ea-app). This is by far, the most complicated Store Handler. **You should read the [wiki entry](https://github.com/erri120/GameFinder/wiki/EA-Desktop).** My implementation decrypts the encrypted file, created by EA Desktop. You should be aware that the key used to encrypt the file is derived from hardware information. If the user changes their hardware, the decryption process might fail because they key has changed.

EA Desktop is only supported on Windows.

**Usage:**

```csharp
var handler = new EADesktopHandler(FileSystem.Shared, new HardwareInfoProvider());

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
Dictionary<EADesktopGame, string> games = handler.FindAllGamesById(out string[] errors);

// method 3: find a single game by id
EADesktopGame? game = handler.FindOneGameById("Origin.SFT.50.0000532", out string[] errors);
```

### Xbox Game Pass

This package used to be deprecated, but I've re-added support in GameFinder 3.0.0. Xbox Game Pass used to install games inside a `SYSTEM` protected folder, making modding not feasible for the average user. You can read more about this [here](https://github.com/Nexus-Mods/NexusMods.App/issues/175).

Xbox Game Pass is only supported on Windows.

**Usage:**

```csharp
var handler = new XboxHandler(FileSystem.Shared);

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
Dictionary<XboxGame, string> games = handler.FindAllGamesById(out string[] errors);

// method 3: find a single game by id
XboxGame? game = handler.FindOneGameById("Distractionware.DiceyDungeons", out string[] errors);
```

### Bethesda.net

[As of May 11, 2022, the Bethesda.net launcher is no longer in use](https://bethesda.net/en/article/2RXxG1y000NWupPalzLblG/sunsetting-the-bethesda-net-launcher-and-migrating-to-steam). The package [GameFinder.StoreHandlers.BethNet](https://www.nuget.org/packages/GameFinder.StoreHandlers.BethNet/) has been deprecated and marked as _legacy_.


## Wine

[Wine](https://www.winehq.org/) is a compatibility layer capable of running Windows applications on Linux. Wine uses [prefixes](https://wiki.winehq.org/FAQ#Wineprefixes) to create and store virtual `C:` drives. A user can install and run Windows program inside these prefixes, and applications running inside the prefixes likely won't even notice they are not actually running on Windows.

Since GameFinder is all about finding games, it also has to be able to find games inside Wine prefixes to provide good Linux support. The package `NexusMods.Paths` from [NexusMods.App](https://github.com/Nexus-Mods/NexusMods.App) provides a file system abstraction `IFileSystem` which enables path re-mappings:

```csharp
AWinePrefix prefix = //...

// creates a new IFileSystem, with path mappings into the wine prefix
IFileSystem wineFileSystem = prefix.CreateOverlayFileSystem(FileSystem.Shared);

// this wineFileSystem can be used instead of FileSystem.Shared:
var handler = new OriginHandler(wineFileSystem);

// you can also create a new IRegistry:
IRegistry wineRegistry = prefix.CreateRegistry(FileSystem.Shared);

// and use both:
var handler = new EGSHandler(wineRegistry, wineFileSystem);
```

### Default Prefix Manager

`GameFinder.Wine` implements a `IWinePrefixManager` for finding Wine prefixes.

**Usage**:

```csharp
var prefixManager = new DefaultWinePrefixManager(FileSystem.Shared);

foreach (var result in prefixManager.FindPrefixes())
{
    result.Switch(prefix =>
    {
        Console.WriteLine($"Found wine prefix at {prefix.ConfigurationDirectory}");
    }, error =>
    {
        Console.WriteLine(error.Value);
    });
}
```

### Bottles

`GameFinder.Wine` implements a `IWinePrefixManager` for finding Wine prefixes managed by [Bottles](https://usebottles.com/).

**Usage**:

```csharp
var prefixManager = new BottlesWinePrefixManager(FileSystem.Shared);

foreach (var result in prefixManager.FindPrefixes())
{
    result.Switch(prefix =>
    {
        Console.WriteLine($"Found wine prefix at {prefix.ConfigurationDirectory}");
    }, error =>
    {
        Console.WriteLine(error.Value);
    });
}
```

### Proton

Valve's [Proton](https://github.com/ValveSoftware/Proton) is a compatibility tool for Steam and is mostly based on Wine. The Wine prefixes managed by Proton are in the `compatdata` directory of the steam library where the game itself is installed. Since the path is relative to the game itself and requires the app id, I've decided to put this functionality in `GameFinder.StoreHandlers.Steam`:

```csharp
SteamGame? steamGame = steamHandler.FindOneGameById(1237970, out var errors);
if (steamGame is null) return;

ProtonWinePrefix protonPrefix = steamGame.GetProtonPrefix();
var protonPrefixDirectory = protonPrefix.ProtonDirectory;

if (protonDirectory != default && fileSystem.DirectoryExists(protonDirectory))
{
    Console.WriteLine($"Proton prefix is at {protonDirectory}");
}
```

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for more information.

## License

See [LICENSE](LICENSE) for more information.
