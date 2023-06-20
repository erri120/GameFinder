using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using FluentResults;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using GameFinder.StoreHandlers.Steam.Services;
using JetBrains.Annotations;
using NexusMods.Paths;
using OneOf;
using ValveKeyValue;

namespace GameFinder.StoreHandlers.Steam;

/// <summary>
/// Handler for finding games installed with Steam.
/// </summary>
[PublicAPI]
public class SteamHandler : AHandler<SteamGame, AppId>
{
    private readonly IRegistry? _registry;
    private readonly IFileSystem _fileSystem;

    private static readonly KVSerializerOptions KvSerializerOptions =
        new()
        {
            HasEscapeSequences = true,
            EnableValveNullByteBugBehavior = true,
        };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fileSystem">
    /// The implementation of <see cref="IFileSystem"/> to use. For a shared instance use
    /// <see cref="FileSystem.Shared"/>. For tests either use <see cref="InMemoryFileSystem"/>,
    /// a custom implementation or just a mock of the interface.
    /// </param>
    /// <param name="registry">
    /// The implementation of <see cref="IRegistry"/> to use. For a shared instance
    /// use <see cref="WindowsRegistry.Shared"/> on Windows. On Linux use <c>null</c>.
    /// For tests either use <see cref="InMemoryRegistry"/>, a custom implementation or just a mock
    /// of the interface.
    /// </param>
    public SteamHandler(IFileSystem fileSystem, IRegistry? registry)
    {
        _fileSystem = fileSystem;
        _registry = registry;
    }

    /// <inheritdoc/>
    public override Func<SteamGame, AppId> IdSelector => game => game.AppId;

    /// <inheritdoc/>
    public override IEqualityComparer<AppId>? IdEqualityComparer => null;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<SteamGame, ErrorMessage>> FindAllGames()
    {
        var os = GetOS();
        var steamPathResult = SteamLocationFinder.FindSteam(_fileSystem, _registry, os);
        if (steamPathResult.IsFailed)
        {
            yield return ConvertResultToErrorMessage(steamPathResult);
            yield break;
        }

        var steamPath = steamPathResult.Value;
        var libraryFoldersFilePath = SteamLocationFinder.GetLibraryFoldersFilePath(steamPath);

        var libraryFoldersResult = LibraryFoldersManifestParser.ParseManifestFile(libraryFoldersFilePath);
        if (libraryFoldersResult.IsFailed)
        {
            yield return ConvertResultToErrorMessage(libraryFoldersResult);
            yield break;
        }

        var libraryFolders = libraryFoldersResult.Value;
        if (libraryFolders.Count == 0) yield break;

        foreach (var libraryFolder in libraryFolders)
        {
            var libraryFolderPath = libraryFolder.Path;
            if (!_fileSystem.DirectoryExists(libraryFolderPath))
            {
                yield return new ErrorMessage($"Steam Library at {libraryFolderPath} doesn't exist!");
                continue;
            }

            foreach (var acfFilePath in libraryFolder.EnumerateAppManifestFilePaths())
            {
                var appManifestResult = AppManifestParser.ParseManifestFile(acfFilePath);
                if (appManifestResult.IsFailed)
                {
                    yield return ConvertResultToErrorMessage(appManifestResult);
                    continue;
                }

                var steamGame = new SteamGame
                {
                    SteamPath = steamPath,
                    AppManifest = appManifestResult.Value,
                    LibraryFolder = libraryFolder,
                };

                yield return steamGame;
            }
        }
    }

    [ExcludeFromCodeCoverage]
    private static OSPlatform GetOS()
    {
        // TODO: use IOSInformation once NexusMods.Paths updated
        var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? OSPlatform.Windows
            : RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? OSPlatform.Linux
                : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                    ? OSPlatform.OSX
                    : throw new PlatformNotSupportedException();
        return os;
    }

    private static ErrorMessage ConvertResultToErrorMessage<T>(Result<T> result)
    {
        // TODO: for compatability, remove this mapping once FindAllGames uses FluentResults
        return new ErrorMessage(result.Errors.Select(x => x.Message).Aggregate((a, b) => $"{a}\n{b}"));
    }
}
