using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NexusMods.Paths;
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
            .FirstOrDefault(x => _fileSystem.DirectoryExists(x));

        if (string.IsNullOrEmpty(defaultLocation.Directory))
        {
            yield return PrefixDiscoveryError.From("Unable to find any bottles installation.");
            yield break;
        }

        var bottles = defaultLocation.CombineUnchecked("bottles");
        foreach (var bottle in _fileSystem.EnumerateDirectories(bottles, recursive:false))
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

    internal static bool IsValidBottlesPrefix(IFileSystem fs, AbsolutePath directory,
        [MaybeNullWhen(true)] out PrefixDiscoveryError error)
    {
        if (!DefaultWinePrefixManager.IsValidPrefix(fs, directory, out error))
            return false;

        var bottlesConfigFile = directory.CombineUnchecked("bottle.yml");
        if (!fs.FileExists(bottlesConfigFile))
        {
            error = PrefixDiscoveryError.From($"Bottles configuration file is missing at {bottlesConfigFile}");
            return false;
        }

        error = null;
        return true;
    }

    internal static IEnumerable<AbsolutePath> GetDefaultLocations(IFileSystem fs)
    {
        // $XDG_DATA_HOME/bottles aka ~/.local/share/bottles
        yield return fs.GetKnownPath(KnownPath.LocalApplicationData)
            .CombineUnchecked("bottles");

        // ~/.var/app/com.usebottles.bottles/data/bottles (flatpak installation)
        // https://github.com/flatpak/flatpak/wiki/Filesystem
        yield return fs.GetKnownPath(KnownPath.HomeDirectory)
            .CombineUnchecked(".var/app/com.usebottles.bottles/data/bottles");
    }
}
