using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using Reloaded.Memory.Extensions;

namespace GameFinder.StoreHandlers.Xbox.Serialization;

/// <summary>
/// Represents a <c>GamingRoot</c> file.
/// </summary>
[PublicAPI]
public record GamingRootFile
{
    /// <summary>
    /// Expected magic constant at the start of the file.
    /// </summary>
    public const uint ExpectedMagic = 0x58424752;

    /// <summary>
    /// Max folder count. Anything more than this indicates a parsing/file error.
    /// It's very unlikely that someone has more than 255 folders.
    /// </summary>
    public const uint MaxFolderCount = byte.MaxValue;

    /// <summary>
    /// Gets the absolute path to the parsed file.
    /// </summary>
    public required AbsolutePath FilePath { get; init; }

    /// <summary>
    /// Gets the array of folder paths parsed from the file.
    /// All of these paths are relative to the parent directory of
    /// <see cref="FilePath"/>.
    /// </summary>
    /// <seealso cref="GetAbsoluteFolderPaths"/>
    public required RelativePath[] Folders { get; init; }

    /// <summary>
    /// Converts <see cref="Folders"/> into <see cref="AbsolutePath"/>.
    /// </summary>
    public AbsolutePath[] GetAbsoluteFolderPaths()
    {
        var parent = FilePath.Parent;
        return Folders.Select(x => parent.Combine(x)).ToArray();
    }

    /// <summary>
    /// Parses the provided span.
    /// </summary>
    public static GamingRootFile? ParseGamingRootFiles(
        ILogger logger,
        ReadOnlySpan<byte> bytes,
        AbsolutePath filePath)
    {
        try
        {
            var actualMagic = ParseUInt32(ref bytes);
            if (actualMagic != ExpectedMagic)
            {
                LogMessages.MagicMismatch(logger, ExpectedMagic, actualMagic, filePath);
                return null;
            }

            var folderCount = ParseUInt32(ref bytes);
            if (folderCount >= MaxFolderCount)
            {
                LogMessages.CountTooHigh(logger, MaxFolderCount, folderCount, filePath);
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
                    LogMessages.MissingNullTerminator(logger, filePath);
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
            LogMessages.ExceptionWhileParsingGamingRootFile(logger, e, filePath);
            return null;
        }

        static uint ParseUInt32(ref ReadOnlySpan<byte> span)
        {
            var value = BitConverter.ToUInt32(span);
            span = span[sizeof(uint)..];
            return value;
        }
    }
}
