using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.Wine;

/// <summary>
/// Abstract class for wine prefixes.
/// </summary>
[PublicAPI]
public abstract class AWinePrefix
{
    /// <summary>
    /// Absolute path to the Wine prefix directory.
    /// </summary>
    public readonly AbsolutePath ConfigurationDirectory;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configurationDirectory"></param>
    protected AWinePrefix(AbsolutePath configurationDirectory)
    {
        ConfigurationDirectory = configurationDirectory;
    }

    /// <summary>
    /// Returns the absolute path to the virtual drive directory of the prefix.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetVirtualDrivePath()
    {
        return ConfigurationDirectory.CombineUnchecked("drive_c");
    }

    /// <summary>
    /// Returns the absolute path to the <c>system.reg</c> file of the prefix.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetSystemRegistryFile()
    {
        return ConfigurationDirectory.CombineUnchecked("system.reg");
    }

    /// <summary>
    /// Returns the absolute path to the <c>user.reg</c> file of the prefix.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetUserRegistryFile()
    {
        return ConfigurationDirectory.CombineUnchecked("user.reg");
    }
}
