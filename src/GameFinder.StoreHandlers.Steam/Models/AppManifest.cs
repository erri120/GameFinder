using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam.Models;

/// <summary>
/// Represents a parse app manifest.
/// </summary>
/// <remarks>
/// Manifest files <c>appmanifest_*.acf</c> are stored in Valve's custom
/// KeyValue format.
/// </remarks>
[PublicAPI]
public record AppManifest
{
    /// <summary>
    /// Gets the <see cref="AbsolutePath"/> to the <c>*.acf</c> file
    /// that was parsed to produce this <see cref="AppManifest"/>.
    /// </summary>
    /// <example><c>E:\SteamLibrary\steamapps\appmanifest_262060.acf</c></example>
    /// <seealso cref="InstallationDirectoryName"/>
    /// <seealso cref="InstallationDirectory"/>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public required AbsolutePath ManifestPath { get; init; }

    #region Values

    /// <summary>
    /// Gets the unique identifier of the app.
    /// </summary>
    public required AppId AppId { get; init; }

    /// <summary>
    /// Gets the <see cref="SteamUniverse"/> this app is part of. This is pretty irrelevant.
    /// </summary>
    public SteamUniverse Universe { get; init; }

    /// <summary>
    /// Gets name of the app.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the current state of the app.
    /// </summary>
    public required StateFlags StateFlags { get; init; }

    /// <summary>
    /// Gets the name of the installation directory of the app.
    /// </summary>
    /// <remarks>
    /// This is the relative path to the installation directory.
    /// Use <see cref="InstallationDirectory"/> get the absolute path.
    /// </remarks>
    /// <example><c>DarkestDungeon</c></example>
    /// <seealso cref="InstallationDirectory"/>
    public required RelativePath InstallationDirectoryName { get; init; }

    /// <summary>
    /// Gets the time when the app was last updated.
    /// </summary>
    /// <remarks>
    /// This value is saved as a unix timestamp in the <c>*.acf</c> file.
    /// </remarks>
    public DateTimeOffset LastUpdated { get; init; } = DateTimeOffset.UnixEpoch;

    /// <summary>
    /// Gets the size of the app on disk.
    /// </summary>
    /// <remarks>
    /// This value is only set when installing or updating the app. If the
    /// user adds or removes files from the <see cref="InstallationDirectoryName"/>, Steam
    /// won't update this value automatically. This value will be <see cref="Size.Zero"/>
    /// while the app is being staged.
    /// </remarks>
    /// <seealso cref="StagingSize"/>
    public Size SizeOnDisk { get; init; } = Size.Zero;

    /// <summary>
    /// Gets the size of the app during staging.
    /// </summary>
    /// <remarks>
    /// This value will be <see cref="Size.Zero"/> after the app has been
    /// completely downloaded and installed.
    /// </remarks>
    /// <seealso cref="SizeOnDisk"/>
    public Size StagingSize { get; init; } = Size.Zero;

    /// <summary>
    /// Gets the unique identifier of the current build of the app.
    /// </summary>
    /// <remarks>
    /// This value represents the current "patch" of the app and is
    /// a global identifier that can be used to retrieve the current
    /// update notes using SteamDB.
    /// </remarks>
    /// <seealso cref="CurrentUpdateNotesUrl"/>
    /// <seealso cref="TargetBuildId"/>
    public BuildId BuildId { get; init; } = BuildId.Empty;

    /// <summary>
    /// Gets the last owner of this app.
    /// </summary>
    /// <remarks>
    /// This is usually the last account that installed and launched the app. This
    /// can be used to get the user date for the current app.
    /// </remarks>
    public SteamId LastOwner { get; init; } = SteamId.Empty;

    /// <summary>
    /// Unknown.
    /// </summary>
    /// <remarks>
    /// The meaning of this value is unknown.
    /// </remarks>
    public int UpdateResult { get; init; }

    /// <summary>
    /// Gets the amount of bytes to download.
    /// </summary>
    /// <remarks>This value will be <see cref="Size.Zero"/> when there is no update available.</remarks>
    /// <seealso cref="BytesDownloaded"/>
    public Size BytesToDownload { get; init; } = Size.Zero;

    /// <summary>
    /// Gets the amount of bytes downloaded.
    /// </summary>
    /// <remarks>This value will be <see cref="Size.Zero"/> when there is no update available.</remarks>
    /// <seealso cref="BytesToDownload"/>
    public Size BytesDownloaded { get; init; } = Size.Zero;

    /// <summary>
    /// Gets the amount of bytes to stage.
    /// </summary>
    /// <remarks>This value will be <see cref="Size.Zero"/> when there is no update available.</remarks>
    /// <seealso cref="BytesStaged"/>
    public Size BytesToStage { get; init; } = Size.Zero;

    /// <summary>
    /// Gets the amount of bytes staged.
    /// </summary>
    /// <remarks>This value will be <see cref="Size.Zero"/> when there is no update available.</remarks>
    /// <seealso cref="BytesToStage"/>
    public Size BytesStaged { get; init; } = Size.Zero;

    /// <summary>
    /// Gets the target build ID of the update.
    /// </summary>
    /// <remarks>
    /// This value will be <c>0</c>, if there is no update available.
    /// </remarks>
    /// <seealso cref="NextUpdateNotesUrl"/>
    /// <seealso cref="BuildId"/>
    public BuildId TargetBuildId { get; init; } = BuildId.Empty;

