using System.Collections.Generic;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a handler that provides functionality for searching
/// for installed games.
/// </summary>
[PublicAPI]
public interface IHandler
{
    /// <summary>
    /// Searches for installed games.
    /// </summary>
    [MustUseReturnValue]
    IReadOnlyList<IGame> Search();
}

/// <summary>
/// Represents a generic handler.
/// </summary>
[PublicAPI]
public interface IHandler<out TGame> : IHandler
    where TGame : IGame
{
    /// <inheritdoc/>
    IReadOnlyList<IGame> IHandler.Search()
    {
        return (IReadOnlyList<IGame>)Search();
    }

    /// <summary>
    /// Searches for installed games.
    /// </summary>
    [MustUseReturnValue]
    new IReadOnlyList<TGame> Search();
}
