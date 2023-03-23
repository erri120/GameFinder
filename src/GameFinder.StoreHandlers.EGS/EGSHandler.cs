using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.EGS;

/// <summary>
/// Represents a game installed with the Epic Games Store.
/// </summary>
/// <param name="CatalogItemId"></param>
/// <param name="DisplayName"></param>
/// <param name="InstallLocation"></param>
[PublicAPI]
public record EGSGame(string CatalogItemId, string DisplayName, AbsolutePath InstallLocation);

record ManifestFile(string CatalogItemId, string DisplayName, string InstallLocation);

/// <summary>
/// Handler for finding games installed with the Epic Games Store.
/// </summary>
[PublicAPI]
public class EGSHandler : AHandler<EGSGame, string>
{
    internal const string RegKey = @"Software\Epic Games\EOS";
    internal const string ModSdkMetadataDir = "ModSdkMetadataDir";

    private readonly IRegistry _registry;
    private readonly IFileSystem _fileSystem;

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new()
        {
            AllowTrailingCommas = true,
        };

    /// <summary>
    /// Constructor for specifying the implementation of <see cref="IRegistry"/> and
    /// <see cref="IFileSystem"/> when doing tests.
    /// </summary>
    /// <param name="registry"></param>
    /// <param name="fileSystem"></param>
    public EGSHandler(IRegistry registry, IFileSystem fileSystem)
    {
        _registry = registry;
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public override IEnumerable<Result<EGSGame>> FindAllGames()
    {
        var manifestDir = GetManifestDir();
        if (!_fileSystem.DirectoryExists(manifestDir))
        {
            yield return Result.FromError<EGSGame>($"The manifest directory {manifestDir.GetFullPath()} does not exist!");
            yield break;
        }

        var itemFiles = _fileSystem
            .EnumerateFiles(manifestDir, "*.item")
            .ToArray();

        if (itemFiles.Length == 0)
        {
            yield return Result.FromError<EGSGame>($"The manifest directory {manifestDir.GetFullPath()} does not contain any .item files");
            yield break;
        }

        foreach (var itemFile in itemFiles)
        {
            yield return DeserializeGame(itemFile);
        }
    }

    /// <inheritdoc/>
    public override IDictionary<string, EGSGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.CatalogItemId, game => game, StringComparer.OrdinalIgnoreCase);
    }

    private Result<EGSGame> DeserializeGame(AbsolutePath itemFile)
    {
        using var stream = _fileSystem.ReadFile(itemFile);

        try
        {
            var game = JsonSerializer.Deserialize<ManifestFile>(stream, _jsonSerializerOptions);

            if (game is null)
            {
                return Result.FromError<EGSGame>($"Unable to deserialize file {itemFile.GetFullPath()}");
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (game.CatalogItemId is null)
            {
                return Result.FromError<EGSGame>($"Manifest {itemFile.GetFullPath()} does not have a value \"CatalogItemId\"");
            }

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (game.DisplayName is null)
            {
                return Result.FromError<EGSGame>($"Manifest {itemFile.GetFullPath()} does not have a value \"DisplayName\"");
            }

            if (string.IsNullOrEmpty(game.InstallLocation))
            {
                return Result.FromError<EGSGame>($"Manifest {itemFile.GetFullPath()} does not have a value \"InstallLocation\"");
            }

            var res = new EGSGame(game.CatalogItemId, game.DisplayName, _fileSystem.FromFullPath(game.InstallLocation));
            return Result.FromGame(res);
        }
        catch (Exception e)
        {
            return Result.FromError<EGSGame>($"Unable to deserialize file {itemFile.GetFullPath()}:\n{e}");
        }
    }

    private AbsolutePath GetManifestDir()
    {
        return TryGetManifestDirFromRegistry(out var manifestDir)
            ? manifestDir
            : GetDefaultManifestsPath(_fileSystem);
    }

    internal static AbsolutePath GetDefaultManifestsPath(IFileSystem fileSystem)
    {
        return fileSystem
            .GetKnownPath(KnownPath.CommonApplicationDataDirectory)
            .CombineUnchecked("Epic/EpicGamesLauncher/Data/Manifests");
    }

    private bool TryGetManifestDirFromRegistry(out AbsolutePath manifestDir)
    {
        manifestDir = default;

        try
        {
            var currentUser = _registry.OpenBaseKey(RegistryHive.CurrentUser);
            using var regKey = currentUser.OpenSubKey(RegKey);

            if (regKey is null || !regKey.TryGetString("ModSdkMetadataDir",
                    out var registryMetadataDir)) return false;

            manifestDir = _fileSystem.FromFullPath(registryMetadataDir);
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }
}
