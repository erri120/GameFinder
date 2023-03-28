using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using NexusMods.Paths;
using OneOf;

namespace GameFinder.Wine;

/// <summary>
/// Prefix manager for a vanilla Wine installation that searches for prefixes inside
/// the default locations.
/// </summary>
[PublicAPI]
public class DefaultWinePrefixManager : IWinePrefixManager<WinePrefix>
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fileSystem"></param>
    public DefaultWinePrefixManager(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public IEnumerable<OneOf<WinePrefix, PrefixDiscoveryError>> FindPrefixes()
    {
        foreach (var defaultWinePrefixLocation in GetDefaultWinePrefixLocations(_fileSystem))
        {
            if (!_fileSystem.DirectoryExists(defaultWinePrefixLocation)) continue;

            if (IsValidPrefix(_fileSystem, defaultWinePrefixLocation, out var error))
            {
                yield return new WinePrefix(defaultWinePrefixLocation);
            }
            else
            {
                yield return error;
            }
        }
    }

    internal static bool IsValidPrefix(IFileSystem fileSystem, AbsolutePath directory,
        [MaybeNullWhen(true)] out PrefixDiscoveryError error)
    {
        var virtualDrive = directory.CombineUnchecked("drive_c");
        if (!fileSystem.DirectoryExists(virtualDrive))
        {
            error = PrefixDiscoveryError.From($"Virtual C: drive does not exist at {virtualDrive}");
            return false;
        }

        var systemRegistryFile = directory.CombineUnchecked("system.reg");
        if (!fileSystem.FileExists(systemRegistryFile))
        {
            error = PrefixDiscoveryError.From($"System registry file does not exist at {systemRegistryFile}");
            return false;
        }

        var userRegistryFile = directory.CombineUnchecked("user.reg");
        if (!fileSystem.FileExists(userRegistryFile))
        {
            error = PrefixDiscoveryError.From($"User registry file does not exist at {userRegistryFile}");
            return false;
        }

        error = null;
        return true;
    }

    internal static IEnumerable<AbsolutePath> GetDefaultWinePrefixLocations(IFileSystem fileSystem)
    {
        // from the docs: https://wiki.winehq.org/FAQ#Wineprefixes

        // ~/.wine is the default prefix
        yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory).CombineUnchecked(".wine");

        var winePrefixEnvVariable = Environment.GetEnvironmentVariable("WINEPREFIX");
        if (winePrefixEnvVariable is not null)
        {
            yield return fileSystem.FromFullPath(winePrefixEnvVariable);
        }

        // WINEPREFIX0, WINEPREFIX1, ...
        foreach (var numberedEnvVariable in GetNumberedEnvironmentVariables())
        {
            yield return fileSystem.FromFullPath(numberedEnvVariable);
        }

        // Bottling standards: https://wiki.winehq.org/Bottling_Standards
        // TODO: not sure which 3rd party applications actually use this
    }

    internal static IEnumerable<string> GetNumberedEnvironmentVariables()
    {
        for (var i = 0; i < 10; i++)
        {
            var envVariable = Environment
                .GetEnvironmentVariable($"WINEPREFIX{i.ToString(CultureInfo.InvariantCulture)}");
            if (envVariable is null) yield break;
            yield return envVariable;
        }
    }
}
