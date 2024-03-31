using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Parsing methods.
/// </summary>
[PublicAPI]
public static class RegistryParser
{
    /// <summary>
    /// Parses the given sub key in <see cref="gogKey"/> into <see cref="GOGGame"/>.
    /// </summary>
    /// <param name="logger">Used for logging.</param>
    /// <param name="fileSystem">
    /// Used to convert the unsanitized installation path found in the Registry
    /// to a sanitized <see cref="AbsolutePath"/>.
    /// </param>
    /// <param name="gogKey">The main GOG registry key.</param>
    /// <param name="subKeyName">The name of the sub key.</param>
    /// <returns>
    /// <see langword="null"/> if the parsing failed, otherwise a valid <see cref="GOGGame"/>.
    /// </returns>
    /// <remarks>
    /// This method can throw exceptions and should be surrounded with a try/catch.
    /// </remarks>
    public static GOGGame? ParseSubKey(
        ILogger logger,
        IFileSystem fileSystem,
        IRegistryKey gogKey,
        string subKeyName)
    {
        using var subKey = gogKey.OpenSubKey(subKeyName);
        if (subKey is null)
        {
            LogMessages.UnableToOpenSubKey(logger, gogKey.GetName(), subKeyName);
            return null;
        }

        if (!TryGetId(logger, subKey, out var id)) return null;
        if (!TryGetGameName(logger, subKey, out var gameName)) return null;
        if (!TryGetPath(logger, fileSystem, subKey, out var path)) return null;

        var game = new GOGGame(id, gameName, path);
        return game;
    }

    /// <summary>
    /// Tries to find a valid path in <paramref name="subKey"/>.
    /// </summary>
    public static bool TryGetPath(
        ILogger logger,
        IFileSystem fileSystem,
        IRegistryKey subKey,
        out AbsolutePath path)
    {
        const string valuePath = "path";
        const string valueExe = "exe";
        const string valueWorkingDir = "workingDir";

        if (TryGetString(logger, subKey, valuePath, out var sPath))
        {
            path = fileSystem.FromUnsanitizedFullPath(sPath);
            return true;
        }

        if (TryGetString(logger, subKey, valueExe, out var sExe))
        {
            var exePath = fileSystem.FromUnsanitizedFullPath(sExe);
            path = exePath.Parent;
            return true;
        }

        if (TryGetString(logger, subKey, valueWorkingDir, out var sWorkingDir))
        {
            path = fileSystem.FromUnsanitizedFullPath(sWorkingDir);
            return true;
        }

        LogMessages.NoPath(logger, subKey.GetName());
        path = default;
        return false;
    }

    /// <summary>
    /// Tries to find a valid game name in <paramref name="subKey"/>.
    /// </summary>
    public static bool TryGetGameName(
        ILogger logger,
        IRegistryKey subKey,
        [NotNullWhen(true)] out string? gameName)
    {
        const string valueGameName = "gameName";

        if (!TryGetString(logger, subKey, valueGameName, out gameName))
        {
            LogMessages.NoGameName(logger, subKey.GetName());
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tries to find a valid ID in <paramref name="subKey"/>.
    /// </summary>
    public static bool TryGetId(
        ILogger logger,
        IRegistryKey subKey,
        out GOGGameId id)
    {
        const string valueGameID = "gameID";
        const string valueProductID = "productID";

        id = default;
        if (TryParse<ulong>(logger, subKey, valueGameID, out var gameId))
        {
            id = GOGGameId.From(gameId);
            return true;
        }

        if (TryParse<ulong>(logger, subKey, valueProductID, out var productId))
        {
            id = GOGGameId.From(productId);
            return true;
        }

        LogMessages.NoId(logger, subKey.GetName());
        return false;
    }

    /// <summary>
    /// Tries to get a string from the given registry key and parses it into
    /// the given integer type.
    /// </summary>
    public static bool TryParse<T>(
        ILogger logger,
        IRegistryKey subKey,
        string valueName,
        out T value) where T : struct, INumberBase<T>
    {
        value = default;
        if (!TryGetString(logger, subKey, valueName, out var sValue)) return false;
        if (T.TryParse(sValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) return true;

        LogMessages.FailedToParse(logger, subKey.GetName(), sValue, valueName, typeof(T).Name);
        return false;
    }

    /// <summary>
    /// Tries to get a string from the given registry key.
    /// </summary>
    public static bool TryGetString(
        ILogger logger,
        IRegistryKey subKey,
        string valueName,
        [NotNullWhen(true)] out string? value)
    {
        if (!subKey.TryGetString(valueName, out value))
        {
            LogMessages.NoValueForKey(logger, valueName, subKey.GetName());
            return false;
        }

        LogMessages.ValueForKey(logger, valueName, subKey.GetName(), value);
        return true;
    }
}
