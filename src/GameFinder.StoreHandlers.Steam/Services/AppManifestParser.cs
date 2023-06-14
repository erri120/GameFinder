using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using FluentResults;
using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using JetBrains.Annotations;
using NexusMods.Paths;
using ValveKeyValue;

namespace GameFinder.StoreHandlers.Steam.Services;

/// <summary>
/// Parser for <c>appmanifest_*.acf</c> files.
/// </summary>
[PublicAPI]
public static class AppManifestParser
{
    /// <summary>
    /// Parses the <c>appmanifest_*.acf</c> file at the given path.
    /// </summary>
    public static Result<AppManifest> ParseManifestFile(AbsolutePath manifestPath)
    {
        if (!manifestPath.FileExists)
        {
            return Result.Fail(new Error("Manifest file doesn't exist!")
                .WithMetadata("Path", manifestPath.GetFullPath())
            );
        }

        try
        {
            using var stream = manifestPath.Read();

            var kv = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            var appState = kv.Deserialize(stream, KVSerializerOptions.DefaultOptions);

            if (appState is null)
            {
                return Result.Fail(
                    new Error($"{nameof(KVSerializer)} returned null trying to deserialize the manifest file!")
                        .WithMetadata("Path", manifestPath.GetFullPath())
                );
            }

            if (!appState.Name.Equals("AppState", StringComparison.Ordinal))
            {
                return Result.Fail(
                    new Error("Manifest file is potentially broken because the name doesn't match!")
                        .WithMetadata("Path", manifestPath.GetFullPath())
                        .WithMetadata("ExpectedName", "AppState")
                        .WithMetadata("ActualName", appState.Name)
                );
            }

            // NOTE (@erri120 on 2023-06-02):
            // The ValveKeyValue package by SteamDB (https://github.com/SteamDatabase/ValveKeyValue)
            // is currently "broken" and has multiple issues regarding parsing values
            // of type "uint", "ulong" and "string".
            // see the following links for more information:
            // - https://github.com/SteamDatabase/ValveKeyValue/pull/47
            // - https://github.com/SteamDatabase/ValveKeyValue/issues/53
            // - https://github.com/SteamDatabase/ValveKeyValue/issues/73
            // Until those issues are resolved or I find another library, parsing is going to be broken.

            var appIdResult = ParseRequiredChildObject(appState, "appid", ParseAppId);
            var universeResult = ParseOptionalChildObject(appState, "Universe", ParseByte).Map(x => (SteamUniverse)x);
            var nameResult = ParseRequiredChildObject(appState, "name", ParseString);
            var stateFlagsResult = ParseRequiredChildObject(appState, "StateFlags", ParseByte).Map(x => (StateFlags)x);
            var installationDirectoryNameResult = ParseRequiredChildObject(appState, "installdir", ParseRelativePath);
            var lastUpdatedResult = ParseOptionalChildObject(appState, "LastUpdated", ParseUInt32).Map(x => DateTimeOffset.FromUnixTimeSeconds(x));
            var sizeOnDiskResult = ParseOptionalChildObject(appState, "SizeOnDisk", ParseSize, Size.Zero);
            var stagingSizeResult = ParseOptionalChildObject(appState, "StagingSize", ParseSize, Size.Zero);
            var buildIdResult = ParseOptionalChildObject(appState, "buildid", ParseBuildId, BuildId.Empty);
            var lastOwnerResult = ParseOptionalChildObject(appState, "LastOwner", ParseSteamId, SteamId.Empty);
            var updateResult = ParseOptionalChildObject(appState, "UpdateResult", ParseUInt32);
            var bytesToDownloadResult = ParseOptionalChildObject(appState, "BytesToDownload", ParseSize, Size.Zero);
            var bytesDownloadedResult = ParseOptionalChildObject(appState, "BytesDownloaded", ParseSize, Size.Zero);
            var bytesToStageResult = ParseOptionalChildObject(appState, "BytesToStage", ParseSize, Size.Zero);
            var bytesStagedResult = ParseOptionalChildObject(appState, "BytesStaged", ParseSize, Size.Zero);
            var targetBuildIdResult = ParseOptionalChildObject(appState, "TargetBuildID", ParseBuildId, BuildId.Empty);
            var autoUpdateBehaviorResult = ParseOptionalChildObject(appState, "AutoUpdateBehavior", ParseByte).Map(x => (AutoUpdateBehavior)x);
            var backgroundDownloadBehaviorResult= ParseOptionalChildObject(appState, "AllowOtherDownloadsWhileRunning", ParseByte).Map(x => (BackgroundDownloadBehavior)x);
            var scheduledAutoUpdateResult = ParseOptionalChildObject(appState, "ScheduledAutoUpdate", ParseUInt32).Map(x => DateTimeOffset.FromUnixTimeSeconds(x));
            var fullValidateAfterNextUpdateResult = ParseOptionalChildObject(appState, "FullValidateAfterNextUpdate", ParseBool);

            var installedDepotsResult = ParseInstalledDepots(appState);
            var installScriptsResult = ParseBasicDictionary(
                appState,
                "InstallScripts",
                key => DepotId.From(uint.Parse(key)),
                ParseRelativePath);

            var sharedDepotsResult = ParseBasicDictionary(
                appState,
                "SharedDepots",
                key => DepotId.From(uint.Parse(key)),
                ParseAppId);

            var userConfigResult = ParseBasicDictionary(
                appState,
                "UserConfig",
                key => key,
                ParseString,
                StringComparer.OrdinalIgnoreCase);

            var mountedConfigResult = ParseBasicDictionary(
                appState,
                "MountedConfig",
                key => key,
                ParseString,
                StringComparer.OrdinalIgnoreCase);

            var mergedResults = Result.Merge(
                appIdResult,
                universeResult,
                nameResult,
                stateFlagsResult,
                installationDirectoryNameResult,
                lastUpdatedResult,
                sizeOnDiskResult,
                stagingSizeResult,
                buildIdResult,
                lastOwnerResult,
                updateResult,
                bytesToDownloadResult,
                bytesDownloadedResult,
                bytesToStageResult,
                bytesStagedResult,
                targetBuildIdResult,
                autoUpdateBehaviorResult,
                backgroundDownloadBehaviorResult,
                scheduledAutoUpdateResult,
                fullValidateAfterNextUpdateResult,
                installedDepotsResult,
                installScriptsResult,
                sharedDepotsResult,
                userConfigResult,
                mountedConfigResult
            );

            if (mergedResults.IsFailed) return mergedResults;

            return Result.Ok(
                new AppManifest
                {
                    ManifestPath = manifestPath,
                    AppId = appIdResult.Value,
                    Universe = universeResult.Value,
                    Name = nameResult.Value,
                    StateFlags = stateFlagsResult.Value,
                    InstallationDirectoryName = installationDirectoryNameResult.Value,

                    LastUpdated = lastUpdatedResult.Value,
                    SizeOnDisk = sizeOnDiskResult.Value,
                    StagingSize = stagingSizeResult.Value,
                    BuildId = buildIdResult.Value,
                    LastOwner = lastOwnerResult.Value,
                    UpdateResult = updateResult.Value,
                    BytesToDownload = bytesToDownloadResult.Value,
                    BytesDownloaded = bytesDownloadedResult.Value,
                    BytesToStage = bytesToStageResult.Value,
                    BytesStaged = bytesStagedResult.Value,
                    TargetBuildId = targetBuildIdResult.Value,
                    AutoUpdateBehavior = autoUpdateBehaviorResult.Value,
                    BackgroundDownloadBehavior = backgroundDownloadBehaviorResult.Value,
                    ScheduledAutoUpdate = scheduledAutoUpdateResult.Value,
                    FullValidateAfterNextUpdate = fullValidateAfterNextUpdateResult.Value,

                    InstalledDepots = installedDepotsResult.Value,
                    InstallScripts = installScriptsResult.Value,
                    SharedDepots = sharedDepotsResult.Value,
                    UserConfig = userConfigResult.Value,
                    MountedConfig = mountedConfigResult.Value,
                }
            );
        }
        catch (Exception ex)
        {
            return Result.Fail(
                new ExceptionalError("Exception was thrown while deserializing the manifest file!", ex)
                    .WithMetadata("Path", manifestPath.GetFullPath())
            );
        }
    }

