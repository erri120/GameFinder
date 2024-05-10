using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using Reloaded.Memory.Extensions;

namespace GameFinder.StoreHandlers.Xbox.Serialization;

[PublicAPI]
public record GamingRootFile
{
    public const uint Magic = 0x58424752;

    public required AbsolutePath FilePath { get; init; }

    public required RelativePath[] Folders { get; init; }

    public static GamingRootFile? ParseGamingRootFiles(
        ILogger logger,
        ReadOnlySpan<byte> bytes,
        AbsolutePath filePath)
    {
        try
        {
            var magic = ParseUInt32(ref bytes);
            if (magic != Magic)
            {
                // TODO: logging
                return null;
            }

            var folderCount = ParseUInt32(ref bytes);
            if (folderCount >= byte.MaxValue)
            {
                // TODO: logging
                return null;
            }

            // NOTE(erri120): Strings in the file are encoded with Unicode, which is the
            // same representation of strings in C#. This allows us to just cast the bytes
            // to chars and have much easier and faster parsing.
            var span = bytes.Cast<byte, char>();

            var folders = GC.AllocateUninitializedArray<RelativePath>(length: (int)folderCount);
            for (var i = 0; i < folderCount; i++)
            {
                var nullIndex = span.IndexOf('\0');
                if (nullIndex == -1)
                {
                    // TODO: logging
                    return null;
                }

                var slice = span[..nullIndex];
                var path = RelativePath.FromUnsanitizedInput(slice);
                folders[i] = path;
            }

            return new GamingRootFile
            {
                FilePath = filePath,
                Folders = folders,
            };
        }
        catch (Exception e)
        {
            // TODO: logging
            return null;
        }

        static uint ParseUInt32(ref ReadOnlySpan<byte> span)
        {
            // TODO: logging
            var value = BitConverter.ToUInt32(span);
            span = span[sizeof(uint)..];
            return value;
        }
    }
}
