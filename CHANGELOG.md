# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][Keep a Changelog] and this project adheres to [Semantic Versioning][Semantic Versioning].

## [Unreleased]

## [Released]

## [1.5.2] - 2021-06-09

### Changed

- Steam, GOG and EGS Handlers will not also include init errors in the final result
- Steam: Added support for new `libraryfolders.vdf` format introduced in update 1623193086 (2021-06-07). See README for more information.

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

[Unreleased]: https://github.com/erri120/GameFinder/compare/v1.5.2...master
[Released]: https://github.com/erri120/GameFinder/releases
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
