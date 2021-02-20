# GameFinder

.NET 5 library for finding games on Windows.

## Supported Stores

- Steam
- GOG
- Bethesda.net Launcher
- Epic Games Store
- ~~Origin~~ _TODO_

## Example

```c#
var steamHandler = new SteamHandler();
steamHandler.FindAllGames();
foreach (var steamGame in steamHandler.Games)
{
    Console.WriteLine($"{steamGame} is located at {steamGame.Path}");
}
```

## When to use this Library

This library is best used when you have to find multiple different games from different game stores. If you build a tool for only one game then you might better be off finding it via the registry (common method is looking for the Uninstaller). You could still use this library to only find that one game but it's a bit overkill.

If you build some sort of game library manager like [Playnite](https://github.com/JosefNemec/Playnite/) or [LaunchBox](https://www.launchbox-app.com/) then you could make massive use of this library.

## How it works

### Finding Steam Games

Source: [SteamHandler.cs](GameFinder/StoreHandlers/Steam/SteamHandler.cs)

Steam games can be easily found by searching through _"Steam Universes"_. An Universe is simply a folder where you install Steam games. You can find all Universes by parsing the `config.vdf` file in `Steam/config`. We can get the Steam folder by opening the registry key `HKEY_CURRENT_USER\Software\Valve\Steam` and getting the `SteamPath` value.

The `config.vdf` file uses Valve's KeyValue format which is similar to JSON:

```text
"InstallConfigStore"
{
	"Software"
	{
		"Valve"
		{
			"Steam"
			{
				"BaseInstallFolder_1"		"F:\\SteamLibrary"
				"BaseInstallFolder_3"		"E:\\SteamLibrary"
			}
		}
	}
}
```

What we want to look for are these `BaseInstallFolder_` values which point to a Universe folder. The `steamapps` subdirectory contains the `appmanifest_*.acf` files we need. `.acf` files have the same KeyValue format as `.vdf` files so parsing very easy:

```text
"AppState"
{
	"appid"		"8930"
	"Universe"		"1"
	"LauncherPath"		"C:\\Program Files (x86)\\Steam\\steam.exe"
	"name"		"Sid Meier's Civilization V"
	"StateFlags"		"4"
	"installdir"		"Sid Meier's Civilization V"
	"LastUpdated"		"1600350073"
	"UpdateResult"		"0"
	"SizeOnDisk"		"9235434479"
	"buildid"		"4390913"
	"LastOwner"		"76561198110222274"
	"BytesToDownload"		"20736"
	"BytesDownloaded"		"20736"
	"BytesToStage"		"26039"
	"BytesStaged"		"26039"
}
```

Important is this file are the `appid`, `name` and `installdir` fields. Note: `installdir` is the name of the folder in `Universe/steamapps/common/` where the game is installed. It is not absolute but relative to the `common` folder.

### Finding GOG Games

Source: [GOGHandler.cs](GameFinder/StoreHandlers/GOG/GOGHandler.cs)

GOG stores all information in the registry. This can either be at `HKEY_LOCAL_MACHINE\Software\GOG.com\Games` or `HKEY_LOCAL_MACHINE\Software\WOW6432Node\GOG.com\Games`. Simply open the registry key and get all sub-key names. Each sub-key in `GOG.com\Games` is an installed game with the ID being the name of the sub-key:

![GOG Games Tree in Registry](assets/docs/readme-gog-1.png)

Now you can iterate over all sub-keys to get all the informations you need:

![GOG Game in Registry](assets/docs/readme-gog-2.png)

Important fields are `path`, `gameID` and `gameName`.

### Finding BethNet Games

Source: [BethNetHandler.cs](GameFinder/StoreHandlers/BethNet/BethNetHandler.cs)

Finding games installed with the Bethesda.net Launcher was very rather tricky because there are no config files you can parse or simple registry keys you can open. I ended up using a similar method to the GOG Galaxy Bethesda.net plugin by TouwaStar: [GitHub](https://github.com/TouwaStar/Galaxy_Plugin_Bethesda). The interesting part is the `_scan_games_registry_keys` function in [`betty/local.py`](https://github.com/TouwaStar/Galaxy_Plugin_Bethesda/blob/master/betty/local.py#L154):

1) open the uninstaller registry key at `HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall`
2) iterate over every sub-key:
    - filter out the sub-keys that open the Bethesda Launcher with `bethesdanet://uninstall/`

![Bethesda.net Launcher Games Uninstaller in Registry](assets/docs/readme-bethnet-1.png)

With this you can find all games installed via Bethesda.net. The important fields are `DisplayName`, `ProductID` (64bit value) and `Path`.

### Finding EGS Games

_TODO_
Source: [EGSHandler.cs](GameFinder/StoreHandlers/EGS/EGSHandler.cs)

### Finding Origin Games

_TODO_

## Contributing

See [CONTRIBUTING](CONTRIBUTING.md) for more information.

## License

GPLv3.0, see [LICENSE](LICENSE) for more information.
