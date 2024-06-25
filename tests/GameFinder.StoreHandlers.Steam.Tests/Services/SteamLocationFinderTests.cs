using System.Globalization;
using System.Runtime.InteropServices;
using FluentResults.Extensions.FluentAssertions;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam.Services;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam.Tests.Services;

public class SteamLocationFinderTests
{
    [Fact]
    public void Test_FindSteam()
    {
        var fs = new InMemoryFileSystem();
        var registry = new InMemoryRegistry();

        var res = SteamLocationFinder.FindSteam(fs, registry: null);
        res
            .Should().BeFailure()
            .And.HaveError("Unable to find a valid Steam installation at the default installation paths!");

        res = SteamLocationFinder.FindSteam(fs, registry);
        res
            .Should().BeFailure()
            .And.HaveError("Unable to find a valid Steam installation at the default installation paths, and in the Registry!");

        var steamPathFromRegistry = ArrangeHelper.CreateSteamPath(fs, createDirectory: true);
        var libraryFoldersFilePathFromRegistry = SteamLocationFinder.GetLibraryFoldersFilePath(steamPathFromRegistry);
        fs.AddEmptyFile(libraryFoldersFilePathFromRegistry);

        var key = registry.AddKey(RegistryHive.CurrentUser, SteamLocationFinder.SteamRegistryKey);
        key.AddValue(SteamLocationFinder.SteamRegistryValueName, steamPathFromRegistry.GetFullPath());

        res = SteamLocationFinder.FindSteam(fs, registry);
        res
            .Should().BeSuccess()
            .And.HaveValue(steamPathFromRegistry);

        var defaultSteamPath = SteamLocationFinder.GetDefaultSteamInstallationPaths(fs).First();
        var defaultLibraryFoldersFilePath = SteamLocationFinder.GetLibraryFoldersFilePath(defaultSteamPath);

        fs.AddDirectory(defaultSteamPath);
        fs.AddEmptyFile(defaultLibraryFoldersFilePath);

        res = SteamLocationFinder.FindSteam(fs, registry: null);
        res
            .Should().BeSuccess()
            .And.HaveValue(defaultSteamPath);
    }

    [Fact]
    public void Test_IsValidSteamInstallation()
    {
        var fs = new InMemoryFileSystem();
        var steamPath = ArrangeHelper.CreateSteamPath(fs);

        SteamLocationFinder
            .IsValidSteamInstallation(steamPath)
            .Should().BeFalse(because: "The Steam directory doesn't exist");

        fs.AddDirectory(steamPath);

        SteamLocationFinder
            .IsValidSteamInstallation(steamPath)
            .Should().BeFalse(because: "The libraryfolders.vdf file doesn't exist");

        var libraryFoldersFilePath = SteamLocationFinder.GetLibraryFoldersFilePath(steamPath);
        fs.AddEmptyFile(libraryFoldersFilePath);

        SteamLocationFinder
            .IsValidSteamInstallation(steamPath)
            .Should().BeTrue();
    }

    [Fact]
    public void Test_GetLibraryFoldersFile()
    {
        var fs = new InMemoryFileSystem();
        var steamPath = ArrangeHelper.CreateSteamPath(fs);
        var libraryFoldersFilePath = steamPath.Combine("config").Combine("libraryfolders.vdf");

        SteamLocationFinder.GetLibraryFoldersFilePath(steamPath).Should().Be(libraryFoldersFilePath);
    }

    [Fact]
    public void Test_GetUserDataDirectoryPath()
    {
        var fs = new InMemoryFileSystem();
        var steamPath = ArrangeHelper.CreateSteamPath(fs);
        var steamId = ArrangeHelper.CreateSteamId();
        var userDataDirectoryPath = steamPath
            .Combine("userdata")
            .Combine(steamId.AccountId.ToString(CultureInfo.InvariantCulture));

        SteamLocationFinder.GetUserDataDirectoryPath(steamPath, steamId).Should().Be(userDataDirectoryPath);
    }

    [Fact]
    public void Test_GetSteamPathFromRegistry()
    {
        var fs = new InMemoryFileSystem();
        var registry = new InMemoryRegistry();
        var steamPath = ArrangeHelper.CreateSteamPath(fs, createDirectory: true);

        var res = SteamLocationFinder.GetSteamPathFromRegistry(fs, registry);
        res
            .Should().BeFailure()
            .And.HaveError("Unable to open the Steam registry key!");

        var key = registry.AddKey(RegistryHive.CurrentUser, SteamLocationFinder.SteamRegistryKey);

        res = SteamLocationFinder.GetSteamPathFromRegistry(fs, registry);
        res
            .Should().BeFailure()
            .And.HaveError("Unable to get string value from the Steam registry key!");

        key.AddValue(SteamLocationFinder.SteamRegistryValueName, steamPath.GetFullPath());

        res = SteamLocationFinder.GetSteamPathFromRegistry(fs, registry);
        res
            .Should().BeSuccess()
            .And.HaveValue(steamPath);
    }

    [Fact]
    public void Test_GetDefaultSteamInstallationPaths_Linux()
    {
        var fs = new InMemoryFileSystem(new OSInformation(OSPlatform.Linux));
        SteamLocationFinder
            .GetDefaultSteamInstallationPaths(fs)
            .ToArray()
            .Should().HaveCount(7);
    }

    [Fact]
    public void Test_GetDefaultSteamInstallationPaths_Windows()
    {
        var fs = new InMemoryFileSystem(new OSInformation(OSPlatform.Windows));
        var overlayFileSystem = fs.CreateOverlayFileSystem(
            new Dictionary<AbsolutePath, AbsolutePath>(),
            new Dictionary<KnownPath, AbsolutePath>
            {
                { KnownPath.ProgramFilesX86Directory, fs.GetKnownPath(KnownPath.TempDirectory) },
            });

        SteamLocationFinder
            .GetDefaultSteamInstallationPaths(overlayFileSystem)
            .ToArray()
            .Should().HaveCount(1);
    }

    [Fact]
    public void Test_GetDefaultSteamInstallationPaths_OSX()
    {
        var fs = new InMemoryFileSystem(new OSInformation(OSPlatform.OSX));

        var overlayFileSystem = fs.CreateOverlayFileSystem(
            new Dictionary<AbsolutePath, AbsolutePath>(),
            new Dictionary<KnownPath, AbsolutePath>
            {
                { KnownPath.ProgramFilesX86Directory, fs.GetKnownPath(KnownPath.TempDirectory) },
            });

        SteamLocationFinder
            .GetDefaultSteamInstallationPaths(overlayFileSystem)
            .ToArray()
            .Should().HaveCount(1);
    }
}
