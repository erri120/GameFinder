using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;
using OneOf;

namespace GameFinder.StoreHandlers.Xbox;

/// <summary>
/// Handler for finding games installed with Xbox Game Pass.
/// </summary>
[PublicAPI]
[RequiresUnreferencedCode($"Calls System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader) with {nameof(Package)}. Make sure {nameof(Package)} is preserved by using TrimmerRootDescriptor or TrimmerRootAssembly!")]
public class XboxHandler : AHandler<XboxGame, XboxGameId>
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fileSystem">
    /// The implementation of <see cref="IFileSystem"/> to use. For a shared instance use
    /// <see cref="FileSystem.Shared"/>. For tests either use <see cref="InMemoryFileSystem"/>,
    /// a custom implementation or just a mock of the interface.
    /// </param>
    public XboxHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public override Func<XboxGame, XboxGameId> IdSelector => game => game.Id;

    /// <inheritdoc/>
    public override IEqualityComparer<XboxGameId> IdEqualityComparer => XboxGameIdComparer.Default;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<XboxGame, LogMessage>> FindAllGames()
    {
        var (paths, messages) = GetAppFolders(_fileSystem);
        foreach (var message in messages)
        {
            yield return message;
        }

        if (paths.Count == 0)
        {
            yield return new LogMessage("Unable to find any app folders!");
        }

        foreach (var path in paths)
        {
            if (!_fileSystem.DirectoryExists(path)) continue;
            var directories = _fileSystem
                .EnumerateDirectories(path, recursive: false)
                .ToArray();

            if (directories.Length == 0)
            {
                yield return new LogMessage($"App folder {path} does not contain any sub directories!", MessageLevel.Debug);
                continue;
            }

            foreach (var directory in directories)
            {
                var appManifestFilePath = directory.CombineUnchecked("appxmanifest.xml");
                if (!_fileSystem.FileExists(appManifestFilePath))
                {
                    var contentDirectory = directory.CombineUnchecked("Content");
                    if (_fileSystem.DirectoryExists(contentDirectory))
                    {
                        appManifestFilePath = contentDirectory.CombineUnchecked("appxmanifest.xml");
                        if (!_fileSystem.FileExists(appManifestFilePath))
                        {
                            yield return new LogMessage($"Manifest file does not exist at {appManifestFilePath}");
                            continue;
                        }
                    }
                    else
                    {
                        yield return new LogMessage($"Manifest file does not exist at {appManifestFilePath} and there is no Content folder at {contentDirectory}");
                        continue;
                    }
                }

                var result = ParseAppManifest(_fileSystem, appManifestFilePath);
                if (result.TryGetGame(out var game))
                {
                    yield return game;
                }
                else
                {
                    yield return result.AsMessage();
                }
            }
        }
    }

    internal static (List<AbsolutePath> paths, List<LogMessage> messages) GetAppFolders(IFileSystem fileSystem)
    {
        var paths = new List<AbsolutePath>();
        var messages = new List<LogMessage>();

        foreach (var rootDirectory in fileSystem.EnumerateRootDirectories())
        {
            if (!fileSystem.DirectoryExists(rootDirectory)) continue;

            var modifiableWindowsAppsPath = rootDirectory
                .CombineUnchecked("Program Files")
                .CombineUnchecked("ModifiableWindowsApps");

            var gamingRootFilePath = rootDirectory.CombineUnchecked(".GamingRoot");

            var modifiableWindowsAppsDirectoryExists = fileSystem.DirectoryExists(modifiableWindowsAppsPath);
            var gamingRootFileExists = fileSystem.FileExists(gamingRootFilePath);

            if (modifiableWindowsAppsDirectoryExists) paths.Add(modifiableWindowsAppsPath);

            if (!modifiableWindowsAppsDirectoryExists && !gamingRootFileExists)
            {
                messages.Add(new($"Neither {modifiableWindowsAppsPath} nor {gamingRootFilePath} exist on the current drive.", MessageLevel.Debug));
                continue;
            }

            if (!gamingRootFileExists) continue;

            var parseGamingRootFileResult = ParseGamingRootFile(fileSystem, gamingRootFilePath);
            parseGamingRootFileResult.Switch(
                additionalPaths => paths.AddRange(additionalPaths),
                message => messages.Add(message)
            );
        }

        return (paths, messages);
    }

    internal static OneOf<XboxGame, LogMessage> ParseAppManifest(IFileSystem fileSystem,
        AbsolutePath manifestFilePath)
    {
        try
        {
            using var stream = fileSystem.ReadFile(manifestFilePath);
            using var reader = XmlReader.Create(stream, new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
            });

            var obj = new XmlSerializer(typeof(Package)).Deserialize(reader);
            if (obj is null)
            {
                return new LogMessage($"Unable to deserialize file {manifestFilePath}");
            }

            if (obj is not Package appManifest)
            {
                return new LogMessage($"Deserialization of {manifestFilePath} failed: resulting object is not of type {typeof(Package)} but {obj.GetType()}");
            }

            var displayName = appManifest.Properties.DisplayName;
            var id = appManifest.Identity.Name;
            var game = new XboxGame(XboxGameId.From(id), displayName, manifestFilePath.Parent);
            return game;
        }
        catch (Exception e)
        {
            return new LogMessage(e, $"Unable to parse manifest file {manifestFilePath}");
        }
    }

    internal static OneOf<List<AbsolutePath>, LogMessage> ParseGamingRootFile(
        IFileSystem fileSystem, AbsolutePath gamingRootFilePath)
    {
        try
        {
            using var stream = fileSystem.ReadFile(gamingRootFilePath);
            using var reader = new BinaryReader(stream, Encoding.Unicode);

            const uint expectedMagic = 0x58424752;
            var magic = reader.ReadUInt32();
            if (magic != expectedMagic)
            {
                return new LogMessage($"Unable to parse {gamingRootFilePath}, file magic does not match: expected {expectedMagic.ToString("x8", NumberFormatInfo.InvariantInfo)} got {magic.ToString("x8", NumberFormatInfo.InvariantInfo)}");
            }

            var folderCount = reader.ReadUInt32();
            if (folderCount >= byte.MaxValue)
            {
                return new LogMessage($"Folder count exceeds the limit: {folderCount}");
            }

            var parentFolder = gamingRootFilePath.Parent;
            var folders = new List<AbsolutePath>((int)folderCount);
            for (var i = 0; i < folderCount; i++)
            {
                var sb = new StringBuilder();
                var c = reader.ReadChar();
                while (c != '\0')
                {
                    sb.Append(c);
                    c = reader.ReadChar();
                }

                var part = sb.ToString().ToRelativePath();
                folders.Add(parentFolder.CombineUnchecked(part));
            }

            return folders;
        }
        catch (Exception e)
        {
            return new LogMessage(e, $"Unable to parse gaming root file {gamingRootFilePath}");
        }

    }
}
