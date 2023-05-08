using System.Diagnostics.CodeAnalysis;
using GameFinder.Wine;
using JetBrains.Annotations;
using NexusMods.Paths;

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
    public readonly AbsolutePath ProtonDirectory;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="protonDirectory"></param>
    /// <param name="configurationDirectory"></param>
    public ProtonWinePrefix(AbsolutePath protonDirectory, AbsolutePath configurationDirectory) : base(configurationDirectory)
    {
        ProtonDirectory = protonDirectory;
    }

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    protected override string GetUserName()
    {
        return "steamuser";
    }

    /// <summary>
    /// Returns the absolute path to the <c>config_info</c> file.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetConfigInfoFile()
    {
        return ProtonDirectory.CombineUnchecked("config_info");
    }

    /// <summary>
    /// Returns the absolute path to the <c>launch_command</c> file.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetLaunchCommandFile()
    {
        return ProtonDirectory.CombineUnchecked("launch_command");
    }

    /// <summary>
    /// Returns the absolute path to the <c>version</c> file.
    /// </summary>
    /// <returns></returns>
    public AbsolutePath GetVersionFile()
    {
        return ProtonDirectory.CombineUnchecked("version");
    }
}
