using System;

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
