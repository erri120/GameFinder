using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentResults;
using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using JetBrains.Annotations;
using NexusMods.Paths;
using ValveKeyValue;

namespace GameFinder.StoreHandlers.Steam.Services;

/// <summary>
/// Writer for <see cref="LibraryFoldersManifest"/>.
/// </summary>
/// <seealso cref="LibraryFoldersManifestParser"/>
[PublicAPI]
public static class LibraryFoldersManifestWriter
{
    /// <summary>
    /// Saves the manifest to file.
    /// </summary>
    public static Result Write(LibraryFoldersManifest manifest, AbsolutePath outputPath)
    {
        var values = new List<KVObject>();

        for (var i = 0; i < manifest.Count; i++)
        {
            var libraryFolder = manifest[i];
            var children = new List<KVObject>();
            children.AddValue("path", libraryFolder.Path.ToString(), string.Empty);
            children.AddValue("label", libraryFolder.Label, string.Empty);
            children.AddValue("totalsize", libraryFolder.TotalSize.Value, default);
            children.AddDictionary("apps", libraryFolder.AppSizes
                .Select(kv => new KeyValuePair<AppId, ulong>(kv.Key, kv.Value.Value))
                .ToDictionary(kv => kv.Key, kv => kv.Value),
                default
            );

            values.Add(new KVObject($"{i.ToString(CultureInfo.InvariantCulture)}", children));
        }

        var data = new KVObject("libraryfolders", values);

        try
        {
            var serializer = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);
            using var stream = outputPath.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            serializer.Serialize(stream, data);
        }
        catch (Exception e)
        {
            return Result.Fail(
                new ExceptionalError("Exception while writing the Manifest to file!", e)
                    .WithMetadata("Path", outputPath)
            );
        }

        return Result.Ok();
    }
}
