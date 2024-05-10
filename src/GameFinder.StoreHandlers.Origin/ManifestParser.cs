using System;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Parsing methods.
/// </summary>
public static class ManifestParser
{
    /// <summary>
    /// Parses the provided Manifest file into a <see cref="OriginGame"/> value.
    /// </summary>
    /// <param name="logger">Used for logging.</param>
    /// <param name="fileSystem">
    /// Used to convert the unsanitized installation path found in the Manifest
    /// to a sanitized <see cref="AbsolutePath"/>.
    /// </param>
    /// <param name="contents">The contents of the Manifest file.</param>
    /// <param name="manifestFile">The path to the Manifest file, used for logging.</param>
    /// <returns>
    /// <see langword="null"/> if the parsing failed, otherwise a valid <see cref="OriginGame"/>.
    /// </returns>
    /// <remarks>
    /// This method can throw exceptions and should be surrounded with a try/catch.
    /// </remarks>
    public static OriginGame? ParseManifestFile(
        ILogger logger,
        IFileSystem fileSystem,
        string contents,
        AbsolutePath manifestFile)
    {
        const string keyId = "id";
        const string keyInstallPath = "dipInstallPath";

        // NOTE(erri120): NameValueCollection is case-insensitive by default,
        // which is exactly what we need to parse Manifest files because the
        // casing is inconsistent.
        var query = HttpUtility.ParseQueryString(contents, Encoding.UTF8);

        // NOTE(erri120): The Manifest can contain duplicate key-value entries,
        // so we have to use GetValues to get all of them.
        var idValues = query.GetValues(keyId);
        if (idValues is null || idValues.Length == 0)
        {
            LogMessages.NoValuesForKey(logger, manifestFile, keyId);
            return null;
        }

        LogMessages.ValuesForKey(logger, manifestFile, keyId, idValues.Length, idValues);

        idValues = idValues
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderByDescending(value => value.Length)
            .ToArray();

        if (idValues.Length == 0)
        {
            LogMessages.NoValuesForKey(logger, manifestFile, keyId);
            return null;
        }

        // NOTE(erri120): Origin will append "@steam" to the ID if the game was
        // bought and installed via Steam. Origin will also launch even if you
        // run the game through Steam. We skip these games since they will be
        // picked up by the Steam StoreHandler instead.
        var steamId = idValues.FirstOrDefault(value => value.EndsWith("@steam", StringComparison.OrdinalIgnoreCase));
        if (steamId is not null)
        {
            LogMessages.SkipSteamGame(logger, manifestFile, steamId);
            return null;
        }

        var id = idValues[0];
        LogMessages.FoundValueForKey(logger, manifestFile, keyId, id);

        var installPathValues = query.GetValues(keyInstallPath);
        if (installPathValues is null || idValues.Length == 0)
        {
            LogMessages.NoValuesForKey(logger, manifestFile, keyInstallPath);
            return null;
        }

        LogMessages.ValuesForKey(logger, manifestFile, keyInstallPath, installPathValues.Length, installPathValues);

        installPathValues = installPathValues
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .OrderByDescending(value => value.Length)
            .ToArray();

        if (installPathValues.Length == 0)
        {
            LogMessages.NoValuesForKey(logger, manifestFile, keyInstallPath);
            return null;
        }

        var installPath = installPathValues[0];
        LogMessages.FoundValueForKey(logger, manifestFile, keyInstallPath, installPath);

        var game = new OriginGame
        {
            Id = OriginGameId.From(id),
            Path = fileSystem.FromUnsanitizedFullPath(installPath),
        };

        return game;
    }
}
