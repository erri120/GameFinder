using GameFinder.Common;
using JetBrains.Annotations;
using TransparentValueObjects;

namespace GameFinder.StoreHandlers.GOG;

/// <summary>
/// Represents an ID for games installed with GOG Galaxy.
/// </summary>
[PublicAPI]
[ValueObject<ulong>]
public readonly partial struct GOGGameId : IId
{
    /// <inheritdoc/>
    public bool Equals(IId? other) => other is GOGGameId same && Equals(same);
}
