using System.Globalization;
using JetBrains.Annotations;
using Vogen;

namespace GameFinder.StoreHandlers.Steam.Models.ValueTypes;

/// <summary>
/// Represents a 32-bit unsigned integer unique identifier for a depot.
/// </summary>
/// <example><c>262061</c></example>
[PublicAPI]
[ValueObject<uint>(conversions: Conversions.None)]
public readonly partial struct DepotId
{
    /// <summary>
    /// Gets the URL to the SteamDB page of this depot.
    /// </summary>
    /// <example><c>https://steamdb.info/depot/262061</c></example>
    public string SteamDbUrl => $"{Constants.SteamDbBaseUrl}/depot/{Value.ToString(CultureInfo.InvariantCulture)}";
}
