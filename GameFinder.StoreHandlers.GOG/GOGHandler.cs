using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Versioning;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Result = GameFinder.Common.Result<GameFinder.StoreHandlers.GOG.GOGGame>;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Represents a game installed with GOG Galaxy.
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Path"></param>
[PublicAPI]
public record GOGGame(long Id, string Name, string Path);

/// <summary>
/// Handler for finding games installed with GOG Galaxy.
/// </summary>
[PublicAPI]
public class GOGHandler : AHandler<GOGGame, long>
{
    internal const string GOGRegKey = @"Software\GOG.com\Games";

    private readonly IRegistry _registry;

    /// <summary>
    /// Default constructor. This uses the <see cref="WindowsRegistry"/> implementation
    /// of <see cref="IRegistry"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public GOGHandler() : this(new WindowsRegistry()) { }

    /// <summary>
    /// Constructor for specifying the implementation of <see cref="IRegistry"/>.
    /// </summary>
    /// <param name="registry"></param>
    public GOGHandler(IRegistry registry)
    {
        _registry = registry;
    }

    /// <inheritdoc/>
    public override IEnumerable<Result> FindAllGames()
    {
        try
        {
            var localMachine =_registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

            using var gogKey = localMachine.OpenSubKey(GOGRegKey);
            if (gogKey is null)
            {
                return new[]{new Result(null, $"Unable to open HKEY_LOCAL_MACHINE\\{GOGRegKey}")};
            }

            var subKeyNames = gogKey.GetSubKeyNames().ToArray();
            if (subKeyNames.Length == 0)
            {
                return new[]{new Result(null, $"Registry key {gogKey.GetName()} has no sub-keys")};
            }

            return subKeyNames
                .Select(subKeyName => ParseSubKey(gogKey, subKeyName))
                .ToArray();
        }
        catch (Exception e)
        {
            return new[] { new Result(null, $"Exception looking for GOG games:\n{e}") };
        }
    }

    /// <inheritdoc/>
    public override IDictionary<long, GOGGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.Id, game => game);
    }

    private static Result ParseSubKey(IRegistryKey gogKey, string subKeyName)
    {
        try
        {
            using var subKey = gogKey.OpenSubKey(subKeyName);
            if (subKey is null)
            {
                return new Result(null,$"Unable to open {gogKey}\\{subKeyName}");
            }

            if (!subKey.TryGetString("gameID", out var sId))
            {
                return new Result(null,$"{subKey.GetName()} doesn't have a string value \"gameID\"");
            }

            if (!long.TryParse(sId, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
            {
                return new Result(null,$"The value \"gameID\" of {subKey.GetName()} is not a number: \"{sId}\"");
            }

            if (!subKey.TryGetString("gameName", out var name))
            {
                return new Result(null, $"{subKey.GetName()} doesn't have a string value \"gameName\"");
            }

            if (!subKey.TryGetString("path", out var path))
            {
                return new Result(null,$"{subKey.GetName()} doesn't have a string value \"path\"");
            }

            var game = new GOGGame(id, name, path);
            return new Result(game, null);
        }
        catch (Exception e)
        {
            return new Result(null,$"Exception while parsing registry key {gogKey}\\{subKeyName}:\n{e}");
        }
    }
}