    private static Result<IReadOnlyDictionary<DepotId, InstalledDepot>> ParseInstalledDepots(KVObject appState)
    {
        var installedDepotsObject = FindOptionalChildObject(appState, "InstalledDepots");
        if (installedDepotsObject is null)
        {
            return Result.Ok(
                (IReadOnlyDictionary<DepotId, InstalledDepot>)ImmutableDictionary<DepotId, InstalledDepot>.Empty
            );
        }

        var installedDepotResults = installedDepotsObject.Children
            .Select(ParseInstalledDepot)
            .ToArray();

        var mergedResults = Result.Merge(installedDepotResults);
        return mergedResults.Bind(installedDepots =>
            Result.Ok(
                (IReadOnlyDictionary<DepotId, InstalledDepot>)installedDepots
                    .ToDictionary(x => x.DepotId, x => x)
            )
        );
    }

    private static Result<InstalledDepot> ParseInstalledDepot(KVObject depotObject)
    {
        if (!uint.TryParse(depotObject.Name, NumberFormatInfo.InvariantInfo, out var rawDepotId))
        {
            return Result.Fail(
                new Error("Unable to parse Depot name as a 32-bit unsigned integer!")
                    .WithMetadata("OriginalDepotName", depotObject.Name)
            );
        }

        var depotId = DepotId.From(rawDepotId);

        var manifestIdResult = ParseRequiredChildObject(depotObject, "manifest", ParseManifestId);
        var sizeOnDiskResult = ParseRequiredChildObject(depotObject, "size", ParseSize);
        var dlcAppIdResult = ParseOptionalChildObject(depotObject, "dlcappid", ParseAppId, AppId.Empty);

        var mergedResults = Result.Merge(
            manifestIdResult,
            sizeOnDiskResult,
            dlcAppIdResult
        );

        if (mergedResults.IsFailed) return mergedResults;

        var installedDepot = new InstalledDepot
        {
            DepotId = depotId,
            ManifestId = manifestIdResult.Value,
            SizeOnDisk = sizeOnDiskResult.Value,
            DLCAppId = dlcAppIdResult.Value,
        };

        return Result.Ok(installedDepot);
    }

