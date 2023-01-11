using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Web;
using GameFinder.Common;
using JetBrains.Annotations;
using Result = GameFinder.Common.Result<GameFinder.StoreHandlers.Origin.OriginGame>;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Represents a game installed with Origin.
/// </summary>
/// <param name="Id"></param>
/// <param name="InstallPath"></param>
[PublicAPI]
public record OriginGame(string Id, string InstallPath);

/// <summary>
/// Handler for finding games install with Origin.
/// </summary>
[PublicAPI]
public class OriginHandler : AHandler<OriginGame, string>
{
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Default constructor that uses the real filesystem <see cref="FileSystem"/>.
    /// </summary>
    public OriginHandler() : this(new FileSystem()) { }

    /// <summary>
    /// Constructor for specifying the <see cref="IFileSystem"/> implementation to use.
    /// </summary>
    /// <param name="fileSystem"></param>
    public OriginHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    internal static IDirectoryInfo GetManifestDir(IFileSystem fileSystem)
    {
        return fileSystem.DirectoryInfo.New(fileSystem.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Origin",
            "LocalContent"
        ));
    }

    /// <inheritdoc/>
    public override IEnumerable<Result> FindAllGames()
    {
        var manifestDir = GetManifestDir(_fileSystem);

        if (!manifestDir.Exists)
        {
            yield return new Result(null, $"Manifest folder {manifestDir.FullName} does not exist!");
            yield break;
        }

        var mfstFiles = manifestDir.EnumerateFiles("*.mfst", SearchOption.AllDirectories).ToList();
        if (mfstFiles.Count == 0)
        {
            yield return new Result(null,$"Manifest folder {manifestDir.FullName} does not contain any .mfst files");
            yield break;
        }

        foreach (var mfstFile in mfstFiles)
        {
            var (game, error) = ParseMfstFile(mfstFile);
            if (error is not null)
            {
                yield return new Result(null, error);
                continue;
            }

            // ignored game
            if (game is null) continue;

            yield return new Result(game, null);
        }
    }

    /// <inheritdoc/>
    public override IDictionary<string, OriginGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.Id, game => game, StringComparer.OrdinalIgnoreCase);
    }

    private static Result ParseMfstFile(IFileInfo fileInfo)
    {
        try
        {
            var contents = fileInfo.OpenText().ReadToEnd();
            var query = HttpUtility.ParseQueryString(contents, Encoding.UTF8);

            // using GetValues because some manifest have duplicate key-value entries for whatever reason
            var ids = query.GetValues("id");
            if (ids is null || ids.Length == 0)
            {
                return new Result(null,$"Manifest {fileInfo.FullName} does not have a value \"id\"");
            }

            var id = ids.First();
            if (id.EndsWith("@steam", StringComparison.OrdinalIgnoreCase))
                return new Result(null, null);

            var installPaths = query.GetValues("dipInstallPath");
            if (installPaths is null || installPaths.Length == 0)
            {
                return new Result(null, $"Manifest {fileInfo.FullName} does not have a value \"dipInstallPath\"");
            }

            return new Result(new OriginGame(id, installPaths.First()), null);
        }
        catch (Exception e)
        {
            return new Result(null, $"Exception while parsing {fileInfo.FullName}\n{e}");
        }
    }
}
