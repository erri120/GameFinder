using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Handler for finding games installed with GOG Galaxy.
/// </summary>
/// <remarks>
/// This is the base class of <see cref="GOGHandler"/> which is probably
/// what you're looking for instead. This abstract class is only useful if you
/// want to extend the base functionality.
/// </remarks>
/// <seealso cref="GOGHandler"/>
[PublicAPI]
public abstract class GOGHandler<TGame> where TGame : class, IGame
{
    /// <summary>
    /// Registry sub-key of GOG Galaxy.
    /// </summary>
    /// <remarks>
    /// The base key for this is <see cref="RegistryHive.LocalMachine"/>,
    /// also make sure to use the 32-bit registry view.
    /// </remarks>
    public const string GOGRegKey = @"Software\GOG.com\Games";

    /// <summary>
    /// Logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Filesystem.
    /// </summary>
    protected readonly IFileSystem FileSystem;

    /// <summary>
    /// Registry.
    /// </summary>
    protected readonly IRegistry Registry;

    /// <summary>
    /// Constructor.
    /// </summary>
    protected GOGHandler(
        ILoggerFactory loggerFactory,
        IFileSystem fileSystem,
        IRegistry registry)
    {
        Logger = loggerFactory.CreateLogger<GOGHandler<TGame>>();
        FileSystem = fileSystem;
        Registry = registry;
    }

    [Pure]
    public IReadOnlyList<TGame> Search()
    {
        var baseKey = Registry.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

        using var gogKey = baseKey.OpenSubKey(GOGRegKey);
        if (gogKey is null)
        {
            LogMessages.UnableToOpenGOGSubKey(Logger, baseKey.GetName(), GOGRegKey);
            return Array.Empty<TGame>();
        }

        var subKeyNames = gogKey.GetSubKeyNames().ToArray();
        LogMessages.FoundSubKeyNames(Logger, gogKey.GetName(), subKeyNames.Length, subKeyNames);

        var res = new List<TGame>(capacity: subKeyNames.Length);
        foreach (var subKeyName in subKeyNames)
        {
            try
            {
                var game = ParseSubKey(gogKey, subKeyName);
                if (game is null) continue;

                LogMessages.ParsedRegistryKey(Logger, subKeyName, game);
                res.Add(game);
            }
            catch (Exception e)
            {
                LogMessages.ExceptionWhileParsingRegistryKey(Logger, e, subKeyName);
            }
        }

        return res;
    }

    /// <summary>
    /// Parses the given <see cref="IRegistryKey"/> and key name into <typeparamref name="TGame"/>.
    /// </summary>
    protected abstract TGame? ParseSubKey(IRegistryKey gogKey, string subKeyName);
}

/// <summary>
/// Handler for finding games installed with GOG Galaxy.
/// </summary>
[PublicAPI]
public class GOGHandler : GOGHandler<GOGGame>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public GOGHandler(
        ILoggerFactory loggerFactory,
        IFileSystem fileSystem,
        IRegistry registry) : base(loggerFactory, fileSystem, registry) { }

    /// <inheritdoc/>
    protected override GOGGame? ParseSubKey(IRegistryKey gogKey, string subKeyName)
    {
        return RegistryParser.ParseSubKey(Logger, FileSystem, gogKey, subKeyName);
    }
}
