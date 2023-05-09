using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using GameFinder.Common;
using JetBrains.Annotations;
using NexusMods.Paths;
using OneOf;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Handler for finding games install with Origin.
/// </summary>
[PublicAPI]
public class OriginHandler : AHandler<OriginGame, OriginGameId>
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="fileSystem"></param>
    public OriginHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    internal static AbsolutePath GetManifestDir(IFileSystem fileSystem)
    {
        return fileSystem.GetKnownPath(KnownPath.CommonApplicationDataDirectory)
            .CombineUnchecked("Origin")
            .CombineUnchecked("LocalContent");
    }

    /// <inheritdoc/>
    public override Func<OriginGame, OriginGameId> IdSelector => game => game.Id;

    /// <inheritdoc/>
    public override IEqualityComparer<OriginGameId> IdEqualityComparer => OriginGameIdComparer.Default;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<OriginGame, ErrorMessage>> FindAllGames()
    {
        var manifestDir = GetManifestDir(_fileSystem);

        if (!_fileSystem.DirectoryExists(manifestDir))
        {
            yield return new ErrorMessage($"Manifest folder {manifestDir} does not exist!");
            yield break;
        }

        var mfstFiles = _fileSystem.EnumerateFiles(manifestDir, "*.mfst").ToList();
        if (mfstFiles.Count == 0)
        {
            yield return new ErrorMessage($"Manifest folder {manifestDir} does not contain any .mfst files");
            yield break;
        }

        foreach (var mfstFile in mfstFiles)
        {
            var result = ParseMfstFile(mfstFile);

            // ignore steam games
            if (result.IsT2) continue;

            if (result.IsT1)
            {
                yield return result.AsT1;
                continue;
            }

            yield return result.AsT0;
        }
    }

    [SuppressMessage("ReSharper", "IdentifierTypo")]
    private OneOf<OriginGame, ErrorMessage, bool> ParseMfstFile(AbsolutePath filePath)
    {
        try
        {
            using var stream = _fileSystem.ReadFile(filePath);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var contents = reader.ReadToEnd();

            var query = HttpUtility.ParseQueryString(contents, Encoding.UTF8);

            // using GetValues because some manifest have duplicate key-value entries for whatever reason
            var ids = query.GetValues("id");
            if (ids is null || ids.Length == 0)
            {
                return new ErrorMessage($"Manifest {filePath} does not have a value \"id\"");
            }

            var id = ids[0];
            if (id.EndsWith("@steam", StringComparison.OrdinalIgnoreCase))
                return true;

            var installPaths = query.GetValues("dipInstallPath");
            if (installPaths is null || installPaths.Length == 0)
            {
                return new ErrorMessage($"Manifest {filePath} does not have a value \"dipInstallPath\"");
            }

            var path = installPaths
                .OrderByDescending(x => x.Length)
                .First();

            var game = new OriginGame(
                OriginGameId.From(id),
                _fileSystem.FromFullPath(SanitizeInputPath(path))
            );

            return game;
        }
        catch (Exception e)
        {
            return new ErrorMessage(e, $"Exception while parsing {filePath}");
        }
    }
}
