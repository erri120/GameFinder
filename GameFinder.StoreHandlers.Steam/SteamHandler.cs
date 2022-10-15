using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using ValveKeyValue;

namespace GameFinder.StoreHandlers.Steam;

public record SteamGame(int AppId, string Name, string Path);

[PublicAPI]
public class SteamHandler
{
    private readonly IRegistry? _registry;
    private readonly IFileSystem _fileSystem;
    
    public SteamHandler(IFileSystem fileSystem, IRegistry? registry)
    {
        _fileSystem = fileSystem;
        
        if (registry is null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _registry = new WindowsRegistry();
        }
        else
        {
            _registry = registry;
        }
    }

    public IEnumerable<(SteamGame? game, string? error)> FindAllGames()
    {
        var steamDir = FindSteam();
        if (steamDir is null)
        {
            yield return (null, "Unable to find Steam!");
            yield break;
        }

        var libraryFoldersFile = GetLibraryFoldersFile(steamDir);
        if (!libraryFoldersFile.Exists)
        {
            yield return (null, $"File {libraryFoldersFile.FullName} does not exist!");
            yield break;
        }

        var libraryFolderPaths = ParseLibraryFoldersFile(libraryFoldersFile);
        if (libraryFolderPaths is null)
        {
            yield return (null, $"Found no Steam Libraries in {libraryFoldersFile.FullName}");
            yield break;
        }

        foreach (var libraryFolderPath in libraryFolderPaths)
        {
            var libraryFolder = _fileSystem.DirectoryInfo.FromDirectoryName(libraryFolderPath);
            if (!libraryFolder.Exists)
            {
                yield return (null, $"Steam Library {libraryFolder.FullName} does not exist!");
                continue;
            }

            var acfFiles = libraryFolder.EnumerateFiles("*.acf", SearchOption.TopDirectoryOnly);
            foreach (var acfFile in acfFiles)
            {
                yield return ParseAppManifestFile(acfFile, libraryFolder);
            }
        }
    }

    private IDirectoryInfo? FindSteam()
    {
        var defaultSteamDir = FindSteamInDefaultPath();
        if (defaultSteamDir is not null) return defaultSteamDir;
        
        if (_registry is not null && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var steamDir = FindSteamInRegistry(_registry);
            return steamDir;
        }

        return null;
    }
    
    private IDirectoryInfo? FindSteamInRegistry(IRegistry registry)
    {
        var currentUser = registry.OpenBaseKey(RegistryHive.CurrentUser);

        using var regKey = currentUser.OpenSubKey(@"SOFTWARE\Valve\Steam");
        if (regKey is null) return null;
        
        if (!regKey.TryGetString("SteamPath", out var steamPath)) return null;
        
        var directoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(steamPath);
        return IsValidSteamDirectory(directoryInfo) ? directoryInfo : null;
    }

    private IDirectoryInfo? FindSteamInDefaultPath()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var directoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_fileSystem.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                "Steam"
            ));
            
            return IsValidSteamDirectory(directoryInfo) ? directoryInfo : null; 
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // steam on linux can be found in ~/.local/share/Steam
            // https://github.com/dotnet/runtime/blob/3b1df9396e2a7cc6797e76793e8547f8a7771953/src/libraries/System.Private.CoreLib/src/System/Environment.GetFolderPathCore.Unix.cs#L124
            var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            var directoryInfo = _fileSystem.DirectoryInfo.FromDirectoryName(_fileSystem.Path.Combine(
                path,
                "Steam"
            ));
            
            return IsValidSteamDirectory(directoryInfo) ? directoryInfo : null;
        }

        return null;
    }
    
    private IFileInfo GetLibraryFoldersFile(IDirectoryInfo steamDirectory)
    {
        var fileInfo =_fileSystem.FileInfo.FromFileName(_fileSystem.Path.Combine(
            steamDirectory.FullName,
            "steamapps",
            "libraryfolders.vdf"));

        return fileInfo;
    }
    
    private bool IsValidSteamDirectory(IDirectoryInfo? directoryInfo)
    {
        if (directoryInfo is null) return false;
        var fileInfo = GetLibraryFoldersFile(directoryInfo);
        return fileInfo.Exists;
    }

    private List<string>? ParseLibraryFoldersFile(IFileInfo fileInfo)
    {
        using var stream = fileInfo.OpenRead();

        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        var data = kv.Deserialize(stream, new KVSerializerOptions
        {
            HasEscapeSequences = true,
            EnableValveNullByteBugBehavior = true
        });

        if (data is null) return null;
        if (!data.Name.Equals("libraryfolders", StringComparison.OrdinalIgnoreCase)) return null;

        var paths = data.Children
            .Where(child => int.TryParse(child.Name, out _))
            .Select(child => child["path"])
            .Where(pathValue => pathValue is not null && pathValue.ValueType == KVValueType.String)
            .Select(pathValue => pathValue.ToString(CultureInfo.InvariantCulture))
            .Select(path => _fileSystem.Path.Combine(path, "steamapps"))
            .ToList();
        
        return paths.Any() ? paths : null;
    }

    private (SteamGame? game, string? error) ParseAppManifestFile(IFileInfo manifestFile, IDirectoryInfo libraryFolder)
    {
        using var stream = manifestFile.OpenRead();
        
        var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
        var data = kv.Deserialize(stream, new KVSerializerOptions
        {
            HasEscapeSequences = true,
            EnableValveNullByteBugBehavior = true
        });

        if (data is null)
        {
            return (null, $"Unable to parse {manifestFile.FullName}");
        }

        if (!data.Name.Equals("AppState", StringComparison.OrdinalIgnoreCase))
        {
            return (null, $"Manifest {manifestFile.FullName} is not a valid format!");
        }

        var appIdValue = data["appid"];
        if (appIdValue is null)
        {
            return (null, $"Manifest {manifestFile.FullName} does not have the value \"appid\"");
        }
        
        var nameValue = data["name"];
        if (nameValue is null)
        {
            return (null, $"Manifest {manifestFile.FullName} does not have the value \"name\"");
        }
        
        var installDirValue = data["installdir"];
        if (installDirValue is null)
        {
            return (null, $"Manifest {manifestFile.FullName} does not have the value \"installdir\"");
        }
        
        var appId = appIdValue.ToInt32(NumberFormatInfo.InvariantInfo);
        var name = nameValue.ToString(CultureInfo.InvariantCulture);
        var installDir = installDirValue.ToString(CultureInfo.InvariantCulture);
        
        var gamePath = _fileSystem.Path.Combine(
            libraryFolder.FullName,
            "common",
            installDir
        );

        var game = new SteamGame(appId, name, gamePath);
        return (game, null);
    }
}
