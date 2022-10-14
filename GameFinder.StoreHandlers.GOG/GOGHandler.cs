using System.Collections.Generic;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;

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
public static class GOGHandler
{
    private const string GOGRegKey = @"Software\GOG.com\Games";

    /// <summary>
    /// Searches for all games installed with GOG Galaxy. This functions returns either
    /// a non-null <see cref="GOGGame"/> or a non-null error message.
    /// </summary>
    /// <param name="registry">Use either <see cref="WindowsRegistry"/> or <see cref="InMemoryRegistry"/>.</param>
    /// <returns></returns>
    public static IEnumerable<(GOGGame? game, string? error)> FindAllGames(IRegistry registry)
    {
        var localMachine = registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

        using var gogKey = localMachine.OpenSubKey(GOGRegKey);
        if (gogKey is null)
        {
            yield return (null, $"Unable to open HKEY_LOCAL_MACHINE\\{GOGRegKey}");
            yield break;
        }

        var subKeyNames = gogKey.GetSubKeyNames();
        foreach (var subKeyName in subKeyNames)
        {
            var fullKey = $"HKEY_LOCAL_MACHINE\\{GOGRegKey}\\{subKeyName}";
            
            using var subKey = gogKey.OpenSubKey(subKeyName);
            if (subKey is null)
            {
                yield return (null, $"Unable to open {fullKey}");
                continue;
            }

            if (!subKey.TryGetString("gameID", out var sId))
            {
                yield return (null, $"{fullKey} doesn't have a string value \"gameID\"");
                continue;
            }

            if (!long.TryParse(sId, out var id))
            {
                yield return (null, $"The value \"gameID\" of {fullKey} is not a number: \"{sId}\"");
                continue;
            }

            if (!subKey.TryGetString("gameName", out var name))
            {
                yield return (null, $"{fullKey} doesn't have a string value \"gameName\"");
                continue;
            }
            
            if (!subKey.TryGetString("path", out var path))
            {
                yield return (null, $"{fullKey} doesn't have a string value \"path\"");
                continue;
            }

            var game = new GOGGame(id, name, path);
            yield return (game, null);
        }
    }
}
