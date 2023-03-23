using System;
using System.Collections.Generic;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.Wine;

/// <summary>
/// Abstract class for wine prefixes.
/// </summary>
[PublicAPI]
public abstract class AWinePrefix
{
    /// <summary>
    /// Absolute path to the Wine prefix directory.
    /// </summary>
    public readonly AbsolutePath ConfigurationDirectory;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configurationDirectory"></param>
    protected AWinePrefix(AbsolutePath configurationDirectory)
    {
        ConfigurationDirectory = configurationDirectory;
    }

    /// <summary>
    /// Returns the absolute path to the virtual drive directory of the prefix.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetVirtualDrivePath()
    {
        return ConfigurationDirectory.CombineUnchecked("drive_c");
    }

    /// <summary>
    /// Returns the absolute path to the <c>system.reg</c> file of the prefix.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetSystemRegistryFile()
    {
        return ConfigurationDirectory.CombineUnchecked("system.reg");
    }

    /// <summary>
    /// Returns the absolute path to the <c>user.reg</c> file of the prefix.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetUserRegistryFile()
    {
        return ConfigurationDirectory.CombineUnchecked("user.reg");
    }

    /// <summary>
    /// Creates an overlay <see cref="IFileSystem"/> with path
    /// mappings into the wine prefix.
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <returns></returns>
    public IFileSystem CreateOverlayFileSystem(IFileSystem fileSystem)
    {
        var rootDirectory = GetVirtualDrivePath();

        var pathMappings = new Dictionary<AbsolutePath, AbsolutePath>
        {
            { fileSystem.FromFullPath("/c"), rootDirectory },
        };

        // TODO: the Wine user can be different, Proton uses "steamuser"
        var newHomeDirectory = rootDirectory
            .CombineUnchecked("Users")
            .CombineUnchecked(Environment.GetEnvironmentVariable("USER")!);

        var knownPaths = Enum.GetValues<KnownPath>();
        foreach (var knownPath in knownPaths)
        {
            var originalPath = fileSystem.GetKnownPath(knownPath);
            var newPath = knownPath switch
            {
                KnownPath.EntryDirectory => originalPath,
                KnownPath.CurrentDirectory => originalPath,

                KnownPath.CommonApplicationDataDirectory => rootDirectory.CombineUnchecked("ProgramData"),

                KnownPath.HomeDirectory => newHomeDirectory,
                KnownPath.MyDocumentsDirectory => newHomeDirectory.CombineUnchecked("Documents"),
                KnownPath.MyGamesDirectory => newHomeDirectory.CombineUnchecked("Documents/My Games"),
                KnownPath.LocalApplicationDataDirectory => newHomeDirectory.CombineUnchecked("AppData/Local"),
                KnownPath.ApplicationDataDirectory => newHomeDirectory.CombineUnchecked("AppData/Roaming"),
                KnownPath.TempDirectory => newHomeDirectory.CombineUnchecked("AppData/Local/Temp"),
            };

            pathMappings[originalPath] = newPath;
        }

        return fileSystem.CreateOverlayFileSystem(pathMappings, true);
    }

    /// <summary>
    /// Creates a new <see cref="IRegistry"/> implementation,
    /// based on the registry files in the configuration
    /// directory.
    /// </summary>
    /// <returns></returns>
    public IRegistry CreateRegistry()
    {
        // TODO: this entire method
        return new InMemoryRegistry();
    }
}
