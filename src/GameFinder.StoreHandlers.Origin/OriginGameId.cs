using System;
using System.Collections.Generic;
using GameFinder.Common;
using JetBrains.Annotations;
using TransparentValueObjects;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Represents an ID for games installed with Origin.
/// </summary>
[PublicAPI]
[ValueObject<string>]
public readonly partial struct OriginGameId : IId<OriginGameId>, IAugmentWith<DefaultEqualityComparerAugment>
{
    /// <inheritdoc/>
    public static IEqualityComparer<string> InnerValueDefaultEqualityComparer { get; } = StringComparer.OrdinalIgnoreCase;
}
