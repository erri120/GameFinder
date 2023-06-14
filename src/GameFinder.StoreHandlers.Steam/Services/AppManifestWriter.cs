using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FluentResults;
using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using JetBrains.Annotations;
using NexusMods.Paths;
using ValveKeyValue;

namespace GameFinder.StoreHandlers.Steam.Services;

/// <summary>
/// Writer for <see cref="AppManifest"/>.
/// </summary>
/// <seealso cref="AppManifestParser"/>
[PublicAPI]
public static class AppManifestWriter
{
    public static Result Write(AppManifest manifest, AbsolutePath outputPath)
    {
        var values = new List<KVObject>();
        values.AddValue("appid", manifest.AppId, AppId.Empty);
        values.AddValue("Universe", (byte)manifest.Universe, -1);
        values.AddValue("name", manifest.Name, string.Empty);
        values.AddValue("StateFlags", (byte)manifest.StateFlags, -1);
        values.AddValue("installdir", manifest.InstallationDirectoryName.ToString(), string.Empty);
        values.AddValue("LastUpdated", manifest.LastUpdated.ToUnixTimeSeconds(), default);
        values.AddValue("SizeOnDisk", manifest.SizeOnDisk.Value, default);
        values.AddValue("StagingSize", manifest.StagingSize.Value, default);
        values.AddValue("buildid", manifest.BuildId, BuildId.Empty);
        values.AddValue("LastOwner", manifest.LastOwner.RawId, SteamId.Empty.RawId);
        values.AddValue("UpdateResult", manifest.UpdateResult, default);
        values.AddValue("BytesToDownload", manifest.BytesToDownload.Value, default);
        values.AddValue("BytesDownloaded", manifest.BytesDownloaded.Value, default);
        values.AddValue("BytesToStage", manifest.BytesToStage.Value, default);
        values.AddValue("BytesStaged", manifest.BytesStaged.Value, default);
        values.AddValue("TargetBuildID", manifest.TargetBuildId, BuildId.Empty);
        values.AddValue("AutoUpdateBehavior", (byte)manifest.AutoUpdateBehavior, -1);
        values.AddValue("AllowOtherDownloadsWhileRunning", (byte)manifest.BackgroundDownloadBehavior, -1);
        values.AddValue("ScheduledAutoUpdate", manifest.ScheduledAutoUpdate.ToUnixTimeSeconds(), default);
        values.AddValue("FullValidateAfterNextUpdate", manifest.FullValidateAfterNextUpdate ? "1" : "0", string.Empty);

        if (manifest.InstalledDepots.Count != 0)
        {
            var children = new List<KVObject>();

            foreach (var kv in manifest.InstalledDepots)
            {
                var (depotId, installedDepot) = kv;
                var objValues = new List<KVObject>();
                objValues.AddValue("manifest", installedDepot.ManifestId, ManifestId.Empty);
                objValues.AddValue("size", installedDepot.SizeOnDisk.Value, default);
                objValues.AddValue("dlcappid", installedDepot.DLCAppId, AppId.Empty);

                var obj = new KVObject(depotId.ToString(), objValues);
                children.Add(obj);
            }

            values.Add(new KVObject("InstalledDepots", children));
        }

        values.AddDictionary("InstallScripts", manifest.InstallScripts, RelativePath.Empty);
        values.AddDictionary("SharedDepots", manifest.SharedDepots, AppId.Empty);
        values.AddDictionary("UserConfig", manifest.UserConfig, string.Empty);
        values.AddDictionary("MountedConfig", manifest.MountedConfig, string.Empty);

        var data = new KVObject("AppState", values);

        try
        {
            var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            using var stream = outputPath.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            serializer.Serialize(stream, data);
        }
        catch (Exception e)
        {
            return Result.Fail(
                new ExceptionalError("Exception while writing the AppManifest to file!", e)
                    .WithMetadata("AppId", manifest.AppId)
                    .WithMetadata("Path", outputPath)
            );
        }

        return Result.Ok();
    }

    private static void AddValue<TValue>(this List<KVObject> list, string name, TValue value, TValue defaultValue)
        where TValue : notnull
    {
        if (value.Equals(defaultValue)) return;
        list.Add(new KVObject(name, new StringValue(value)));
    }

    private static void AddDictionary<TKey, TValue>(this List<KVObject> list, string name, IReadOnlyDictionary<TKey, TValue> dictionary, TValue defaultValue)
        where TKey : notnull
        where TValue : notnull
    {
        if (dictionary.Count == 0) return;

        var children = new List<KVObject>();
        foreach (var kv in dictionary)
        {
            children.AddValue(kv.Key.ToString()!, kv.Value, defaultValue);
        }

        list.Add(new KVObject(name, children));
    }

    [ExcludeFromCodeCoverage]
    private class StringValue : KVValue
    {
        private readonly string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public StringValue(object obj)
        {
            _value = obj.ToString() ?? throw new ArgumentException($"Doesn't have a ToString: {obj.GetType()}", nameof(obj));
        }

        public override string ToString() => _value;

        public override KVValueType ValueType => KVValueType.String;
        public override TypeCode GetTypeCode() => TypeCode.String;

        public override bool ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(_value, provider);
        public override byte ToByte(IFormatProvider? provider) => Convert.ToByte(_value, provider);
        public override char ToChar(IFormatProvider? provider) => Convert.ToChar(_value, provider);
        public override DateTime ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(_value, provider);
        public override decimal ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(_value, provider);
        public override double ToDouble(IFormatProvider? provider) => Convert.ToDouble(_value, provider);
        public override short ToInt16(IFormatProvider? provider) => Convert.ToInt16(_value, provider);
        public override int ToInt32(IFormatProvider? provider) => Convert.ToInt32(_value, provider);
        public override long ToInt64(IFormatProvider? provider) => Convert.ToInt64(_value, provider);
        public override sbyte ToSByte(IFormatProvider? provider) => Convert.ToSByte(_value, provider);
        public override float ToSingle(IFormatProvider? provider) => Convert.ToSingle(_value, provider);
        public override string ToString(IFormatProvider? provider) => Convert.ToString(_value, provider);
        public override object ToType(Type conversionType, IFormatProvider? provider) => throw new NotSupportedException();
        public override ushort ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(_value, provider);
        public override uint ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(_value, provider);
        public override ulong ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(_value, provider);
    }
}
