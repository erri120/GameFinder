using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Marker interface for records that represent found games.
/// </summary>
[PublicAPI]
public interface IGame
{
    // TODO: consider having a "Directory" property or similar
}

// TODO: consider having a IGame<TId> generic interface with an "TId Id property"
