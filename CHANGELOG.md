# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog][Keep a Changelog] and this project adheres to [Semantic Versioning][Semantic Versioning].

## [Unreleased]

## 1.4.0

### Changed

- Update Newtonsoft.Json to 13.0.1
- Update Windows SDK to 10.0.19041.0

## [Released]

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

[Unreleased]: https://github.com/erri120/GameFinder/compare/v1.3.1...master
[Released]: https://github.com/erri120/GameFinder/releases
[1.3.1]: https://github.com/erri120/GameFinder/compare/v1.3.0...v1.3.1
[1.3.0]: https://github.com/erri120/GameFinder/compare/v1.2.0...v1.3.0
[1.2.0]: https://github.com/erri120/GameFinder/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/erri120/GameFinder/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/erri120/GameFinder/releases/v1.0.0