    /// <summary>
    /// Gets the automatic update behavior for this app.
    /// </summary>
    public AutoUpdateBehavior AutoUpdateBehavior { get; init; }

    /// <summary>
    /// Gets the background download behavior for this app.
    /// </summary>
    public BackgroundDownloadBehavior BackgroundDownloadBehavior { get; init; }

    /// <summary>
    /// Gets the time when the app is scheduled to be updated.
    /// </summary>
    /// <remarks>
    /// The <c>*.acf</c> file saves this value as a unix timestamp and the value will be
    /// <c>0</c> or <see cref="DateTimeOffset.UnixEpoch"/> if there is no update scheduled.
    /// </remarks>
    public DateTimeOffset ScheduledAutoUpdate { get; init; } = DateTimeOffset.UnixEpoch;

    /// <summary>
    /// Gets all locally installed depots.
    /// </summary>
    public IReadOnlyDictionary<DepotId, InstalledDepot> InstalledDepots { get; init; } = ImmutableDictionary<DepotId, InstalledDepot>.Empty;

    /// <summary>
    /// Gets all locally installed shared depots.
    /// </summary>
    /// <remarks>
    /// Shared depots are depots from another app and are commonly used for the Steamworks Common Redistributables.
    /// </remarks>
    public IReadOnlyList<KeyValuePair<DepotId, AppId>> SharedDepots { get; init; } = ImmutableList<KeyValuePair<DepotId, AppId>>.Empty;

    /// <summary>
    /// Gets the local user config.
    /// </summary>
    /// <remarks>
    /// This can contains keys like <c>language</c> or <c>BetaKey</c>. It should be noted
    /// that the existence of a key shouldn't be used to infer a config value. As an example:
    /// If the user opts into a beta, the <c>BetaKey</c> value will be set to the beta they
    /// want to participate in. However, once they opt out of the beta, the key <c>BetaKey</c>
    /// will still exist but the value will be an empty string. As such, you shouldn't make
    /// assumption using the existence of a key alone.
    /// </remarks>
    /// <seealso cref="MountedConfig"/>
    public IReadOnlyDictionary<string, string> UserConfig { get; init; } = ImmutableDictionary<string, string>.Empty;

    /// <summary>
    /// Gets the local mounted config.
    /// </summary>
    /// <remarks>
    /// The meaning of these values are unknown. You'd think they have something to do with <see cref="UserConfig"/>
    /// but at the time of writing, I couldn't make out a clear connection since these values aren't being updated at all.
    /// </remarks>
    /// <seealso cref="UserConfig"/>
    public IReadOnlyDictionary<string, string> MountedConfig { get; init; } = ImmutableDictionary<string, string>.Empty;

    #endregion

    #region Helpers

    /// <summary>
    /// Gets the <see cref="AbsolutePath"/> to the installation directory of the app.
    /// </summary>
    /// <remarks>This uses <see cref="ManifestPath"/> to get to the installation directory.</remarks>
    /// <example><c>E:\SteamLibrary\steamapps\common\DarkestDungeon</c></example>
    /// <seealso cref="InstallationDirectoryName"/>
    [SuppressMessage("ReSharper", "CommentTypo")]
    public AbsolutePath InstallationDirectory => ManifestPath.Parent
        .CombineUnchecked("common")
        .CombineUnchecked(InstallationDirectoryName);

    /// <summary>
    /// Gets all locally installed DLCs.
    /// </summary>
    public IEnumerable<KeyValuePair<AppId, InstalledDepot>> InstalledDLCs => InstalledDepots
        .Where(kv => kv.Value.DLCAppId != AppId.Empty)
        .Select(kv => new KeyValuePair<AppId, InstalledDepot>(kv.Value.DLCAppId, kv.Value));

    /// <summary>
    /// Gets the URL to the Update Notes for the current <see cref="BuildId"/> on SteamDB.
    /// </summary>
    public string CurrentUpdateNotesUrl => BuildId.SteamDbUpdateNotesUrl;

    /// <summary>
    /// Gets the URL to the Update Notes for the next update using <see cref="TargetBuildId"/> on SteamDB.
    /// </summary>
    /// <remarks>
    /// This value will be <c>null</c>, if <see cref="TargetBuildId"/> is <see cref="ValueTypes.BuildId.Empty"/>.
    /// </remarks>
    public string? NextUpdateNotesUrl => TargetBuildId == BuildId.Empty ? null : TargetBuildId.SteamDbUpdateNotesUrl;

    /// <summary>
    /// Gets the user-data path for the current app using <see cref="LastOwner"/> and
    /// <see cref="AppId"/>.
    /// </summary>
    /// <param name="steamUserDataDirectory">
    /// Path to the <c>userdata</c> directory in the Steam installation. Example:
    /// <c>C:\Program Files\Steam\userdata</c>
    /// </param>
    /// <example><c>C:\Program Files\Steam\userdata\149956546\262060</c></example>
    /// <returns></returns>
    public AbsolutePath GetUserDataPath(AbsolutePath steamUserDataDirectory)
    {
        return steamUserDataDirectory
            .CombineUnchecked(LastOwner.AccountId.ToString(CultureInfo.InvariantCulture))
            .CombineUnchecked(AppId.Value.ToString(CultureInfo.InvariantCulture));
    }

    #endregion
}
