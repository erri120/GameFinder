# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.org) and this project adheres to [Semantic Versioning](https://semver.org).

## [Unreleased](https://github.com/erri120/GameFinder/compare/v3.0.0...master)

## [Released](https://github.com/erri120/GameFinder/releases)

## [3.0.1](https://github.com/erri120/GameFinder/compare/v3.0.0...v3.0.1) - 2023-05-10

First small release after the major [3.0.0](#300---2023-05-09) release.

**Changes**:

- Added `AHandler`. This is a non-generic abstract base class, inherited by `AHandler<TGame, TId>` and should alleviate some of the issues with generics hell. This class only has one method `IEnumerable<OneOf<IGame, ErrorMessage>> FindAllInterfaceGames()` which makes use of the new `IGame` interface added in [3.0.0](#300---2023-05-09).

## [3.0.0](https://github.com/erri120/GameFinder/compare/v2.6.0...v3.0.0) - 2023-05-09

This is a major release with big changes featuring Wine, Bottles and Proton support for GOG, EGS and Origin. This release also replaces [`System.IO.Abstraction`](https://github.com/TestableIO/System.IO.Abstractions) with `NexusMods.Paths` from the new [Nexus Mods App](https://github.com/Nexus-Mods/NexusMods.App), changes how results are handled and re-adds [Xbox Game Pass](./README.md#xbox-game-pass) support.

**Breaking Changes**:

- Reworked the constructors of all handlers and removed most overloads. All handlers now have a single constructor with no default values.
- Updated `FindAllGames` to return `OneOf<TGame, ErrorMessage>` instead of `Result<TGame>` (using the [OneOf](https://github.com/mcintyre321/OneOf) library).
- Replaced `System.IO.Abstraction` with `NexusMods.Path.IFileSystem`.
  - Paths are now of type `AbsolutePath` instead of `string`.
- Changed `AHandler<TGame, TId>` to require `TId : notnull`.
- Added `IGame` interface and changed `AHandler<TGame, TId>` to require `TGame : IGame`.
- Removed the extension functions `OnlyGame` and `OnlyError`.
- Changed all game Ids to be value objects using [Vogen](https://github.com/SteveDunn/Vogen).
- Changed `FindAllGamesById` to return `IReadOnlyDictionary<TGame, TId>` instead of `IDictionary<TGame, TId>`.

**(Hopefully) non-breaking changes**:

- Re-added Xbox Game Pass.
- Added `Func<TGame, TId> IdSelector` and `IEqualityComparer<TId>? IdEqualityComparer` to `AHandler<TGame, TId>`. These can be used to construct key-value types like a dictionary.
- Added `WindowsRegistry.Shared` for a shared instance of `IRegistry`.
- Wine: added `GetUserName`, `ProtonWinePrefix` will now use `steamuser`.
- Enabled [Trimming](./README.md#trimming).

**How to upgrade**:

The transition from `Result<TGame>` to `OneOf<TGame, ErrorMessage>` should be straight forward. You can use the provided helper methods for [matching](https://github.com/mcintyre321/OneOf#matching) like `result.Match(game => { }, error => { })` or `result.Switch(game => { }, error => { })` depending on your needs. I've also added some extension methods to make the transition easier:

- `bool IsGame()`
- `bool IsError()`
- `TGame AsGame()`
- `ErrorMessage AsError`
- `bool TryGetGame(out TGame)`
- `bool TryGetError(out ErrorMessage)`

Store handlers, like `GOGHandler`, now require an implementation of `NexusMods.Paths.IFileSystem`, instead of `System.IO.Abstraction.IFileSystem`. You can use the shared instance at `NexusMods.Path.FileSystem.Shared`, if you want to use the real file system.

For testing, you can either mock `NexusMods.Paths.IFileSystem`, or use `NexusMods.Paths.InMemoryFileSystem`. If you need to do more in-depth testing, you can also use the `NexusMods.Paths.TestingHelpers` package.

Since `AHandler<TGame, TId>` has changed, you might need to update the constraints for `TId`, if you use generics. Simply add `where TId : notnull` to the constraints. All Ids have been replaced to using value objects. For example: instead of having `long id` for `GOGGame`, it's now `GOGGameId id`. You can still get the value using `id.Value`, which is still a `long`.

**How to use with Wine on Linux**:

See the updated [README](./README.md#wine).

## [2.6.0](https://github.com/erri120/GameFinder/compare/v2.5.0...v2.6.0) - 2023-03-07

- removed support for `net6.0`
- add support for finding Wine prefixes (check the [README](./README.md#wine))
- Steam: added new default installation directories:
    - `~/.steam/debian-installation` (apparently used on Debian/Ubuntu systems)
    - `~/.var/app/com.valvesoftware.Steam/data/Steam` (used by the flatpak installation)
    - `~/.steam/steam` (legacy installation, links to another installation)

## [2.5.0](https://github.com/erri120/GameFinder/compare/v2.4.0...v2.5.0) - 2023-01-17

Added support for EA Desktop. Read the [wiki entry](https://github.com/erri120/GameFinder/wiki/EA-Desktop) before using.

## [2.4.0](https://github.com/erri120/GameFinder/compare/v2.3.0...v2.4.0) - 2023-01-13

Although this release contains some API changes and a lot of internal changes, consumers will likely not be affected.

- Core: `AHandler.FindAllGamesById` now returns `IDictionary<>` instead of `Dictionary<>`
- Core: changed `Result<TGame>` from a record struct to a custom readonly struct
- Steam: added `SteamGame.GetManifestPath` that returns the absolute path to the parsed manifest. This is useful if you need to extract more information from the manifest, after the game has been found.
- Internal: restructured the project and reworked all tests

## [2.3.0](https://github.com/erri120/GameFinder/compare/v2.2.2...v2.3.0) - 2022-12-16

- Steam: added `~/.steam` and `~/.local/.steam` as possible default Steam directories on Linux
- Steam: added constructor that accepts a custom Steam path in case the library can't find it
- upgrade to C# 11
- upgrade `System.IO.Abstractions` to v19.1.1

## [2.2.2](https://github.com/erri120/GameFinder/compare/v2.2.1...v2.2.2) - 2022-11-11

- `FindAllGamesById` will no longer throw an exception for duplicate IDs

## [2.2.1](https://github.com/erri120/GameFinder/compare/v2.2.0...v2.2.1) - 2022-10-21

Small update that changes the equality comparer of the dictionary returned by `EGSHandler.FindAllGamesById` and `OriginHandler.FindAllGamesById` to `StringComparer.OrdinalIgnoreCase`.

## [2.2.0](https://github.com/erri120/GameFinder/compare/v2.1.0...v2.2.0) - 2022-10-21

This update adds some utility functions you can use. I've also created a new package `GameFinder.Common` where all those new utility functions live:

- added `AHandler` an abstract class that all store handlers inherit:
  - `AHandler.FindAllGames`: same function as before
  - `AHandler.FindAllGamesById`: wraps `FindAllGames` and returns a `Dictionary<TId, TGame>`
  - `AHandler.FindOneGameById`: if you just need to find 1 game
- added some extension methods that work with `IEnumerable<Result<TGame>> results`:
  - `IEnumerable<TGame> OnlyGames`: returns only non-null games and discards the rest
  - `IEnumerable<string> OnlyErrors`: returns only non-null errors and discards the rest
  - `(TGame[] games, string[] errors) SplitResults`: splits the results into two arrays

## [2.1.0](https://github.com/erri120/GameFinder/compare/v2.0.1...v2.1.0) - 2022-10-21

Minor update after the major rework:

- everything was made safer and less functions will throw (you should still wrap the `FindAllGames` function call)
- API changed a bit: `FindAllGames` will now return a readonly record struct instead of a named tuple
- lots of internal changes and more tests

## [2.0.1](https://github.com/erri120/GameFinder/compare/v2.0.0...v2.0.1) - 2022-10-19

- Origin: fix parsing manifests with duplicate key-value entries

## [2.0.0](https://github.com/erri120/GameFinder/compare/v1.8.0...v2.0.0) - 2022-10-15

Major rework of all packages:

- removed `netstandard2.1` target
- added `net7.0` target
- added `IRegistry` interface with `WindowsRegistry` and `InMemoryRegistry` implementations
- added runtime platform check in all packages
- deprecated the Bethesda.net and Xbox packages (see [README](./README.md) for more information)
- removed logging from all packages
- removed the `GameFinder` package (it's not needed anymore)
- simplified the public APIs

## [1.8.0](https://github.com/erri120/GameFinder/compare/v1.7.3...v1.8.0) - 2022-06-27

- Fix SteamHandler not finding any Libraries when there are no external libs
- Remove .NET 5 target
- Upgrade dependencies

## [1.7.3](https://github.com/erri120/GameFinder/compare/v1.7.2...v1.7.3) - 2022-05-14

### Changes

- Steam: parsing is now more relaxed, only required values are `appid`, `installdir` and `name`

## [1.7.2](https://github.com/erri120/GameFinder/compare/v1.7.1...v1.7.2) - 2021-11-10

### Changes

- GOG: most stuff is no longer required from the registry, the only values that actually matter are `gameID`, `gameName`, `path`
- Xbox: updated Windows SDK to 10.0.20348.0

## [1.7.1](https://github.com/erri120/GameFinder/compare/v1.7.0...v1.7.1) - 2021-08-19

### Changes

- GOG: `productID` from registry is no longer required

## [1.7.0](https://github.com/erri120/GameFinder/compare/v1.6.4...v1.7.0) - 2021-08-09

### Changes

- Added support for .NET 6

## [1.6.4](https://github.com/erri120/GameFinder/compare/v1.6.3...v1.6.4) - 2021-08-03

### Changes

- Steam: Added support for Unix systems

## [1.6.3](https://github.com/erri120/GameFinder/compare/v1.6.2...v1.6.3) - 2021-07-30

### Changes

- Improved Regex performance by making it `static readonly` and using `RegexOptions.Compiled`
- Steam: Removed parsing the `mounted` value in `libraryfolders.vdf` files

## [1.6.2](https://github.com/erri120/GameFinder/compare/v1.6.1...v1.6.2) - 2021-07-17

### Fixed

- Steam: Fixed Regex for parsing old `libraryfolders.vdf` files
- Origin: Fixed returning invalid Paths
- Origin: Fixed looking for Games with an empty ID
- Origin: Fixed registry lookup with 64bit view

## [1.6.1](https://github.com/erri120/GameFinder/compare/a280ce37eacd33ab7198651e8486fc411c056611...v1.6.1) - 2021-07-10

### Added

- Origin Handler Documentation

### Fixed

- Release Workflow because NuGet doesn't like it when a new project suddenly exists

## [1.6.0](https://github.com/erri120/GameFinder/compare/v1.5.3...a280ce37eacd33ab7198651e8486fc411c056611) - 2021-07-10

This update reverted the change from [v1.5.0](#150---2021-05-29) because it did not perform very well, made the code worse and harder to read and felt like a quick and dirty fix. The reason why I even added `Result<T>` was to let the consumer of this library, which should be an application, know what went wrong by returning something akin to Rust's error propagation. Instead of having an exception whenever something went wrong, it would just add another error as a `string` to a `List<string>` inside the `Result<T>` which the consumer can access and print out. I have removed all of this code and replaced it with simple logging using `ILogger` from `Microsoft.Extensions.Logging`. This way you can simply pass a `ILogger` to the constructor of a Store Handler and it will log to it. Since the consumer of this library is an application which likely has some form of logging going on, this should be very easy to implement.

Additionally I finally went ahead and added Origin Support, see the [README](README.md) for more information.

### Changed

- Using `ILogger` from `Microsoft.Extensions.Logging` instead of custom `Result<T>` to let the user know what went wrong

### Added

- Added Origin Support, see [README](README.md) for more information

## [1.5.3](https://github.com/erri120/GameFinder/compare/v1.5.2...v1.5.3) - 2021-06-13

### Changed

- Steam: Fixed double slashes in Paths ([#8](https://github.com/erri120/GameFinder/issues/8))
- Steam: Improved parsing of `config.vdf`, `libraryfolders.vdf` and `*.acf` files

## [1.5.2](https://github.com/erri120/GameFinder/compare/v1.5.1...v1.5.2) - 2021-06-09

### Changed

- Steam, GOG and EGS Handlers will not also include init errors in the final result
- Steam: Added support for new `libraryfolders.vdf` format introduced in update 1623193086 (2021-06-08). See README for more information.

## [1.5.1](https://github.com/erri120/GameFinder/compare/v1.5.0...v1.5.1) - 2021-06-06

### Changed

- Steam: `libraryfolders.vdf` will now also get parsed ([#7](https://github.com/erri120/GameFinder/pull/7))

## [1.5.0](https://github.com/erri120/GameFinder/compare/v1.4.1...v1.5.0) - 2021-05-29

### Changed

All functions are now no-throw and return `Return<T>` for better error handling and error aggregation. Checkout the example on how to use this: [Link](https://github.com/erri120/GameFinder/blob/20f1cefda485cb7e22fa158cc29ff06fe2b96e21/GameFinder.Example/Program.cs#L43-L57).

## [1.4.1](https://github.com/erri120/GameFinder/compare/v1.4.0...v1.4.1) - 2021-03-26

### Changed

- Include snupkg in output
- Skip broken EGS manifest files

### Fixed

- Fixed `exe` and `exeFile` values not being optional for GOGHandler. DLCs don't have this set.

## [1.4.0](https://github.com/erri120/GameFinder/compare/v1.3.1...v1.4.0) - 2021-03-26

### Changed

- All StoreHandlers are less strict about missing directories
- Update Newtonsoft.Json to 13.0.1
- Update Windows SDK to 10.0.19041.0

## [1.3.1](https://github.com/erri120/GameFinder/compare/v1.3.0...v1.3.1) - 2021-03-14

### Added

- Added helper DTOs to XboxHandler

### Changed

- GameFinder.Example will also log to file
- Replaced Placeholder Exceptions in EGSHandler with EGSExceptions

### Fixed

- Fixed AccessDeniedException in XboxHandler when trying to access Package properties

## [1.3.0](https://github.com/erri120/GameFinder/compare/v.1.2.0...v1.3.0) - 2021-03-13

### Added

- XboxHandler for finding games installed via Xbox Game Pass App (or all UWP apps, see README for more information)
- Example application using all game handlers

## [1.2.0](https://github.com/erri120/GameFinder/compare/v1.1.0...v.1.2.0) - 2021-03-13

### Changed

- Updated project structure: each StoreHandler now has it's own package.

## [1.1.0](https://github.com/erri120/GameFinder/compare/35133cb53afbda1f71581f502506f28e705b9ce3...23687715e5751f5507eac027ba8354f7f95a9019) - 2021-02-21

### Added

- Added support for `netstandard2.1`

## [1.0.0](https://github.com/erri120/GameFinder/compare/09bf20ffd10674012ab61f1a9d83e4fc4a595398...35133cb53afbda1f71581f502506f28e705b9ce3) - 2021-02-21

### Added

- Everything. This is the first release.
