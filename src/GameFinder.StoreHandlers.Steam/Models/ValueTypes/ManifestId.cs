using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Vogen;

namespace GameFinder.StoreHandlers.Steam.Models.ValueTypes;

/// <summary>
/// Represents a unique identifier for a manifest of a depot change.
/// </summary>
/// <remarks>
/// Although the value is a string and that looks like a 64-bit unsigned
/// integer, Steam actually treats the manifest ID as a string.
/// See https://github.com/SteamDatabase/ValveKeyValue/pull/47#issuecomment-984605893
/// for more information.
/// </remarks>
/// <example><c>"5542773349944116172"</c></example>
[PublicAPI]
[ValueObject<string>(conversions: Conversions.None)]
public readonly partial struct ManifestId
{
    /// <summary>
    /// Empty or default value of <c>0</c>.
    /// </summary>
    public static readonly ManifestId Empty = From("0");

    /// <summary>
    /// Gets the URL to the SteamDB page for the Changeset of the manifest associated with this id.
    /// </summary>
    /// <param name="depotId">ID of the depot this manifest ID is a part of.</param>
    /// <returns></returns>
    /// <example><c>https://steamdb.info/depot/262061/history/?changeid=M:5542773349944116172</c></example>
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public string GetSteamDbChangesetUrl(DepotId depotId)
    {
        return $"{depotId.SteamDbUrl}/history/?changeid=M:{Value}";
    }
}
