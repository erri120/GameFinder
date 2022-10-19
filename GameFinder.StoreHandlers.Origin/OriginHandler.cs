using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Web;
using JetBrains.Annotations;

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
public class OriginHandler
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

    /// <summary>
    /// Finds all games installed with Origin. This function will either return a non-null
    /// <see cref="OriginGame"/> or a non-null error.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<(OriginGame? game, string? error)> FindAllGames()
    {
        var manifestDir = _fileSystem.DirectoryInfo.FromDirectoryName(_fileSystem.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Origin",
            "LocalContent"
        ));

        if (!manifestDir.Exists)
        {
            yield return (null, $"Manifest folder {manifestDir.FullName} does not exist!");
            yield break;
        }

        var mfstFiles = manifestDir.EnumerateFiles("*.mfst", SearchOption.AllDirectories);
        foreach (var mfstFile in mfstFiles)
        {
            var (game, error) = ParseMfstFile(mfstFile);
            if (error is not null)
            {
                yield return (null, error);
                continue;
            }

            // ignore game
            if (game is null) continue;

            yield return (game, null);
        }
    }

    private (OriginGame? game, string? error) ParseMfstFile(IFileInfo fileInfo)
    {
        var contents = fileInfo.OpenText().ReadToEnd();
        var query = HttpUtility.ParseQueryString(contents, Encoding.UTF8);

        // using GetValues because some manifest have duplicate key-value entries for whatever reason
        var ids = query.GetValues("id");
        if (ids is null || ids.Length == 0)
        {
            return (null, $"Manifest {fileInfo.FullName} does not have a value \"id\"");
        }

        var id = ids.First();
        if (id.EndsWith("@steam", StringComparison.OrdinalIgnoreCase)) return (null, null);

        var installPaths = query.GetValues("dipInstallPath");
        if (installPaths is null || installPaths.Length == 0)
        {
            return (null, $"Manifest {fileInfo.FullName} does not have a value \"dipInstallPath\"");
        }
        
        return (new OriginGame(id, installPaths.First()), null);
    }
}
