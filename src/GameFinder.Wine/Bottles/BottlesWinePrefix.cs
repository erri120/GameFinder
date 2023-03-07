using System.IO.Abstractions;
using JetBrains.Annotations;

namespace GameFinder.Wine.Bottles;

/// <summary>
/// Represents a Wineprefix managed by Bottles.
/// </summary>
[PublicAPI]
public class BottlesWinePrefix : AWinePrefix
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configurationDirectory"></param>
    public BottlesWinePrefix(string configurationDirectory) : base(configurationDirectory) { }

    /// <summary>
    /// Returns the absolute path to <c>bottle.yml</c>.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public string GetBottlesConfigFile(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ConfigurationDirectory, "bottle.yml");
    }
}
