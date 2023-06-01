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
    /// Empty id.
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
    /// Universe of the account.
    /// </summary>
    public SteamUniverse Universe => (SteamUniverse)(int)(RawId >> 56);

    /// <summary>
    /// Account type.
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
    /// Account id.
    /// </summary>
    /// <example>149956546</example>
    public int AccountId => (int)(RawId << 32 >> 32);

    /// <summary>
    /// Account number.
    /// </summary>
    /// <example></example>
    public int AccountNumber => (int)(RawId << 32 >> 33);

    /// <summary>
    /// Textually representation in the Steam2 ID format.
    /// </summary>
    /// <example>STEAM_1:0:74978273</example>
    public string Steam2Id => $"STEAM_{((int)Universe).ToString(CultureInfo.InvariantCulture)}:{((int)(RawId << 63 >> 63)).ToString(CultureInfo.InvariantCulture)}:{AccountNumber.ToString(CultureInfo.InvariantCulture)}";

    /// <summary>
    /// Textually representation in the Steam3 ID format.
    /// </summary>
    /// <example>[U:1:149956546]</example>
    public string Steam3Id => $"[{AccountTypeLetter}:1:{AccountId.ToString(CultureInfo.InvariantCulture)}]";

    /// <summary>
    /// URL to the community profile of the account using <see cref="RawId"/>.
    /// </summary>
    /// <example>https://steamcommunity.com/profiles/76561198110222274</example>
    public string ProfileUrl => $"{Constants.SteamCommunityBaseUrl}/profiles/{RawId}";

    /// <summary>
    /// URL to the community profile of the account using the <see cref="Steam3Id"/>.
    /// </summary>
    /// <example>https://steamcommunity.com/profiles/[U:1:149956546]</example>
    public string Steam3IdProfileUrl => $"{Constants.SteamCommunityBaseUrl}/profiles/{Steam3Id}";

    /// <inheritdoc/>
    public override string ToString() => Steam3Id;
}
