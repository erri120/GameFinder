using System;
using System.Text;
using FluentResults;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using GameFinder.StoreHandlers.Steam.Services;
using JetBrains.Annotations;
using NexusMods.Paths;
using NexusMods.Paths.Extensions;

namespace GameFinder.StoreHandlers.Steam.Models;

/// <summary>
/// Represents a parsed registry entry.
/// </summary>
[PublicAPI]
public sealed record RegistryEntry
{
    /// <summary>
    /// Gets the unique identifier of the app
    /// that was parsed to produce this <see cref="RegistryEntry"/>.
    /// </summary>
    public required AppId AppId { get; init; }

    #region Parsed Values

    /// <summary>
    /// Gets the <see cref="IRegistryKey"/> for the Uninstall registry subkey.
    /// </summary>
    public required IRegistryKey RegistryPath { get; init; }

    /// <summary>
    /// Gets the path to the icon for this app.
    /// </summary>
    public AbsolutePath? DisplayIcon { get; init; }

    /// <summary>
    /// Gets name of the app.
    /// </summary>
    public required string DisplayName { get; init; }

    /// <summary>
    /// Gets the help URL (invariably https://help.steampowered.com/)
    /// </summary>
    public required string HelpLink { get; init; }

    /// <summary>
    /// Gets the installation directory of the app.
    /// </summary>
    public required AbsolutePath? InstallLocation { get; init; }

    /// <summary>
    /// Gets the publisher name
    /// </summary>
    public required string Publisher { get; init; }

    /// <summary>
    /// Gets the uninstall executable
    /// </summary>
    /// <example><c>"C:\Program Files\Steam\steam.exe"</c></example>
    public required AbsolutePath? UninstallExecutable { get; init; }

    /// <summary>
    /// Gets the uninstall parameters (note the steam:// URL by itself without the executable should be sufficient)
    /// </summary>
    /// <example><c>steam://uninstall/262060</c></example>
    public required string UninstallParameters { get; init; }

    /// <summary>
    /// Gets the info URL
    /// </summary>
    public required string URLInfoAbout { get; init; }

    #endregion

    #region Helpers

    private static readonly RelativePath CommonDirectoryName = "common".ToRelativePath();
    private static readonly RelativePath ShaderCacheDirectoryName = "shadercache".ToRelativePath();
    private static readonly RelativePath WorkshopDirectoryName = "workshop".ToRelativePath();
    private static readonly RelativePath CompatabilityDataDirectoryName = "compatdata".ToRelativePath();

    /// <summary>
    /// Parses the registry for <see cref="AppId"/> again and returns a new
    /// instance of <see cref="RegistryEntry"/>.
    /// </summary>
    [Pure]
    [System.Diagnostics.Contracts.Pure]
    [MustUseReturnValue]
    public Result<RegistryEntry> Reload(IFileSystem fileSystem, IRegistry? registry)
    {
        return RegistryEntryParser.ParseRegistryEntry(AppId, fileSystem, registry);
    }

    #endregion

    #region Overwrites

    /// <inheritdoc/>
    public bool Equals(RegistryEntry? other)
    {
        if (other is null) return false;
        if (AppId != other.AppId) return false;
        if (DisplayIcon != other.DisplayIcon) return false;
        if (!string.Equals(DisplayName, other.DisplayName, StringComparison.Ordinal)) return false;
        if (!string.Equals(HelpLink, other.HelpLink, StringComparison.Ordinal)) return false;
        if (InstallLocation != other.InstallLocation) return false;
        if (!string.Equals(Publisher, other.Publisher, StringComparison.Ordinal)) return false;
        if (UninstallExecutable != other.UninstallExecutable) return false;
        if (!string.Equals(UninstallParameters, other.UninstallParameters, StringComparison.Ordinal)) return false;
        if (!string.Equals(URLInfoAbout, other.URLInfoAbout, StringComparison.Ordinal)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(AppId);
        hashCode.Add(DisplayIcon);
        hashCode.Add(DisplayName);
        hashCode.Add(HelpLink);
        hashCode.Add(InstallLocation);
        hashCode.Add(Publisher);
        hashCode.Add(UninstallExecutable);
        hashCode.Add(UninstallParameters);
        hashCode.Add(URLInfoAbout);
        return hashCode.ToHashCode();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append("{ ");
        sb.Append($"DisplayIcon = {DisplayIcon}, ");
        sb.Append($"Uninstall = {UninstallExecutable} {UninstallParameters}");
        sb.Append(" }");

        return sb.ToString();
    }

    #endregion
}