    private static Result<IReadOnlyDictionary<TKey, TValue>> ParseBasicDictionary<TKey, TValue>(
        KVObject parentObject,
        string dictionaryObjectName,
        Func<string, TKey> keyParser,
        Func<KVValue, TValue> valueParser,
        IEqualityComparer<TKey>? equalityComparer = null)
        where TKey : notnull
    {
        var dictionaryObject = FindOptionalChildObject(parentObject, dictionaryObjectName);
        if (dictionaryObject is null)
        {
            return Result.Ok(
                (IReadOnlyDictionary<TKey, TValue>)ImmutableDictionary<TKey, TValue>.Empty
            );
        }

        var dictionaryValueResults = dictionaryObject.Children
            .Select<KVObject, Result<KeyValuePair<TKey, TValue>>>(childObject =>
            {
                var keyResult = Result.Try(() => keyParser(childObject.Name));
                var valueResult = ParseValue(childObject.Value, valueParser);

                var mergedResult = Result.Merge(
                    keyResult,
                    valueResult
                );

                if (mergedResult.IsFailed) return mergedResult;
                return Result.Ok(new KeyValuePair<TKey, TValue>(keyResult.Value, valueResult.Value));
            }).ToArray();

        var mergedResults = Result.Merge(dictionaryValueResults);
        return mergedResults.Bind(values => Result.Ok(
                (IReadOnlyDictionary<TKey, TValue>)values.ToDictionary(x => x.Key, x => x.Value, equalityComparer)
            )
        );
    }

