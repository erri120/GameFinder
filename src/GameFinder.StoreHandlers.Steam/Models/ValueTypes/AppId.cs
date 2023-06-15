using System.Globalization;
using System.Web;
using JetBrains.Annotations;
using Vogen;

[assembly:VogenDefaults(debuggerAttributes: DebuggerAttributeGeneration.Basic)]

namespace GameFinder.StoreHandlers.Steam.Models.ValueTypes;

/// <summary>
/// Represents a 32-bit unsigned integer unique identifier of an app.
/// </summary>
/// <example><c>262060</c></example>
[PublicAPI]
[ValueObject<uint>]
public readonly partial struct AppId
{
    /// <summary>
    /// Empty or default value of <c>0</c>.
    /// </summary>
    public static readonly AppId Empty = From(0);

    /// <summary>
    /// Gets the URL to the Steam Store page of the app associated with this id.
    /// </summary>
    /// <example><c>https://store.steampowered.com/app/262060</c></example>
    /// <seealso cref="GetSteamStoreUrlWithTracking"/>
    public string SteamStoreUrl => $"{Constants.SteamStoreBaseUrl}/app/{Value.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Gets the URL to the SteamDB page of the app associated with this id.
    /// </summary>
    /// <example><c>https://steamdb.info/app/262060</c></example>
    public string SteamDbUrl => $"{Constants.SteamDbBaseUrl}/app/{Value.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Gets the URL to the Steam Store page of the app associated with this id and with additional UTM parameters.
    /// </summary>
    /// <remarks>
    /// Setting the UTM source parameter helps developers identify what links to their app.
    /// See https://partner.steamgames.com/doc/marketing/utm_analytics for more details.
    /// </remarks>
    /// <param name="source">The current source. This should be the name of your app.</param>
    /// <returns></returns>
    /// <example><c>https://store.steampowered.com/app/262060/?utm_source=MyApp</c></example>
    /// <seealso cref="SteamStoreUrl"/>
    public string GetSteamStoreUrlWithTracking(string source)
    {
        return $"{SteamStoreUrl}/?utm_source={HttpUtility.UrlEncode(source)}";
    }
}
