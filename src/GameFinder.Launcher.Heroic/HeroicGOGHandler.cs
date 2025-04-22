using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using GameFinder.Common;
using GameFinder.StoreHandlers.GOG;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using OneOf;

namespace GameFinder.Launcher.Heroic;

[PublicAPI]
public class HeroicGOGHandler : AHandler<GOGGame, GOGGameId>
{
    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    public HeroicGOGHandler(IFileSystem fileSystem, ILogger logger)
    {
        _fileSystem = fileSystem;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override Func<GOGGame, GOGGameId> IdSelector { get; } = static game => game.Id;

    /// <inheritdoc/>
    public override IEqualityComparer<GOGGameId>? IdEqualityComparer => null;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<GOGGame, ErrorMessage>> FindAllGames()
    {
        var configDirectory = FindConfigDirectory(_fileSystem)
            .FirstOrDefault(path => path.DirectoryExists());

        if (configDirectory == default)
        {
            yield return new ErrorMessage("Didn't find any heroic files, this can be ignored if heroic isn't installed");
            yield break;
        }

        var installedJsonFile = GetInstalledJsonFilePath(configDirectory);

        if (!installedJsonFile.FileExists)
        {
            yield return new ErrorMessage($"Didn't find the installed.json file in `{configDirectory}`. This can be ignored if you haven't signed into GOG on Heroic yet.");
            yield break;
        }

        var games = ParseInstalledJsonFile(installedJsonFile, configDirectory);
        foreach (var x in games)
        {
            yield return x;
        }
    }

    internal IEnumerable<OneOf<GOGGame, ErrorMessage>> ParseInstalledJsonFile(AbsolutePath path, AbsolutePath configPath)
    {
        using var stream = path.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        var root = JsonSerializer.Deserialize<DTOs.Root>(stream, JsonSerializerOptions);
        if (root is null)
        {
            yield return new ErrorMessage($"Unable to deserialize `{path}`");
            yield break;
        }

        foreach (var installed in root.Installed)
        {
            OneOf<GOGGame, ErrorMessage> res;
            try
            {
                res = Parse(installed, configPath, path.FileSystem);
            }
            catch (Exception e)
            {
                res = new ErrorMessage(e, $"Unable to parse in {path}: `{installed}`");
            }

            yield return res;
        }
    }

    internal OneOf<GOGGame, ErrorMessage> Parse(
        DTOs.Installed installed,
        AbsolutePath configPath,
        IFileSystem fileSystem)
    {
        if (!long.TryParse(installed.AppName, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            return new ErrorMessage($"The value \"appName\" is not a number: \"{installed.AppName}\"");
        }

        var installedPlatform = installed.Platform switch
        {
            "windows" => OSPlatform.Windows,
            "linux" => OSPlatform.Linux,
            _ => OSPlatform.Create(installed.Platform)
        };

        WineData? wineData = null;
        if (installedPlatform == OSPlatform.Windows && OperatingSystem.IsLinux())
        {
            var wineDataRes = GetWineData(installed, configPath, id);
            if (wineDataRes.TryPickT1(out var errorMessage, out wineData))
            {
                _logger.LogWarning("Unable to extract WineData for `{Id}`: {Message}", id, errorMessage.Message);
            }
        }

        var path = fileSystem.FromUnsanitizedFullPath(installed.InstallPath);
        return new HeroicGOGGame(GOGGameId.From(id), installed.AppName, path, installed.BuildId, wineData, installedPlatform);
    }

    [RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(JsonSerializerOptions)")]
    internal OneOf<WineData, ErrorMessage> GetWineData(DTOs.Installed installed, AbsolutePath configPath, long id)
    {
        var gamesConfigFile = GetLinuxGamesConfigJsonFile(configPath, id.ToString(CultureInfo.InvariantCulture));
        if (!gamesConfigFile.FileExists) return new ErrorMessage($"File `{gamesConfigFile}` doesn't exist!");

        using var stream = gamesConfigFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var doc = JsonDocument.Parse(stream, new JsonDocumentOptions
        {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip,
        });

        var element = doc.RootElement.GetProperty(id.ToString(CultureInfo.InvariantCulture));
        var gameConfig = element.Deserialize<DTOs.LinuxGameConfig>();
        if (gameConfig is null) return new ErrorMessage($"Unable to deserialize `{gamesConfigFile}`");

        var winePrefixPath = _fileSystem.FromUnsanitizedFullPath(gameConfig.WinePrefix);

        var environmentOptions = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase);
        if (gameConfig.EnvironmentOptions is not null && gameConfig.EnvironmentOptions.Count > 0)
        {
            foreach (var option in gameConfig.EnvironmentOptions)
            {
                environmentOptions[option.Key] = option.Value;
            }
        }

        return new WineData(winePrefixPath, environmentOptions, gameConfig.WineVersion);
    }

    internal static AbsolutePath GetLinuxGamesConfigJsonFile(AbsolutePath configPath, string name)
    {
        return configPath.Combine("GamesConfig").Combine($"{name}.json");
    }

    internal static AbsolutePath GetInstalledJsonFilePath(AbsolutePath configPath)
    {
        return configPath.Combine("gog_store").Combine("installed.json");
    }

    internal static IEnumerable<AbsolutePath> FindConfigDirectory(IFileSystem fileSystem)
    {
        yield return fileSystem.GetKnownPath(KnownPath.XDG_CONFIG_HOME).Combine("heroic");

        // Flatpak installation
        yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory)
            .Combine(".var")
            .Combine("app")
            .Combine("com.heroicgameslauncher.hgl")
            .Combine("config")
            .Combine("heroic");
    }
}
