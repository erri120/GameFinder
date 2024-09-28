using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using GameFinder.Common;
using GameFinder.StoreHandlers.GOG;
using JetBrains.Annotations;
using NexusMods.Paths;
using OneOf;

namespace GameFinder.Launcher.Heroic;

[PublicAPI]
public class HeroicGOGHandler : AHandler<GOGGame, GOGGameId>
{
    private readonly IFileSystem _fileSystem;

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
    };

    /// <summary>
    /// Constructor.
    /// </summary>
    public HeroicGOGHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public override Func<GOGGame, GOGGameId> IdSelector { get; } = static game => game.Id;

    /// <inheritdoc/>
    public override IEqualityComparer<GOGGameId>? IdEqualityComparer => null;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<GOGGame, ErrorMessage>> FindAllGames()
    {
        var installedJsonFile = FindConfigDirectory(_fileSystem)
            .Select(GetInstalledJsonFilePath)
            .FirstOrDefault(path => path.FileExists);

        if (installedJsonFile == default)
        {
            yield return new ErrorMessage("Didn't find any heroic files, this can be ignored if heroic isn't installed");
            yield break;
        }

        var games = ParseInstalledJsonFile(installedJsonFile);
        foreach (var x in games)
        {
            yield return x;
        }
    }

    internal static IEnumerable<OneOf<GOGGame, ErrorMessage>> ParseInstalledJsonFile(AbsolutePath path)
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
                res = Parse(installed, path.FileSystem);
            }
            catch (Exception e)
            {
                res = new ErrorMessage(e, $"Unable to parse in {path}: `{installed}`");
            }

            yield return res;
        }
    }

    internal static OneOf<GOGGame, ErrorMessage> Parse(DTOs.Installed installed, IFileSystem fileSystem)
    {
        if (!long.TryParse(installed.AppName, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
        {
            return new ErrorMessage($"The value \"appName\" is not a number: \"{installed.AppName}\"");
        }

        var path = fileSystem.FromUnsanitizedFullPath(installed.InstallPath);
        return new GOGGame(GOGGameId.From(id), installed.AppName, path);
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
