using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Vogen;

namespace GameFinder.StoreHandlers.EGS;

/// <summary>
/// Represents an id for games installed with the Epic Games Store.
/// </summary>
[ValueObject<string>]
public readonly partial struct EGSGameId { }

/// <inheritdoc/>
[PublicAPI]
public class EGSGameIdComparer : IEqualityComparer<EGSGameId>
{
    private static EGSGameIdComparer? _default;

    /// <summary>
    /// Default equality comparer that uses <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </summary>
    public static EGSGameIdComparer Default => _default ??= new();

    private readonly StringComparison _stringComparison;

    /// <summary>
    /// Default constructor that uses <see cref="StringComparison.OrdinalIgnoreCase"/>.
    /// </summary>
    public EGSGameIdComparer() : this(StringComparison.OrdinalIgnoreCase) { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="stringComparison"></param>
    public EGSGameIdComparer(StringComparison stringComparison)
    {
        _stringComparison = stringComparison;
    }

    /// <inheritdoc/>
    public bool Equals(EGSGameId x, EGSGameId y) => string.Equals(x.Value, y.Value, _stringComparison);

    /// <inheritdoc/>
    public int GetHashCode(EGSGameId obj) => obj.Value.GetHashCode(_stringComparison);
}
