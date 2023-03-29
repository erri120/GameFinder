using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;

namespace GameFinder.StoreHandlers.Xbox;

/// <summary>
/// Handler for finding games installed with Xbox Game Pass.
/// </summary>
[PublicAPI]
public class XboxHandler : AHandler<XboxGame, string>
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
    public override IEnumerable<Result<XboxGame>> FindAllGames()
    {
        var (paths, errors) = GetAppFolders(_fileSystem);
        foreach (var error in errors)
        {
            yield return Result.FromError<XboxGame>(error);
        }

        foreach (var path in paths)
        {
            if (!_fileSystem.DirectoryExists(path)) continue;
            var directories = _fileSystem.EnumerateDirectories(path, recursive: false);
            foreach (var directory in directories)
            {
                var appManifestFilePath = directory.CombineUnchecked("appmanifest.xml");
                if (!_fileSystem.FileExists(appManifestFilePath)) continue;

                var (game, error) = ParseAppManifest(_fileSystem, appManifestFilePath);
                if (game is not null)
                {
                    yield return Result.FromGame(game);
                } else if (error is not null)
                {
                    yield return Result.FromError<XboxGame>(error);
                }
            }
        }
    }

    /// <inheritdoc/>
    public override IDictionary<string, XboxGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.Id, game => game);
    }

    internal static (List<AbsolutePath> paths, List<string> errors) GetAppFolders(IFileSystem fileSystem)
    {
        var paths = new List<AbsolutePath>();
        var errors = new List<string>();

        var programFilesDirectory = fileSystem.GetKnownPath(KnownPath.ProgramFilesDirectory);
        if (programFilesDirectory != default)
            paths.Add(programFilesDirectory.CombineUnchecked("ModifiableWindowsApps"));

        foreach (var rootDirectory in fileSystem.EnumerateRootDirectories())
        {
            var gamingRootFilePath = rootDirectory.CombineUnchecked(".GamingRoot");

            if (!fileSystem.FileExists(gamingRootFilePath)) continue;
            var (additionalPaths, error) = ParseGamingRootFile(fileSystem, gamingRootFilePath);
            if (additionalPaths is not null)
            {
                paths.AddRange(additionalPaths);
            }
            else if (error is not null)
            {
                errors.Add(error);
            }
        }

        return (paths, errors);
    }

    internal static Result<XboxGame> ParseAppManifest(IFileSystem fileSystem,
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
                return Result.FromError<XboxGame>($"Unable to deserialize file {manifestFilePath}");
            }

            if (obj is not Package appManifest)
            {
                return Result.FromError<XboxGame>($"Deserialization of {manifestFilePath} failed: resulting object is not of type {typeof(Package)} but {obj.GetType()}");
            }

            var displayName = appManifest.Properties.DisplayName;
            var id = appManifest.Identity.Name;
            var game = new XboxGame(id, displayName, manifestFilePath.Parent);
            return Result.FromGame(game);
        }
        catch (Exception e)
        {
            return Result.FromException<XboxGame>($"Unable to parse manifest file {manifestFilePath}", e);
        }
    }

    internal static Result<List<AbsolutePath>> ParseGamingRootFile(
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
                return Result.FromError<List<AbsolutePath>>($"Unable to parse {gamingRootFilePath}, file magic does not match: expected {expectedMagic.ToString("x8", NumberFormatInfo.InvariantInfo)} got {magic.ToString("x8", NumberFormatInfo.InvariantInfo)}");
            }

            var folderCount = reader.ReadUInt32();
            if (folderCount >= byte.MaxValue)
            {
                return Result.FromError<List<AbsolutePath>>($"Folder count exceeds the limit: {folderCount}");
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

            return Result.FromGame(folders);
        }
        catch (Exception e)
        {
            return Result.FromException<List<AbsolutePath>>($"Unable to parse gaming root file {gamingRootFilePath}", e);
        }

    }
}