    #region Core Parsers

    private static Result<T> ParseValue<T>(KVValue value, Func<KVValue, T> parser)
    {
        return Result.Try(
            () => parser(value),
            ex => new ExceptionalError("Unable to parse value!", ex)
        );
    }

    private static Result<T> ParseChildObjectValue<T>(
        KVObject childObject,
        KVObject parentObject,
        Func<KVValue, T> parser)
    {
        return Result.Try(
            () => parser(childObject.Value),
            ex => new ExceptionalError("Unable to parse value of child object!", ex)
                .WithMetadata("ChildObjectName", childObject.Name)
                .WithMetadata("ParentObjectName", parentObject.Name)
        );
    }

    private static KVObject? FindOptionalChildObject(KVObject parentObject, string childObjectName)
    {
        var childObject = parentObject
            .Children
            .FirstOrDefault(child => child.Name.Equals(childObjectName, StringComparison.OrdinalIgnoreCase));

        if (childObject is null && Debugger.IsLogging())
        {
            Debugger.Log(0, Debugger.DefaultCategory, $"Optional child object {childObjectName} was not found in {parentObject.Name}");
        }

        return childObject;
    }

    private static Result<T> ParseOptionalChildObject<T>(
        KVObject parentObject,
        string childObjectName,
        Func<KVValue, T> parser,
        T defaultValue = default)
        where T : struct
    {
        var childObject = FindOptionalChildObject(parentObject, childObjectName);
        return childObject is null
            ? Result.Ok(defaultValue)
            : ParseChildObjectValue(childObject, parentObject, parser);
    }

    private static Result<KVObject> FindRequiredChildObject(KVObject parentObject, string childObjectName)
    {
        var childObject = FindOptionalChildObject(parentObject, childObjectName);

        if (childObject is null)
        {
            return Result.Fail(
                new Error("Unable to find required child object by name in parent!")
                    .WithMetadata("ChildObjectName", childObjectName)
                    .WithMetadata("ParentObjectName", parentObject.Name)
            );
        }

        return Result.Ok(childObject);
    }

    private static Result<T> ParseRequiredChildObject<T>(
        KVObject parentObject,
        string childObjectName,
        Func<KVValue, T> parser)
    {
        var childObjectResult = FindRequiredChildObject(parentObject, childObjectName);
        return childObjectResult.Bind(childObject => ParseChildObjectValue(childObject, parentObject, parser));
    }

    #endregion

    #region Type Parser

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static byte ParseByte(KVValue value) => byte.Parse(ParseString(value), CultureInfo.InvariantCulture);

    private static bool ParseBool(KVValue value)
    {
        var s = ParseString(value);
        if (string.Equals(s, "0", StringComparison.Ordinal)) return false;
        if (string.Equals(s, "1", StringComparison.Ordinal)) return true;
        throw new FormatException($"Unable to parse '{value}' as a boolean!");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint ParseUInt32(KVValue value) => uint.Parse(ParseString(value), CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong ParseUInt64(KVValue value) => ulong.Parse(ParseString(value), CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static string ParseString(KVValue value) => value.ToString(CultureInfo.InvariantCulture);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SteamId ParseSteamId(KVValue value) => SteamId.From(ParseUInt64(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static AppId ParseAppId(KVValue value) => AppId.From(ParseUInt32(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static BuildId ParseBuildId(KVValue value) => BuildId.From(ParseUInt32(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static DepotId ParseDepotId(KVValue value) => DepotId.From(ParseUInt32(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ManifestId ParseManifestId(KVValue value) => ManifestId.From(ParseString(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Size ParseSize(KVValue value) => Size.From(ParseUInt64(value));

    // TODO: sanitize the path (requires https://github.com/Nexus-Mods/NexusMods.App/pull/345)
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static RelativePath ParseRelativePath(KVValue value) => new(ParseString(value));

    #endregion
}
