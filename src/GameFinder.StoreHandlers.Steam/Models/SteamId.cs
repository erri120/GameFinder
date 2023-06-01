using System.Globalization;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam.Models;

/// <summary>
/// Unique identifier used to identify a Steam account.
/// </summary>
/// <remarks>
/// See https://developer.valvesoftware.com/wiki/SteamID for more information.
/// </remarks>
[PublicAPI]
public readonly struct SteamId
{
    /// <summary>
    /// Represents an empty ID of an invalid user.
    /// </summary>
    public static readonly SteamId Empty = new(0);

    /// <summary>
    /// Compressed binary representation of the id.
    /// </summary>
    /// <example>76561198110222274</example>
    public readonly ulong RawId;

    /// <summary>
    /// Constructor using the compressed binary representation of the id.
    /// </summary>
    /// <param name="rawId">The raw 64-bit unique identifier.</param>
    public SteamId(ulong rawId)
    {
        RawId = rawId;
    }

    /// <summary>
    /// Gets the universe of the account.
    /// </summary>
    public SteamUniverse Universe => (SteamUniverse)(int)(RawId >> 56);

    /// <summary>
    /// Gets the account type.
    /// </summary>
    public SteamAccountType AccountType => (SteamAccountType)((byte)(RawId >> 52) & 0xF);

    /// <summary>
    /// Gets the account type letter for the current <see cref="AccountType"/>.
    /// </summary>
    public char AccountTypeLetter => AccountType switch
    {
        SteamAccountType.Invalid => 'I',
        SteamAccountType.Individual => 'U',
        SteamAccountType.Multiseat => 'M',
        SteamAccountType.GameServer => 'G',
        SteamAccountType.AnonGameServer => 'A',
        SteamAccountType.Pending => 'P',
        SteamAccountType.ContentServer => 'C',
        SteamAccountType.Clan => 'g',
        SteamAccountType.Chat => 'T',
        SteamAccountType.AnonUser => 'a',
        _ => '?'
    };

    /// <summary>
    /// Gets the account identifier.
    /// </summary>
    /// <remarks>
    /// This identifier can be used to get the current user data in the Steam installation directory.
    /// It's also used by <see cref="Steam3Id"/>.
    /// </remarks>
    /// <example>149956546</example>
    public int AccountId => (int)(RawId << 32 >> 32);

    /// <summary>
    /// Gets the account number.
    /// </summary>
    /// <remarks>
    /// This is only useful for <see cref="Steam2Id"/>.
    /// </remarks>
    /// <example>74978273</example>
    public int AccountNumber => (int)(RawId << 32 >> 33);

    /// <summary>
    /// Gets the textually representation in the Steam2 ID format.
    /// </summary>
    /// <example>STEAM_1:0:74978273</example>
    /// <seealso cref="Steam3Id"/>
    public string Steam2Id => $"STEAM_{((int)Universe).ToString(CultureInfo.InvariantCulture)}:{((int)(RawId << 63 >> 63)).ToString(CultureInfo.InvariantCulture)}:{AccountNumber.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Gets the textually representation in the Steam3 ID format.
    /// </summary>
    /// <example>[U:1:149956546]</example>
    /// <seealso cref="Steam2Id"/>
    public string Steam3Id => $"[{AccountTypeLetter}:1:{AccountId.ToString(CultureInfo.InvariantCulture)}]";

    /// <summary>
    /// Gets the URL to the community profile page of the account using <see cref="RawId"/>.
    /// </summary>
    /// <example>https://steamcommunity.com/profiles/76561198110222274</example>
    public string ProfileUrl => $"{Constants.SteamCommunityBaseUrl}/profiles/{RawId}";

    /// <summary>
    /// Gets the URL to the community profile page of the account using the <see cref="Steam3Id"/>.
    /// </summary>
    /// <example>https://steamcommunity.com/profiles/[U:1:149956546]</example>
    public string Steam3IdProfileUrl => $"{Constants.SteamCommunityBaseUrl}/profiles/{Steam3Id}";

    /// <inheritdoc/>
    public override string ToString() => Steam3Id;
}
