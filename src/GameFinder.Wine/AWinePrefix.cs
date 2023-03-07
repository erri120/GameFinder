using System.IO.Abstractions;
using JetBrains.Annotations;

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
    /// <param name="fs"></param>
    /// <returns></returns>
    public string GetVirtualDrivePath(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ConfigurationDirectory, "drive_c");
    }

    /// <summary>
    /// Returns the absolute path to the <c>system.reg</c> file of the prefix.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public string GetSystemRegistryFile(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ConfigurationDirectory, "system.reg");
    }

    /// <summary>
    /// Returns the absolute path to the <c>user.reg</c> file of the prefix.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    public string GetUserRegistryFile(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ConfigurationDirectory, "user.reg");
    }

    /// <summary>
    /// FS helper.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    protected static IFileSystem GetFs(IFileSystem? fs)
    {
        return fs switch
        {
            not null => fs,
            null => new FileSystem()
        };
    }
}
