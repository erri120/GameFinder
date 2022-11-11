# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][Keep a Changelog] and this project adheres to [Semantic Versioning][Semantic Versioning].

## [Unreleased]

## [Released]

## [2.2.2] - 2022-11-11

- `FindAllGamesById` will no longer throw an exception for duplicate IDs

## [2.2.1] - 2022-10-21

Small update that changes the equality comparer of the dictionary returned by `EGSHandler.FindAllGamesById` and `OriginHandler.FindAllGamesById` to `StringComparer.OrdinalIgnoreCase`.

## [2.2.0] - 2022-10-21

This update adds some utility functions you can use. I've also created a new package `GameFinder.Common` where all those new utility functions live:

- added `AHandler` an abstract class that all store handlers inherit:
  - `AHandler.FindAllGames`: same function as before
  - `AHandler.FindAllGamesById`: wraps `FindAllGames` and returns a `Dictionary<TId, TGame>`
  - `AHandler.FindOneGameById`: if you just need to find 1 game
- added some extension methods that work with `IEnumerable<Result<TGame>> results`:
  - `IEnumerable<TGame> OnlyGames`: returns only non-null games and discards the rest
  - `IEnumerable<string> OnlyErrors`: returns only non-null errors and discards the rest
  - `(TGame[] games, string[] errors) SplitResults`: splits the results into two arrays

## [2.1.0] - 2022-10-21

Minor update after the major rework:

- everything was made safer and less functions will throw (you should still wrap the `FindAllGames` function call)
- API changed a bit: `FindAllGames` will now return a readonly record struct instead of a named tuple
- lots of internal changes and more tests

## [2.0.1] - 2022-10-19

- Origin: fix parsing manifests with duplicate key-value entries

## [2.0.0] - 2022-10-15

Major rework of all packages:

