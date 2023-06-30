using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;

namespace GameFinder.StoreHandlers.Steam.Tests;

public static class ArrangeHelper
{
    private static AbsolutePath CreateOrReturnPath(IFileSystem fileSystem, AbsolutePath path, bool createDirectory)
    {
        if (!createDirectory) return path;
        fileSystem.CreateDirectory(path);
        return path;
    }

    public static AbsolutePath CreateSteamPath(IFileSystem fileSystem, bool createDirectory = false)
    {
        var steamPath = fileSystem
            .GetKnownPath(KnownPath.TempDirectory)
            .Combine($"Steam-{Guid.NewGuid():N}");

        return CreateOrReturnPath(fileSystem, steamPath, createDirectory);
    }

    public static AbsolutePath CreateSteamLibraryPath(IFileSystem fileSystem, bool createDirectory = false)
    {
        var libraryPath = fileSystem
            .GetKnownPath(KnownPath.TempDirectory)
            .Combine($"SteamLibrary-{Guid.NewGuid():N}");

        return CreateOrReturnPath(fileSystem, libraryPath, createDirectory);
    }

    public static AbsolutePath CreateAppManifestPath(IFileSystem fileSystem, AbsolutePath libraryPath = default)
    {
        if (libraryPath == default) libraryPath = CreateSteamLibraryPath(fileSystem);
        return libraryPath.Combine("steamapps").Combine($"appmanifest_{Guid.NewGuid():N}.acf");
    }

    public static SteamId CreateSteamId() => SteamId.From(76561198110222274);

    public static AppManifest CreateAppManifest(AbsolutePath manifestPath)
    {
        var fixture = new Fixture();
        fixture.Customize<Size>(composer => composer.FromFactory<ulong>(Size.From));
        fixture.Customize<BuildId>(composer => composer.FromFactory<uint>(BuildId.From));
        fixture.Customize<SteamId>(composer => composer.FromFactory<ulong>(SteamId.From));
        fixture.Customize<DepotId>(composer => composer.FromFactory<uint>(DepotId.From));
        fixture.Customize<AppId>(composer => composer.FromFactory<uint>(AppId.From));
        fixture.Customize<ManifestId>(composer => composer.FromFactory<ulong>(ManifestId.From));
        fixture.Customize<DateTimeOffset>(composer => composer.FromFactory(() => DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds())));

        return new AppManifest
        {
            ManifestPath = manifestPath,
            AppId = fixture.Create<AppId>(),
            Universe = SteamUniverse.Public,
            Name = fixture.Create<string>(),
            StateFlags = StateFlags.FullyInstalled,
            InstallationDirectoryName = fixture.Create<string>().ToRelativePath(),
            LastUpdated = fixture.Create<DateTimeOffset>(),
            SizeOnDisk = fixture.Create<Size>(),
            StagingSize = fixture.Create<Size>(),
            BuildId = fixture.Create<BuildId>(),
            LastOwner = fixture.Create<SteamId>(),
            UpdateResult = 0,
            BytesToDownload = fixture.Create<Size>(),
            BytesDownloaded = fixture.Create<Size>(),
            BytesToStage = fixture.Create<Size>(),
            BytesStaged = fixture.Create<Size>(),
            TargetBuildId = fixture.Create<BuildId>(),
            AutoUpdateBehavior = AutoUpdateBehavior.AlwaysUpdated,
            BackgroundDownloadBehavior = BackgroundDownloadBehavior.AlwaysAllow,
            ScheduledAutoUpdate = fixture.Create<DateTimeOffset>(),
            FullValidateAfterNextUpdate = true,
            InstalledDepots = fixture
                .CreateMany<DepotId>()
                .Select(depotId => new InstalledDepot
                {
                    DepotId = depotId,
                    ManifestId = fixture.Create<ManifestId>(),
                    SizeOnDisk = fixture.Create<Size>(),
                })
                .ToDictionary(depot => depot.DepotId, depot => depot),
            InstallScripts = fixture
                .CreateMany<DepotId>()
                .Select(depotId => (depotId, fixture.Create<string>().ToRelativePath()))
                .ToDictionary(kv => kv.Item1, kv => kv.Item2),
            SharedDepots = fixture
                .CreateMany<DepotId>()
                .Select(depotId => (depotId, fixture.Create<AppId>()))
                .ToDictionary(kv => kv.Item1, kv => kv.Item2),
            UserConfig = fixture
                .CreateMany<string>()
                .Select(key => (key, fixture.Create<string>()))
                .ToDictionary(kv => kv.Item1, kv => kv.Item2, StringComparer.OrdinalIgnoreCase),
            MountedConfig = fixture
                .CreateMany<string>()
                .Select(key => (key, fixture.Create<string>()))
                .ToDictionary(kv => kv.Item1, kv => kv.Item2, StringComparer.OrdinalIgnoreCase),
        };
    }

    public static WorkshopManifest CreateWorkshopManifest(AbsolutePath manifestPath)
    {
        var fixture = new Fixture();
        fixture.Customize<Size>(composer => composer.FromFactory<ulong>(Size.From));
        fixture.Customize<AppId>(composer => composer.FromFactory<uint>(AppId.From));
        fixture.Customize<WorkshopItemId>(composer => composer.FromFactory<ulong>(WorkshopItemId.From));
        fixture.Customize<WorkshopManifestId>(composer => composer.FromFactory<ulong>(WorkshopManifestId.From));
        fixture.Customize<DateTimeOffset>(composer => composer.FromFactory(() => DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds())));

        return new WorkshopManifest
        {
            ManifestPath = manifestPath,
            AppId = fixture.Create<AppId>(),
            SizeOnDisk = fixture.Create<Size>(),
            NeedsUpdate = fixture.Create<bool>(),
            NeedsDownload = fixture.Create<bool>(),
            LastUpdated = fixture.Create<DateTimeOffset>(),
            LastAppStart = fixture.Create<DateTimeOffset>(),
            InstalledWorkshopItems = fixture
                .CreateMany<WorkshopItemId>()
                .Select(x => new WorkshopItemDetails
                {
                    ItemId = x,
                    SizeOnDisk = fixture.Create<Size>(),
                    ManifestId = fixture.Create<WorkshopManifestId>(),
                    LastUpdated = fixture.Create<DateTimeOffset>(),
                    LastTouched = fixture.Create<DateTimeOffset>(),
                    SubscribedBy = SteamId.FromAccountId(fixture.Create<uint>()),
                })
                .ToDictionary(x => x.ItemId, x => x),
        };
    }

    public static LibraryFoldersManifest CreateLibraryFoldersManifest(AbsolutePath manifestPath)
    {
        var fixture = new Fixture();
        fixture.Customize<Size>(composer => composer.FromFactory<ulong>(Size.From));
        fixture.Customize<AppId>(composer => composer.FromFactory<uint>(AppId.From));

        return new LibraryFoldersManifest
        {
            ManifestPath = manifestPath,
            LibraryFolders = fixture.CreateMany<string>()
                .Select(folderNames =>
                {
                    return new LibraryFolder
                    {
                        Path = manifestPath.FileSystem
                            .GetKnownPath(KnownPath.TempDirectory)
                            .Combine(folderNames),
                        Label = fixture.Create<string>(),
                        TotalDiskSize = fixture.Create<Size>(),
                        AppSizes = fixture.CreateMany<AppId>()
                            .Select(id => (id, fixture.Create<Size>()))
                            .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2),
                    };
                })
                .ToList(),
        };
    }
}
