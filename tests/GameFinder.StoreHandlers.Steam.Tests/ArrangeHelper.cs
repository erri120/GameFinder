using System.Globalization;
using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;

namespace GameFinder.StoreHandlers.Steam.Tests;

public static class ArrangeHelper
{
    private static readonly RelativePath SteamAppsDirectoryName = "steamapps".ToRelativePath();

    public static AbsolutePath GetSteamLibraryPath(IFileSystem fileSystem)
    {
        var path = fileSystem
            .GetKnownPath(KnownPath.TempDirectory)
            .CombineUnchecked("SteamLibrary-" + Guid.NewGuid().ToString("N"));

        fileSystem.CreateDirectory(path);
        return path;
    }

    public static AbsolutePath GetAppManifestFilePath(AbsolutePath steamLibraryPath, AppId appId)
    {
        return steamLibraryPath
            .CombineUnchecked(SteamAppsDirectoryName)
            .CombineUnchecked($"appmanifest_{appId.Value.ToString(CultureInfo.InvariantCulture)}.acf");
    }

    public static AppManifest CreateAppManifest(AbsolutePath manifestPath)
    {
        var fixture = new Fixture();
        fixture.Customize<Size>(composer => composer.FromFactory<ulong>(Size.From));
        fixture.Customize<BuildId>(composer => composer.FromFactory<uint>(BuildId.From));
        fixture.Customize<SteamId>(composer => composer.FromFactory<ulong>(SteamId.From));
        fixture.Customize<DepotId>(composer => composer.FromFactory<uint>(DepotId.From));
        fixture.Customize<AppId>(composer => composer.FromFactory<uint>(AppId.From));
        fixture.Customize<ManifestId>(composer => composer.FromFactory<ulong>(value => ManifestId.From(value.ToString())));
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
}
