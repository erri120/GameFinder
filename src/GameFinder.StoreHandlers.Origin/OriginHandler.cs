using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Handler for finding games installed with Origin.
/// </summary>
[PublicAPI]
public class OriginHandler
{
    private readonly ILogger<OriginHandler> _logger;
    private readonly IFileSystem _fileSystem;

    /// <summary>
    /// Constructor.
    /// </summary>
    public OriginHandler(
        ILoggerFactory loggerFactory,
        IFileSystem fileSystem)
    {
        _logger = loggerFactory.CreateLogger<OriginHandler>();
        _fileSystem = fileSystem;
    }

    [Pure]
    public IReadOnlyList<OriginGame> Search()
    {
        var manifestDir = ManifestLocator.GetManifestDirectory(_fileSystem);
        if (!_fileSystem.DirectoryExists(manifestDir))
        {
            LogMessages.MissingManifestDirectory(_logger, manifestDir);
            return Array.Empty<OriginGame>();
        }

        var manifestFiles = _fileSystem.EnumerateFiles(manifestDir, "*.mfst").ToArray();
        if (manifestFiles.Length == 0)
        {
            LogMessages.NoManifestFiles(_logger, manifestDir);
            return Array.Empty<OriginGame>();
        }

        var games = new List<OriginGame>(capacity: manifestFiles.Length);

        foreach (var manifestFile in manifestFiles)
        {
            LogMessages.ParsingManifestFile(_logger, manifestFile);

            string contents;

            try
            {
                using var stream = _fileSystem.ReadFile(manifestFile);
                using var reader = new StreamReader(stream, Encoding.UTF8);
                contents = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                LogMessages.ExceptionWhileReadingManifest(_logger, e, manifestFile);
                continue;
            }

            try
            {
                var game = ManifestParser.ParseManifestFile(_logger, _fileSystem, contents, manifestFile);
                if (game is null) continue;

                LogMessages.ParsedManifestFile(_logger, manifestFile, game);
                games.Add(game);
            }
            catch (Exception e)
            {
                LogMessages.ExceptionWhileParsingManifest(_logger, e, manifestFile);
            }
        }

        return games;
    }
}
