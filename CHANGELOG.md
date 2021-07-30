# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][Keep a Changelog] and this project adheres to [Semantic Versioning][Semantic Versioning].

## [Unreleased]

## [Released]

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

[Unreleased]: https://github.com/erri120/GameFinder/compare/v1.6.3...master
[Released]: https://github.com/erri120/GameFinder/releases
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
