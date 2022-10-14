using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.Versioning;
using System.Text.Json;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EGS;

[PublicAPI]
public record EGSGame(string CatalogItemId, string DisplayName, string InstallLocation);

[PublicAPI]
public class EGSHandler
{
    private const string RegKey = @"Software\Epic Games\EOS";
    
    private readonly IRegistry _registry;
    private readonly IFileSystem _fileSystem;
    
    [SupportedOSPlatform("windows")]
    public EGSHandler() : this(new WindowsRegistry(), new FileSystem()) { }
    
    public EGSHandler(IRegistry registry) : this(registry, new FileSystem()) { }
    
    public EGSHandler(IRegistry registry, IFileSystem fileSystem)
    {
        _registry = registry;
        _fileSystem = fileSystem;
    }
    
    public IEnumerable<(EGSGame? game, string? error)> FindAllGames()
    {
        var manifestDir = _fileSystem.DirectoryInfo.FromDirectoryName(GetManifestDir());
        if (!manifestDir.Exists)
        {
            yield return (null, $"The manifest directory {manifestDir.FullName} does not exist!");
            yield break;
        }

        var itemFiles = manifestDir.EnumerateFiles("*.item", SearchOption.TopDirectoryOnly);
        foreach (var itemFile in itemFiles)
        {
            using var stream = itemFile.OpenRead();

            var game = JsonSerializer.Deserialize<EGSGame>(stream);
            if (game is null)
            {
                yield return (null, $"Unable to deserialize file {itemFile.FullName}");
            }
            else
            {
                yield return (game, null);
            }
        }
    }

    private string GetManifestDir()
    {
        return TryGetManifestDirFromRegistry(out var manifestDir) 
            ? manifestDir 
            : Path.Combine(
                Environment.ExpandEnvironmentVariables("%PROGRAMDATA%"),
                "Epic",
                "EpicGamesLauncher",
                "Data",
                "Manifests");
    }

    private bool TryGetManifestDirFromRegistry([MaybeNullWhen(false)] out string manifestDir)
    {
        manifestDir = default;
        
        var currentUser = _registry.OpenBaseKey(RegistryHive.CurrentUser);
        using var regKey = currentUser.OpenSubKey(RegKey);

        return regKey is not null && regKey.TryGetString("ModSdkMetadataDir", out manifestDir);
    }
}