- removed `netstandard2.1` target
- added `net7.0` target
- added `IRegistry` interface with `WindowsRegistry` and `InMemoryRegistry` implementations
- added runtime platform check in all packages
- deprecated the Bethesda.net and Xbox packages (see [README](./README.md) for more information)
- removed logging from all packages
- removed the `GameFinder` package (it's not needed anymore)
- simplified the public APIs

## [1.8.0] - 2022-06-27

- Fix SteamHandler not finding any Libraries when there are no external libs
- Remove .NET 5 target
- Upgrade dependencies

## [1.7.3] - 2022-05-14

### Changes

- Steam: parsing is now more relaxed, only required values are `appid`, `installdir` and `name`

## [1.7.2] - 2021-11-10

### Changes

- GOG: most stuff is no longer required from the registry, the only values that actually matter are `gameID`, `gameName`, `path`
- Xbox: updated Windows SDK to 10.0.20348.0

## [1.7.1] - 2021-08-19

### Changes

- GOG: `productID` from registry is no longer required

## [1.7.0] - 2021-08-09

### Changes

- Added support for .NET 6

## [1.6.4] - 2021-08-03

### Changes

- Steam: Added support for Unix systems

## [1.6.3] - 2021-07-30

### Changes

- Improved Regex performance by making it `static readonly` and using `RegexOptions.Compiled`
- Steam: Removed parsing the `mounted` value in `libraryfolders.vdf` files

## [1.6.2] - 2021-07-17

### Fixed

- Steam: Fixed Regex for parsing old `libraryfolders.vdf` files
- Origin: Fixed returning invalid Paths
- Origin: Fixed looking for Games with an empty ID
- Origin: Fixed registry lookup with 64bit view

## [1.6.1] - 2021-07-10

### Added

- Origin Handler Documentation

### Fixed

- Release Workflow because NuGet doesn't like it when a new project suddenly exists

## [1.6.0] - 2021-07-10

This update reverted the change from [v1.5.0](#150---2021-05-29) because it did not perform very well, made the code worse and harder to read and felt like a quick and dirty fix. The reason why I even added `Result<T>` was to let the consumer of this library, which should be an application, know what went wrong by returning something akin to Rust's error propagation. Instead of having an exception whenever something went wrong, it would just add another error as a `string` to a `List<string>` inside the `Result<T>` which the consumer can access and print out. I have removed all of this code and replaced it with simple logging using `ILogger` from `Microsoft.Extensions.Logging`. This way you can simply pass a `ILogger` to the constructor of a Store Handler and it will log to it. Since the consumer of this library is an application which likely has some form of logging going on, this should be very easy to implement.

Additionally I finally went ahead and added Origin Support, see the [README](README.md) for more information.

### Changed

- Using `ILogger` from `Microsoft.Extensions.Logging` instead of custom `Result<T>` to let the user know what went wrong

### Added

- Added Origin Support, see [README](README.md) for more information

## [1.5.3] - 2021-06-13

### Changed

- Steam: Fixed double slashes in Paths ([#8](https://github.com/erri120/GameFinder/issues/8))
- Steam: Improved parsing of `config.vdf`, `libraryfolders.vdf` and `*.acf` files

## [1.5.2] - 2021-06-09

### Changed

- Steam, GOG and EGS Handlers will not also include init errors in the final result
- Steam: Added support for new `libraryfolders.vdf` format introduced in update 1623193086 (2021-06-08). See README for more information.

## [1.5.1] - 2021-06-06

### Changed

- Steam: `libraryfolders.vdf` will now also get parsed ([#7](https://github.com/erri120/GameFinder/pull/7))

## [1.5.0] - 2021-05-29

### Changed

All functions are now no-throw and return `Return<T>` for better error handling and error aggregation. Checkout the example on how to use this: [Link](https://github.com/erri120/GameFinder/blob/20f1cefda485cb7e22fa158cc29ff06fe2b96e21/GameFinder.Example/Program.cs#L43-L57).

## [1.4.1] - 2021-03-26

### Changed

- Include snupkg in output
- Skip broken EGS manifest files

### Fixed

- Fixed `exe` and `exeFile` values not being optional for GOGHandler. DLCs don't have this set.

## [1.4.0] - 2021-03-26

### Changed

- All StoreHandlers are less strict about missing directories
- Update Newtonsoft.Json to 13.0.1
- Update Windows SDK to 10.0.19041.0

## [1.3.1] - 2021-03-14

### Added

- Added helper DTOs to XboxHandler

### Changed

- GameFinder.Example will also log to file
- Replaced Placeholder Exceptions in EGSHandler with EGSExceptions

### Fixed

- Fixed AccessDeniedException in XboxHandler when trying to access Package properties

## [1.3.0] - 2021-03-13

### Added

- XboxHandler for finding games installed via Xbox Game Pass App (or all UWP apps, see README for more information)
- Example application using all game handlers

## [1.2.0] - 2021-03-13

### Changed

- Updated project structure: each StoreHandler now has it's own package.

## [1.1.0] - 2021-02-21

### Added

- Added support for `netstandard2.1`

## [1.0.0] - 2021-02-21

### Added

- Everything. This is the first release.

<!-- Links -->
[Keep a Changelog]: https://keepachangelog.com/
[Semantic Versioning]: https://semver.org/

[Unreleased]: https://github.com/erri120/GameFinder/compare/v2.2.0...master
[Released]: https://github.com/erri120/GameFinder/releases
[2.2.0]: https://github.com/erri120/GameFinder/compare/v2.1.0...v2.2.0
[2.1.0]: https://github.com/erri120/GameFinder/compare/v2.0.1...v2.1.0
[2.0.1]: https://github.com/erri120/GameFinder/compare/v2.0.0...v2.0.1
[2.0.0]: https://github.com/erri120/GameFinder/compare/v1.8.0...v2.0.0
[1.8.0]: https://github.com/erri120/GameFinder/compare/v1.7.3...v1.8.0
[1.7.3]: https://github.com/erri120/GameFinder/compare/v1.7.2...v1.7.3
[1.7.2]: https://github.com/erri120/GameFinder/compare/v1.7.1...v1.7.2
[1.7.1]: https://github.com/erri120/GameFinder/compare/v1.7.0...v1.7.1
[1.7.0]: https://github.com/erri120/GameFinder/compare/v1.6.4...v1.7.0
[1.6.4]: https://github.com/erri120/GameFinder/compare/v1.6.3...v1.6.4
[1.6.3]: https://github.com/erri120/GameFinder/compare/v1.6.2...v1.6.3
[1.6.2]: https://github.com/erri120/GameFinder/compare/v1.6.1...v1.6.2
[1.6.1]: https://github.com/erri120/GameFinder/compare/v1.6.0...v1.6.1
[1.6.0]: https://github.com/erri120/GameFinder/compare/v1.5.3...v1.6.0
[1.5.3]: https://github.com/erri120/GameFinder/compare/v1.5.2...v1.5.3
[1.5.2]: https://github.com/erri120/GameFinder/compare/v1.5.1...v1.5.2
[1.5.1]: https://github.com/erri120/GameFinder/compare/v1.5.0...v1.5.1
[1.5.0]: https://github.com/erri120/GameFinder/compare/v1.4.1...v1.5.0
[1.4.1]: https://github.com/erri120/GameFinder/compare/v1.4.0...v1.4.1
[1.4.0]: https://github.com/erri120/GameFinder/compare/v1.3.1...v1.4.0
[1.3.1]: https://github.com/erri120/GameFinder/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/erri120/GameFinder/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/erri120/GameFinder/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/erri120/GameFinder/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/erri120/GameFinder/releases/v1.0.0
