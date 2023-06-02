using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using JetBrains.Annotations;
using Vogen;

namespace GameFinder.StoreHandlers.Steam.Models.ValueTypes;

/// <summary>
/// Represents a 64-bit unsigned integer unique identifier for a manifest of a depot change.
/// </summary>
/// <example><c>5542773349944116172</c></example>
[PublicAPI]
[ValueObject<ulong>(conversions: Conversions.None)]
public readonly partial struct ManifestId
{
    /// <summary>
    /// Empty or default value of <c>0</c>.
    /// </summary>
    public static readonly ManifestId Empty = From(0);

    /// <summary>
    /// Gets the URL to the SteamDB page for the Changeset of the manifest associated with this id.
    /// </summary>
    /// <param name="depotId">ID of the depot this manifest ID is a part of.</param>
    /// <returns></returns>
    /// <example><c>https://steamdb.info/depot/262061/history/?changeid=M:5542773349944116172</c></example>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public string GetSteamDbChangesetUrl(DepotId depotId)
    {
        return $"{depotId.SteamDbUrl}/history/?changeid=M:{Value.ToString(CultureInfo.InvariantCulture)}";
    }
}
