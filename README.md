# GameFinder

[![CI](https://github.com/erri120/GameFinder/actions/workflows/ci.yml/badge.svg)](https://github.com/erri120/GameFinder/actions/workflows/ci.yml) [![codecov](https://codecov.io/gh/erri120/GameFinder/branch/master/graph/badge.svg?token=10PVRFWH39)](https://codecov.io/gh/erri120/GameFinder)

.NET library for finding games. The following launchers are supported:

- [Steam](#steam) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.Steam)](https://www.nuget.org/packages/GameFinder.StoreHandlers.Steam)
- [GOG Galaxy](#gog-galaxy) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.GOG)](https://www.nuget.org/packages/GameFinder.StoreHandlers.GOG)
- [Epic Games Store](#epic-games-store) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.EGS)](https://www.nuget.org/packages/GameFinder.StoreHandlers.EGS)
- [Origin](#origin) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.Origin)](https://www.nuget.org/packages/GameFinder.StoreHandlers.Origin)
- [EA Desktop](#ea-desktop) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.EADesktop)](https://www.nuget.org/packages/GameFinder.StoreHandlers.EADesktop)
- [Xbox Game Pass](#xbox-game-pass) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.Xbox)](https://www.nuget.org/packages/GameFinder.StoreHandlers.Xbox)

The following launchers are not yet supported or support has been dropped:

- [Bethesda.net](#bethesdanet) [![Nuget](https://img.shields.io/nuget/v/GameFinder.StoreHandlers.BethNet?color=red&label=deprecated)](https://www.nuget.org/packages/GameFinder.StoreHandlers.BethNet)

If you are interested in understanding _how_ GameFinder finds these games, check [the wiki](https://github.com/erri120/GameFinder/wiki) for more information.

Additionally, the following Linux tools are supported:

- [Wine](#wine) [![Nuget](https://img.shields.io/nuget/v/GameFinder.Wine)](https://www.nuget.org/packages/GameFinder.Wine)

## Example

The [example project](./other/GameFinder.Example) uses every available store handler and can be used as a reference. You can go to the [GitHub Actions Page](https://github.com/erri120/GameFinder/actions/workflows/ci.yml) and click on one of the latest CI workflow runs to download a build of this project.

## Usage

All store handlers inherit from `AHandler<TGame, TId>` and implement `FindAllGames()` which returns `IEnumerable<OneOf<TGame, ErrorMessage>>`. The [`OneOf`](https://github.com/mcintyre321/OneOf) struct is a F# style union and is guaranteed to only contain _one of_ the following: a `TGame` or an `ErrorMessage`. I recommended checking out the [OneOf library](https://github.com/mcintyre321/OneOf), if you want to learn more.

Some **important** things to remember:

- All store handler methods are _pure_, meaning they do not change the internal state of the store handler because they don't have any. This also means that the **results are not cached** and you **shouldn't call the same method multiple times**. It's up to the library consumer to cache the results somewhere.
- Ids are **store dependent**. Each store handler has their own type of id and figuring out the right id for your game might require some testing. You can find useful resources in this README for some store handlers.

### Basic Usage

```csharp
var results = handler.FindAllGames();

foreach (var result in results)
{
    // using the switch method
    result.Switch(game =>
    {
        Console.WriteLine($"Found {game}");
    }, error =>
    {
        Console.WriteLine(error);
    });

    // using the provided extension functions
    if (result.TryGetGame(out var game))
    {
        Console.WriteLine($"Found {game}");
    } else
    {
        Console.WriteLine(result.AsError());
    }
}
```

### Finding a single game

If you're working on an application that only needs to find **1** game, then you can use the `FindOneGameById` method instead. **IMPORTANT NOTE: the results are not cached**. If you call this method multiple, the store handler will do the same thing multiple times, which is search for every game installed. Do not call this method if you need to [find multiple games](#finding-multiple-games).

```csharp
var game = handler.FindOneGameById(someId, out var errors);

// I highly recommend logging errors regardless of whether or not the game was found.
foreach (var error in errors)
{
    Console.WriteLine(error);
}

if (game is null)
{
    Console.WriteLine("Unable to find game");
} else
{
    Console.WriteLine($"Found {game}");
}
```

### Finding multiple games

If you need to find multiple games at once, you can use the `FindAllGamesById` method instead. This returns an `IReadOnlyDictionary<TId, TGame>` which you can use to lookup games by id. **IMPORTANT NOTE: the results are not cached**. You have to do that yourself.

```csharp
var games = handler.FindAllGamesById(out var errors);

// I highly recommend always logging errors.
foreach (var error in errors)
{
    Console.WriteLine(error);
}

if (games.TryGetValue(someId, out var game))
{
    Console.WriteLine($"Found {game}");
} else
{
    Console.WriteLine($"Unable to find game with the id {someId}");
}
```

## Supported Launchers

### Steam

Steam is supported natively on Windows and Linux. Use [SteamDB](https://steamdb.info/) to find the ID of a game.

**Usage (cross-platform):**

```csharp
var handler = new SteamHandler(FileSystem.Shared, OperatingSystem.IsWindows() ? WindowsRegistry.Shared : null);
```

### GOG Galaxy

GOG Galaxy is supported natively on Windows, and with [Wine](#wine) on Linux. Use the [GOG Database](https://www.gogdb.org/) to find the ID of a game.

**Usage (native on Windows):**

```csharp
var handler = new GOGHandler(WindowsRegistry.Shared, FileSystem.Shared);
```

**Usage (Wine on Linux):**

See [Wine](#wine) for more information.

```csharp
// requires a valid prefix
var wineFileSystem = winePrefix.CreateOverlayFileSystem(FileSystem.Shared);
var wineRegistry = winePrefix.CreateRegistry(FileSystem.Shared);

var handler = new GOGHandler(wineRegistry, wineFileSystem);
```

### Epic Games Store

The Epic Games Store is supported natively on Windows, and with [Wine](#wine) on Linux. Use the [Epic Games Store Database](https://github.com/erri120/egs-db) to find the ID of a game (**WIP**).

**Usage (native on Windows):**

```csharp
var handler = new EGSHandler(WindowsRegistry.Shared, FileSystem.Shared);
```

**Usage (Wine on Linux):**

See [Wine](#wine) for more information.

```csharp
// requires a valid prefix
var wineFileSystem = winePrefix.CreateOverlayFileSystem(FileSystem.Shared);
var wineRegistry = winePrefix.CreateRegistry(FileSystem.Shared);

var handler = new EGSHandler(wineRegistry, wineFileSystem);
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

### EA Desktop

EA Desktop is the replacement for [Origin](#origin): See [EA is deprecating Origin](https://www.ea.com/en-gb/news/ea-app). This is by far, the most complicated Store Handler. **You should read the [wiki entry](https://github.com/erri120/GameFinder/wiki/EA-Desktop).** My implementation decrypts the encrypted file, created by EA Desktop. You should be aware that the key used to encrypt the file is derived from hardware information. If the user changes their hardware, the decryption process might fail because they key has changed.

EA Desktop is only supported on Windows.

**Usage:**

```csharp
var handler = new EADesktopHandler(FileSystem.Shared, new HardwareInfoProvider());
```

### Xbox Game Pass

This package used to be deprecated, but I've re-added support in GameFinder [3.0.0](./CHANGELOG.md#300---2023-05-09). Xbox Game Pass used to install games inside a `SYSTEM` protected folder, making modding not feasible for the average user. You can read more about this [here](https://github.com/Nexus-Mods/NexusMods.App/issues/175).

Xbox Game Pass is only supported on Windows.

**Usage:**

```csharp
var handler = new XboxHandler(FileSystem.Shared);
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

## Trimming

Self-contained deployments and executables can be [trimmed](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained) starting with .NET 6. This feature is _only_ available to applications that are published self-contained.

**Trimmable**:

- `GameFinder.Common`
- `GameFinder.RegistryUtils`
- `GameFinder.Wine`
- `GameFinder.StoreHandlers.Steam`
- `GameFinder.StoreHandlers.GOG`
- `GameFinder.StoreHandlers.EGS`
- `GameFinder.StoreHandlers.Origin`

**NOT Trimmable**:

- `GameFinder.StoreHandlers.EADesktop`: This package references `System.Management`, which is **not trimmable** due to COM interop issues. See [dotnet/runtime#78038](https://github.com/dotnet/runtime/issues/78038), [dotnet/runtime#75176](https://github.com/dotnet/runtime/pull/75176) and [dotnet/runtime#61960](https://github.com/dotnet/runtime/issues/61960) for more details.

The [example](./other/GameFinder.Example) project is being published trimmed in the [CI](./.github/workflows/ci.yml) using this command:

```bash
dotnet publish other/GameFinder.Example/GameFinder.Example.csproj -r linux-x64 -o bin/linux --self-contained -p:PublishReadyToRun=true -p:PublishSingleFile=true -p:PublishTrimmed=true
```

I recommend looking at the [project file](./other/GameFinder.Example/GameFinder.Example.csproj) of the example project, if you run into warnings or errors with trimming.

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for more information.

## License

See [LICENSE](LICENSE) for more information.
