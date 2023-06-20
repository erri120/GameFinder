using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using FluentResults;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam.Models;
using JetBrains.Annotations;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;

namespace GameFinder.StoreHandlers.Steam.Services;

/// <summary>
/// Finds the current Steam installation.
/// </summary>
[PublicAPI]
public static class SteamLocationFinder
{
    /// <summary>
    /// The name of the <c>config</c> directory.
    /// </summary>
    /// <seealso cref="GetLibraryFoldersFilePath"/>
    public static readonly RelativePath ConfigDirectoryName = "config".ToRelativePath();

    /// <summary>
    /// The name of the <c>libraryfolders.vdf</c> file.
    /// </summary>
    /// <seealso cref="GetLibraryFoldersFilePath"/>
    public static readonly RelativePath LibraryFoldersFileName = "libraryfolders.vdf".ToRelativePath();

    /// <summary>
    /// The name of the <c>userdata</c> directory.
    /// </summary>
    /// <seealso cref="GetUserDataDirectoryPath"/>
    public static readonly RelativePath UserDataDirectoryName = "userdata".ToRelativePath();

    /// <summary>
    /// The registry key used to find Steam.
    /// </summary>
    /// <seealso cref="GetSteamPathFromRegistry"/>
    public const string SteamRegistryKey = @"Software\Valve\Steam";

    /// <summary>
    /// The registry value name to find Steam.
    /// </summary>
    /// <seealso cref="GetSteamPathFromRegistry"/>
    public const string SteamRegistryValueName = "SteamPath";

    /// <summary>
    /// Tries to find a valid Steam installation.
    /// </summary>
    /// <remarks>
    /// This uses <see cref="GetDefaultSteamInstallationPaths"/>, <see cref="GetSteamPathFromRegistry"/>
    /// and <see cref="IsValidSteamInstallation"/> to find a valid installation.
    /// </remarks>
    public static Result<AbsolutePath> FindSteam(IFileSystem fileSystem, IRegistry? registry, OSPlatform os)
    {
        // TODO: use IOSInformation once NexusMods.Paths updated

        // 1) try the default installation paths
        var defaultSteamInstallationPath = GetDefaultSteamInstallationPaths(fileSystem, os)
            .FirstOrDefault(IsValidSteamInstallation);

        if (defaultSteamInstallationPath != default) return Result.Ok(defaultSteamInstallationPath);

        // 2) try the registry, if there is any
        if (registry is null)
        {
            return Result.Fail(
                new Error("Unable to find a valid Steam installation at the default installation paths!")
            );
        }

        var pathFromRegistryResult = GetSteamPathFromRegistry(fileSystem, registry);
        if (pathFromRegistryResult.IsFailed || !IsValidSteamInstallation(pathFromRegistryResult.Value))
        {
            return Result.Merge(
                Result.Fail(
                    new Error("Unable to find a valid Steam installation at the default installation paths, and in the Registry!")
                ),
                pathFromRegistryResult
            ).ToResult();
        }

        return Result.Ok(pathFromRegistryResult.Value);
    }

    /// <summary>
    /// Checks whether the given Steam installation path is valid.
    /// </summary>
    /// <remarks>
    /// A valid Steam installation requires a existing directory,
    /// and a existing <c>libraryfolders.vdf</c> file. This method
    /// uses <see cref="GetLibraryFoldersFilePath"/> to get that file path.
    /// </remarks>
    public static bool IsValidSteamInstallation(AbsolutePath steamPath)
    {
        if (!steamPath.DirectoryExists()) return false;

        var libraryFoldersFile = GetLibraryFoldersFilePath(steamPath);
        return libraryFoldersFile.FileExists;
    }

    /// <summary>
    /// Returns the path to the <c>libraryfolders.vdf</c> file inside the Steam <c>config</c>
    /// directory.
    /// </summary>
    public static AbsolutePath GetLibraryFoldersFilePath(AbsolutePath steamPath)
    {
        return steamPath
            .CombineUnchecked(ConfigDirectoryName)
            .CombineUnchecked(LibraryFoldersFileName);
    }

    /// <summary>
    /// Returns the path to the user data directory of the provided user.
    /// </summary>
    public static AbsolutePath GetUserDataDirectoryPath(AbsolutePath steamPath, SteamId steamId)
    {
        return steamPath
            .CombineUnchecked(UserDataDirectoryName)
            .CombineUnchecked(steamId.AccountId.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tries to get the Steam installation path from the registry.
    /// </summary>
    public static Result<AbsolutePath> GetSteamPathFromRegistry(
        IFileSystem fileSystem,
        IRegistry registry)
    {
        try
        {
            var currentUser = registry.OpenBaseKey(RegistryHive.CurrentUser);

            using var regKey = currentUser.OpenSubKey(SteamRegistryKey);
            if (regKey is null)
            {
                return Result.Fail(
                    new Error("Unable to open the Steam registry key!")
                        .WithMetadata("RegistryKey", SteamRegistryKey)
                );
            }

            if (!regKey.TryGetString(SteamRegistryValueName, out var steamPath))
            {
                return Result.Fail(
                    new Error("Unable to get string value from the Steam registry key!")
                        .WithMetadata("RegistryKey", SteamRegistryKey)
                        .WithMetadata("ValueName", SteamRegistryValueName)
                );
            }

            // TODO: sanitize the path
            var directoryInfo = fileSystem.FromFullPath(steamPath);
            return directoryInfo;
        }
        catch (Exception e)
        {
            return Result.Fail(
                new ExceptionalError("Exception thrown while getting the Steam installation path from the registry!", e)
            );
        }
    }

    /// <summary>
    /// Returns all possible default Steam installation paths for the given platform.
    /// </summary>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    [SuppressMessage("ReSharper", "CommentTypo")]
    public static IEnumerable<AbsolutePath> GetDefaultSteamInstallationPaths(IFileSystem fileSystem, OSPlatform os)
    {
        // TODO: use IOSInformation once NexusMods.Paths updated

        if (os == OSPlatform.Windows)
        {
            yield return fileSystem
                .GetKnownPath(KnownPath.ProgramFilesX86Directory)
                .CombineUnchecked("Steam");

            yield break;
        }

        if (os == OSPlatform.Linux)
        {
            // "$XDG_DATA_HOME/Steam" which is usually "~/.local/share/Steam"
            yield return fileSystem
                .GetKnownPath(KnownPath.LocalApplicationDataDirectory)
                .CombineUnchecked("Steam");

            // "~/.steam/debian-installation"
            yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory)
                .CombineUnchecked(".steam")
                .CombineUnchecked("debian-installation");

            // "~/.var/app/com.valvesoftware.Steam/data/Steam" (flatpak installation)
            // see https://github.com/flatpak/flatpak/wiki/Filesystem for details
            yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory)
                .CombineUnchecked(".var/app/com.valvesoftware.Steam/data/Steam");

            // "~/.steam/steam"
            // this is a legacy installation directory and is often soft linked to
            // the actual installation directory
            yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory)
                .CombineUnchecked(".steam")
                .CombineUnchecked("steam");

            // "~/.steam"
            yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory)
                .CombineUnchecked(".steam");

            // "~/.local/.steam"
            yield return fileSystem.GetKnownPath(KnownPath.HomeDirectory)
                .CombineUnchecked(".local")
                .CombineUnchecked(".steam");

            yield break;
        }

        throw new PlatformNotSupportedException("GameFinder doesn't support the current platform!");
    }
}
