using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
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

        var id = query.Get("id");
        if (id is null || string.IsNullOrWhiteSpace(id))
        {
            return (null, $"Manifest {fileInfo.FullName} does not have a value \"id\"");
        }

        if (id.EndsWith("@steam", StringComparison.OrdinalIgnoreCase)) return (null, null);
        
        var installPath = query.Get("dipinstallpath");
        if (installPath is null || string.IsNullOrWhiteSpace(installPath))
        {
            return (null, $"Manifest {fileInfo.FullName} does not have a value \"dipinstallpath\"");
        }

        return (new OriginGame(id, installPath), null);
    }
}
