using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam.Models;

/// <summary>
/// Represents a single library folder.
/// </summary>
[PublicAPI]
public sealed record LibraryFolder
{
    /// <summary>
    /// Gets the absolute path to the library folder.
    /// </summary>
    public required AbsolutePath Path { get; init; }

    /// <summary>
    /// Gets the label of the library folder.
    /// </summary>
    /// <remarks>This value can be <see cref="string.Empty"/>.</remarks>
    public string Label { get; init; } = string.Empty;

    /// <summary>
    /// Gets the combined size of all installed apps inside the library folder.
    /// </summary>
    public Size TotalSize { get; init; } = Size.Zero;

    /// <summary>
    /// Gets all installed apps inside the library folders and their sizes.
    /// </summary>
    public IReadOnlyDictionary<AppId, Size> AppSizes { get; init; } = ImmutableDictionary<AppId, Size>.Empty;

    #region Overrides

    /// <inheritdoc/>
    public bool Equals(LibraryFolder? other)
    {
        if (other is null) return false;
        if (!Path.Equals(other.Path)) return false;
        if (!Label.Equals(other.Label, StringComparison.Ordinal)) return false;
        if (!TotalSize.Equals(other.TotalSize)) return false;
        if (!AppSizes.SequenceEqual(other.AppSizes)) return false;
        return true;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Path);
        hashCode.Add(Label);
        hashCode.Add(TotalSize);
        hashCode.Add(AppSizes);
        return hashCode.ToHashCode();
    }

    #endregion
}
