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

### Basic Usage

```typescript
import { findAllGames } from 'gamefinder';

// Find all games from all available stores
const { games, errors, skipped } = await findAllGames();

for (const game of games) {
  console.log(`Found: ${game.name} (${game.store}) at ${game.path}`);
}

// Check for any errors
for (const [store, error] of errors) {
  console.error(`${store}: ${error.message}`);
}
```

### With Options

```typescript
import { findAllGames } from 'gamefinder';

// Only search specific stores
const { games } = await findAllGames({ stores: ['steam', 'gog'] });

// Include errors for unavailable stores (instead of skipping them)
const { games, errors } = await findAllGames({ includeUnavailable: true });
```

### Finding a Single Game

```typescript
import { findAllGames } from 'gamefinder';

const { games } = await findAllGames({ stores: ['steam'] });
const skyrim = games.find(game => game.id === '489830');

if (skyrim) {
  console.log(`Found Skyrim SE at ${skyrim.path}`);
}
```

### Using Individual Handlers

For fine-grained control, you can use the store handlers directly:

```typescript
import { SteamHandler } from 'gamefinder';

const handler = new SteamHandler();
const result = await handler.findAllGames();

result.match(
  (games) => console.log(`Found ${games.length} Steam games`),
  (error) => console.error(error.message)
);
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

| Store | Windows | macOS | Linux | ID Resource |
|-------|---------|-------|-------|-------------|
| Steam | ✓ | ✓ | ✓ | [SteamDB](https://steamdb.info/) |
| GOG Galaxy | ✓ | | ✓ | [GOG Database](https://www.gogdb.org/) |
| Epic Games Store | ✓ | ✓ | | |
| Xbox Game Pass | ✓ | | | |

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for more information.

## License

See [LICENSE](LICENSE) for more information.
