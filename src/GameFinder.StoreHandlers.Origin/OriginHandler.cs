using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GameFinder.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Handler for finding games installed with Origin.
/// </summary>
/// <remarks>
/// This is the base class of <see cref="OriginHandler"/> which is probably
/// what you're looking for instead. This abstract class is only useful if you
/// want to extend the base functionality.
/// </remarks>
/// <seealso cref="OriginHandler"/>
[PublicAPI]
public abstract class OriginHandler<TGame> where TGame : class, IGame
{
    /// <summary>
    /// Logger.
    /// </summary>
    protected readonly ILogger Logger;

    /// <summary>
    /// Filesystem.
    /// </summary>
    protected readonly IFileSystem FileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    protected OriginHandler(
        ILoggerFactory loggerFactory,
        IFileSystem fileSystem)
    {
        Logger = loggerFactory.CreateLogger<OriginHandler<TGame>>();
        FileSystem = fileSystem;
    }

    [Pure]
    public IReadOnlyList<TGame> Search()
    {
        var manifestDir = ManifestLocator.GetManifestDirectory(FileSystem);
        if (!FileSystem.DirectoryExists(manifestDir))
        {
            LogMessages.MissingManifestDirectory(Logger, manifestDir);
            return Array.Empty<TGame>();
        }

        var manifestFiles = FileSystem.EnumerateFiles(manifestDir, "*.mfst").ToArray();
        if (manifestFiles.Length == 0)
        {
            LogMessages.NoManifestFiles(Logger, manifestDir);
            return Array.Empty<TGame>();
        }

        var games = new List<TGame>(capacity: manifestFiles.Length);

        foreach (var manifestFile in manifestFiles)
        {
            LogMessages.ParsingManifestFile(Logger, manifestFile);

            string contents;

            try
            {
                using var stream = FileSystem.ReadFile(manifestFile);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                contents = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                LogMessages.ExceptionWhileReadingManifest(Logger, e, manifestFile);
                continue;
            }

            try
            {
                var game = ParseManifestFile(contents, manifestFile);
                if (game is null) continue;

                LogMessages.ParsedManifestFile(Logger, manifestFile, game);
                games.Add(game);
            }
            catch (Exception e)
            {
                LogMessages.ExceptionWhileParsingManifest(Logger, e, manifestFile);
            }
        }

        return games;
    }

    /// <summary>
    /// Parses the given Manifest file into <typeparamref name="TGame"/>.
    /// </summary>
    public abstract TGame? ParseManifestFile(string contents, AbsolutePath manifestFile);
}

/// <summary>
/// Handler for finding games installed with Origin.
/// </summary>
[PublicAPI]
public class OriginHandler : OriginHandler<OriginGame>
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public OriginHandler(ILoggerFactory loggerFactory, IFileSystem fileSystem) : base(loggerFactory, fileSystem) { }

    /// <inheritdoc/>
    [Pure]
    public override OriginGame? ParseManifestFile(string contents, AbsolutePath manifestFile)
    {
        return ManifestParser.ParseManifestFile(Logger, FileSystem, contents, manifestFile);
    }
}
