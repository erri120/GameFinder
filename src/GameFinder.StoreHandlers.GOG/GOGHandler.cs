using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Represents a game installed with GOG Galaxy.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Path"></param>
[PublicAPI]
public record GOGGame(long Id, string Name, AbsolutePath Path);

/// <summary>
/// Handler for finding games installed with GOG Galaxy.
/// </summary>
[PublicAPI]
public class GOGHandler : AHandler<GOGGame, long>
{
    internal const string GOGRegKey = @"Software\GOG.com\Games";

    private readonly IRegistry _registry;
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="registry"></param>
    /// <param name="fileSystem"></param>
    public GOGHandler(IRegistry registry, IFileSystem fileSystem)
    {
        _registry = registry;
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public override IEnumerable<Result<GOGGame>> FindAllGames()
    {
        try
        {
            var localMachine = _registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            using var gogKey = localMachine.OpenSubKey(GOGRegKey);
            if (gogKey is null)
            {
                return new[]
                {
                    Result.FromError<GOGGame>($"Unable to open HKEY_LOCAL_MACHINE\\{GOGRegKey}"),
                };
            }

            var subKeyNames = gogKey.GetSubKeyNames().ToArray();
            if (subKeyNames.Length == 0)
            {
                return new[]
                {
                    Result.FromError<GOGGame>($"Registry key {gogKey.GetName()} has no sub-keys"),
                };
            }

            return subKeyNames
                .Select(subKeyName => ParseSubKey(gogKey, subKeyName))
                .ToArray();
        }
        catch (Exception e)
        {
            return new[] { Result.FromException<GOGGame>("Exception looking for GOG games", e) };
        }
    }

    /// <inheritdoc/>
    public override IDictionary<long, GOGGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.Id, game => game);
    }

    private Result<GOGGame> ParseSubKey(IRegistryKey gogKey, string subKeyName)
    {
        try
        {
            using var subKey = gogKey.OpenSubKey(subKeyName);
            if (subKey is null)
            {
                return Result.FromError<GOGGame>($"Unable to open {gogKey}\\{subKeyName}");
            }

            if (!subKey.TryGetString("gameID", out var sId))
            {
                return Result.FromError<GOGGame>($"{subKey.GetName()} doesn't have a string value \"gameID\"");
            }

            if (!long.TryParse(sId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                return Result.FromError<GOGGame>($"The value \"gameID\" of {subKey.GetName()} is not a number: \"{sId}\"");
            }

            if (!subKey.TryGetString("gameName", out var name))
            {
                return Result.FromError<GOGGame>($"{subKey.GetName()} doesn't have a string value \"gameName\"");
            }

            if (!subKey.TryGetString("path", out var path))
            {
                return Result.FromError<GOGGame>($"{subKey.GetName()} doesn't have a string value \"path\"");
            }

            var game = new GOGGame(id, name, _fileSystem.FromFullPath(path));
            return Result.FromGame(game);
        }
        catch (Exception e)
        {
            return Result.FromException<GOGGame>($"Exception while parsing registry key {gogKey}\\{subKeyName}", e);
        }
    }
}
