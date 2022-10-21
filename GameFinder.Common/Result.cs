using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a simple discriminated union. It will either contain a non-null result
/// or a non-null error message.
/// </summary>
/// <param name="Game"></param>
/// <param name="Error"></param>
/// <typeparam name="TGame"></typeparam>
[PublicAPI]
public readonly record struct Result<TGame>(TGame? Game, string? Error);
