using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using OneOf;

namespace GameFinder.Wine.Bottles;

/// <summary>
/// Wineprefix manager for prefixes created and managed by Bottles.
/// </summary>
public class BottlesWinePrefixManager : IWinePrefixManager<BottlesWinePrefix>
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fs"></param>
    public BottlesWinePrefixManager(IFileSystem fs)
    {
        _fileSystem = fs;
    }

    /// <inheritdoc/>
    public IEnumerable<OneOf<BottlesWinePrefix, PrefixDiscoveryError>> FindPrefixes()
    {
        var defaultLocation = GetDefaultLocations(_fileSystem)
            .FirstOrDefault(x => _fileSystem.Directory.Exists(x));

        if (defaultLocation is null)
        {
            yield return PrefixDiscoveryError.From("Unable to find any bottles installation.");
            yield break;
        }

        var bottles = _fileSystem.Path.Combine(defaultLocation, "bottles");
        foreach (var bottle in _fileSystem.Directory.EnumerateDirectories(bottles))
        {
            if (IsValidBottlesPrefix(_fileSystem, bottle, out var error))
            {
                yield return new BottlesWinePrefix(bottle);
            }
            else
            {
                yield return error;
            }
        }
    }

    internal static bool IsValidBottlesPrefix(IFileSystem fs, string directory,
        [MaybeNullWhen(true)] out PrefixDiscoveryError error)
    {
        if (!DefaultWinePrefixManager.IsValidPrefix(fs, directory, out error))
            return false;

        var bottlesConfigFile = fs.Path.Combine(directory, "bottle.yml");
        if (!fs.File.Exists(bottlesConfigFile))
        {
            error = PrefixDiscoveryError.From($"Bottles configuration file is missing at {bottlesConfigFile}");
            return false;
        }

        error = null;
        return true;
    }

    internal static IEnumerable<string> GetDefaultLocations(IFileSystem fs)
    {
        // $XDG_DATA_HOME/bottles aka ~/.local/share/bottles
        yield return fs.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "bottles"
        );


        // ~/.var/app/com.valvesoftware.Steam/data/Steam (flatpak installation)
        // https://github.com/flatpak/flatpak/wiki/Filesystem
        yield return fs.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".var",
            "app",
            ".com.usebottles.bottles",
            "data",
            "bottles"
        );
    }
}
