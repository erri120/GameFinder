using JetBrains.Annotations;
using NexusMods.Paths;

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
    public BottlesWinePrefix(AbsolutePath configurationDirectory) : base(configurationDirectory) { }

    /// <summary>
    /// Returns the absolute path to <c>bottle.yml</c>.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetBottlesConfigFile()
    {
        return ConfigurationDirectory.CombineUnchecked("bottle.yml");
    }
}
