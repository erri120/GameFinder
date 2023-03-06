using System.IO.Abstractions;
using JetBrains.Annotations;

namespace GameFinder.Wine;

/// <summary>
/// Abstract class for wine prefixes.
/// </summary>
[PublicAPI]
public abstract class AWinePrefix
{
    public readonly string ConfigurationDirectory;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="configurationDirectory"></param>
    protected AWinePrefix(string configurationDirectory)
    {
        ConfigurationDirectory = configurationDirectory;
    }

    /// <summary>
    /// Returns the absolute path to the virtual drive directory of the prefix.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetVirtualDrivePath(IPath? path = null)
    {
        return path switch
        {
            not null => path.Combine(ConfigurationDirectory, "drive_c"),
            null => System.IO.Path.Combine(ConfigurationDirectory, "drive_c")
        };
    }

    /// <summary>
    /// Returns the absolute path to the <c>system.reg</c> file of the prefix.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetSystemRegistryFile(IPath? path = null)
    {
        return path switch
        {
            not null => path.Combine(ConfigurationDirectory, "system.reg"),
            null => System.IO.Path.Combine(ConfigurationDirectory, "system.reg")
        };
    }

    /// <summary>
    /// Returns the absolute path to the <c>user.reg</c> file of the prefix.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public string GetUserRegistryFile(IPath? path = null)
    {
        return path switch
        {
            not null => path.Combine(ConfigurationDirectory, "user.reg"),
            null => System.IO.Path.Combine(ConfigurationDirectory, "user.reg")
        };
    }
}
