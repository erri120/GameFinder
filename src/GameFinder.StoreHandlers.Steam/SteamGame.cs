using System.Globalization;
using System.IO.Abstractions;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam;

/// <summary>
/// Represents a game installed with Steam.
/// </summary>
/// <param name="AppId">ID of the game</param>
/// <param name="Name">Name of the game</param>
/// <param name="Path">Absolute path to the game installation folder</param>
[PublicAPI]
public record SteamGame(int AppId, string Name, string Path)
{
    /// <summary>
    /// Returns the absolute path of the manifest for this game.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public string GetManifestPath(IFileSystem? fs = null)
    {
        var manifestName = $"{AppId.ToString(CultureInfo.InvariantCulture)}.acf";

        return fs is null
            ? System.IO.Path.GetFullPath(System.IO.Path.Combine(Path, "..", "..", manifestName))
            : fs.Path.GetFullPath(fs.Path.Combine(Path, "..", "..", manifestName));
    }

    /// <summary>
    /// Returns the absolute path to the Wine prefix directory, managed by Proton.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetProtonPrefixDirectory(IPath? path = null)
    {
        return path switch
        {
            not null => path.GetFullPath(path.Combine(Path, "..", "..", "compatdata", AppId.ToString(CultureInfo.InvariantCulture))),
            null => System.IO.Path.GetFullPath(System.IO.Path.Combine(Path, "..", "..", "compatdata", AppId.ToString(CultureInfo.InvariantCulture)))
        };
    }
}
