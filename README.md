# GameFinder

[![CI](https://github.com/erri120/GameFinder/actions/workflows/ci-nodejs.yml/badge.svg)](https://github.com/erri120/GameFinder/actions/workflows/ci-nodejs.yml) [![codecov](https://codecov.io/gh/erri120/GameFinder/branch/master/graph/badge.svg?token=10PVRFWH39)](https://codecov.io/gh/erri120/GameFinder)

TypeScript/JavaScript library for finding games. The following launchers are supported:

- [Steam](#steam) - Windows, macOS, Linux
- [GOG Galaxy](#gog-galaxy) - Windows, Linux
- [Epic Games Store](#epic-games-store) - Windows, macOS
- [Xbox Game Pass](#xbox-game-pass) - Windows only

If you are interested in understanding _how_ GameFinder finds these games, check [the wiki](https://github.com/erri120/GameFinder/wiki) for more information.

## Installation

```bash
npm install gamefinder
# or
pnpm add gamefinder
# or
yarn add gamefinder
```

## Usage

All store handlers implement a `findAllGames()` method which returns a `Result` type from the [neverthrow](https://github.com/supermacro/neverthrow) library. This provides type-safe error handling.

Some **important** things to remember:

- All store handler methods are _pure_, meaning they do not change the internal state of the store handler. This also means that the **results are not cached** and you **shouldn't call the same method multiple times**. It's up to the library consumer to cache the results.
- IDs are **store dependent**. Each store handler has their own type of ID and figuring out the right ID for your game might require some testing.

### Basic Usage

```typescript
import { SteamHandler, GOGHandler, EpicHandler, XboxHandler } from 'gamefinder';

// Create handlers for the stores you need
const steamHandler = new SteamHandler();
const gogHandler = new GOGHandler();
const epicHandler = new EpicHandler();
const xboxHandler = new XboxHandler();

// Find all games from a specific store
const result = await steamHandler.findAllGames();

result.match(
  (games) => {
    for (const game of games) {
      console.log(`Found: ${game.name} at ${game.path}`);
    }
  },
  (error) => {
    console.error(`Error: ${error.message}`);
  }
);
```

### Finding a Single Game

```typescript
import { SteamHandler } from 'gamefinder';

const handler = new SteamHandler();
const result = await handler.findAllGames();

if (result.isOk()) {
  const skyrim = result.value.find(game => game.appId === 489830);
  if (skyrim) {
    console.log(`Found Skyrim SE at ${skyrim.path}`);
  }
}
```

### CLI

GameFinder includes a CLI tool:

```bash
# Run the CLI
npx gamefinder

# Or if installed globally
gamefinder
```

## Supported Launchers

### Steam

Steam is supported on Windows, macOS, and Linux. Use [SteamDB](https://steamdb.info/) to find the App ID of a game.

```typescript
import { SteamHandler } from 'gamefinder';

const handler = new SteamHandler();
const result = await handler.findAllGames();
```

### GOG Galaxy

GOG Galaxy is supported on Windows and Linux. Use the [GOG Database](https://www.gogdb.org/) to find the ID of a game.

```typescript
import { GOGHandler } from 'gamefinder';

const handler = new GOGHandler();
const result = await handler.findAllGames();
```

### Epic Games Store

The Epic Games Store is supported on Windows and macOS.

```typescript
import { EpicHandler } from 'gamefinder';

const handler = new EpicHandler();
const result = await handler.findAllGames();
```

### Xbox Game Pass

Xbox Game Pass is only supported on Windows.

```typescript
import { XboxHandler } from 'gamefinder';

const handler = new XboxHandler();
const result = await handler.findAllGames();
```

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for more information.

## License

See [LICENSE](LICENSE) for more information.
