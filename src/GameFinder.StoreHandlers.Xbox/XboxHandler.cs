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
[RequiresUnreferencedCode("Calls System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader)")]
public class XboxHandler : AHandler<XboxGame, XboxGameId>
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fileSystem"></param>
    public XboxHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public override Func<XboxGame, XboxGameId> IdSelector => game => game.Id;

    /// <inheritdoc/>
    protected override IEqualityComparer<XboxGameId> IdEqualityComparer => XboxGameIdComparer.Default;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<XboxGame, ErrorMessage>> FindAllGames()
    {
        var (paths, errors) = GetAppFolders(_fileSystem);
        foreach (var error in errors)
        {
            yield return error;
        }

        if (paths.Count == 0)
        {
            yield return new ErrorMessage("Unable to find any app folders!");
        }

        foreach (var path in paths)
        {
            if (!_fileSystem.DirectoryExists(path)) continue;
            var directories = _fileSystem
                .EnumerateDirectories(path, recursive: false)
                .ToArray();

            if (directories.Length == 0)
            {
                yield return new ErrorMessage($"App folder {path} does not contain any sub directories!");
                continue;
            }

            foreach (var directory in directories)
            {
                var appManifestFilePath = directory.CombineUnchecked("appxmanifest.xml");
                if (!_fileSystem.FileExists(appManifestFilePath))
                {
                    yield return new ErrorMessage($"Manifest file {appManifestFilePath} does not exist!");
                    continue;
                }

                var result = ParseAppManifest(_fileSystem, appManifestFilePath);
                if (result.TryGetGame(out var game))
                {
                    yield return game;
                }
                else
                {
                    yield return result.AsError();
                }
            }
        }
    }

    internal static (List<AbsolutePath> paths, List<ErrorMessage> errors) GetAppFolders(IFileSystem fileSystem)
    {
        var paths = new List<AbsolutePath>();
        var errors = new List<ErrorMessage>();

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
                errors.Add($"Neither {modifiableWindowsAppsPath} nor {gamingRootFilePath} exist on the current drive.");
                continue;
            }

            if (!gamingRootFileExists) continue;

            var parseGamingRootFileResult = ParseGamingRootFile(fileSystem, gamingRootFilePath);
            parseGamingRootFileResult.Switch(
                additionalPaths => paths.AddRange(additionalPaths),
                error => errors.Add(error)
            );
        }

        return (paths, errors);
    }

    internal static OneOf<XboxGame, ErrorMessage> ParseAppManifest(IFileSystem fileSystem,
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
                return new ErrorMessage($"Unable to deserialize file {manifestFilePath}");
            }

            if (obj is not Package appManifest)
            {
                return new ErrorMessage($"Deserialization of {manifestFilePath} failed: resulting object is not of type {typeof(Package)} but {obj.GetType()}");
            }

            var displayName = appManifest.Properties.DisplayName;
            var id = appManifest.Identity.Name;
            var game = new XboxGame(XboxGameId.From(id), displayName, manifestFilePath.Parent);
            return game;
        }
        catch (Exception e)
        {
            return new ErrorMessage(e, $"Unable to parse manifest file {manifestFilePath}");
        }
    }

    internal static OneOf<List<AbsolutePath>, ErrorMessage> ParseGamingRootFile(
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
                return new ErrorMessage($"Unable to parse {gamingRootFilePath}, file magic does not match: expected {expectedMagic.ToString("x8", NumberFormatInfo.InvariantInfo)} got {magic.ToString("x8", NumberFormatInfo.InvariantInfo)}");
            }

            var folderCount = reader.ReadUInt32();
            if (folderCount >= byte.MaxValue)
            {
                return new ErrorMessage($"Folder count exceeds the limit: {folderCount}");
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
            return new ErrorMessage(e, $"Unable to parse gaming root file {gamingRootFilePath}");
        }

    }
}
