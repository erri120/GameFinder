using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using GameFinder.Wine;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam;

/// <summary>
/// Represents a Wine prefix managed by Valve's Proton library.
/// </summary>
[PublicAPI]
public class ProtonWinePrefix : AWinePrefix
{
    /// <summary>
    /// This is the parent directory of <see cref="AWinePrefix.ConfigurationDirectory"/>.
    /// This directory mostly contains metadata files created by Proton, the actual Wine
    /// prefix is the sub directory <c>pfx</c>, which you can access using
    /// <see cref="AWinePrefix.ConfigurationDirectory"/>.
    /// </summary>
    public readonly string ProtonDirectory;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="protonDirectory"></param>
    /// <param name="configurationDirectory"></param>
    public ProtonWinePrefix(string protonDirectory, string configurationDirectory) : base(configurationDirectory)
    {
        ProtonDirectory = protonDirectory;
    }

    /// <summary>
    /// Returns the absolute path to the <c>config_info</c> file.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    [SuppressMessage("", "IO0006")]
    public string GetConfigInfoFile(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ProtonDirectory, "config_info");
    }

    /// <summary>
    /// Returns the absolute path to the <c>launch_command</c> file.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    [SuppressMessage("", "IO0006")]
    public string GetLaunchCommandFile(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ProtonDirectory, "launch_command");
    }

    /// <summary>
    /// Returns the absolute path to the <c>version</c> file.
    /// </summary>
    /// <param name="fs"></param>
    /// <returns></returns>
    [SuppressMessage("", "IO0006")]
    public string GetVersionFile(IFileSystem? fs = null)
    {
        return GetFs(fs).Path.Combine(ProtonDirectory, "version");
    }
}
