using System;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents an ID.
/// </summary>
/// <seealso cref="IId{TId}"/>
public interface IId : IEquatable<IId> { }

/// <summary>
/// Represents an ID.
/// </summary>
/// <seealso cref="IId"/>
public interface IId<TId> : IId, IEquatable<TId>
    where TId : IId<TId>
{
    /// <inheritdoc/>
    bool IEquatable<IId>.Equals(IId? other)
    {
        return other is IId<TId> same && Equals(same);
    }
}

/// <summary>
/// Represents a game with an ID.
/// </summary>
/// <seealso cref="IGame"/>
/// <seealso cref="IGameName"/>
[PublicAPI]
public interface IGameId<out TId> where TId : IId<TId>
{
    /// <summary>
    /// Gets the ID of the game.
    /// </summary>
    TId Id { get; }
}
