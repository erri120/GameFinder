using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a simple discriminated union. It will either contain a non-null result
/// or a non-null error message.
/// </summary>
/// <typeparam name="TGame"></typeparam>
[PublicAPI]
[DebuggerDisplay("Game = {Game}, Error = {Error}")]
public readonly struct Result<TGame> where TGame : class
{
    /// <summary>
    /// A non-null <typeparamref name="TGame"/> if <see cref="Error"/> is null, otherwise
    /// this value is null.
    /// </summary>
    public readonly TGame? Game;

    /// <summary>
    /// A non-null error message if <see cref="Game"/> is null, otherwise this value is
    /// null.
    /// </summary>
    public readonly string? Error;

    internal Result(TGame? game, string? error)
    {
        Game = game;
        Error = error;
    }

    /// <summary>
    /// Deconstructs the struct into <see cref="Game"/> and <see cref="Error"/>.
    /// </summary>
    /// <param name="game"></param>
    /// <param name="error"></param>
    public void Deconstruct(out TGame? game, out string? error)
    {
        game = Game;
        error = Error;
    }
}

/// <summary>
/// Static helper class containing factory methods for <see cref="Result{TGame}"/>.
/// </summary>
[PublicAPI]
public static class Result
{
    /// <summary>
    /// Factory method that creates a new <see cref="Result{TGame}"/> from the provided
    /// <paramref name="game"/>.
    /// </summary>
    /// <param name="game"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static Result<TGame> FromGame<TGame>(TGame game) where TGame : class
    {
        return new Result<TGame>(game, error: null);
    }

    /// <summary>
    /// Factory method that creates a new <see cref="Result{TGame}"/> from the provided
    /// <paramref name="error"/>.
    /// </summary>
    /// <param name="error"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static Result<TGame> FromError<TGame>(string error) where TGame : class
    {
        return new Result<TGame>(game: null, error);
    }

    /// <summary>
    /// Factory method that creates a new <see cref="Result{TGame}"/> from the provided
    /// <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static Result<TGame> FromException<TGame>(Exception exception) where TGame : class
    {
        return new Result<TGame>(game: null, exception.ToString());
    }

    /// <summary>
    /// Factory method that creates a new <see cref="Result{TGame}"/> from the provided
    /// <paramref name="msg"/> and <paramref name="exception"/>.
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="exception"></param>
    /// <typeparam name="TGame"></typeparam>
    /// <returns></returns>
    public static Result<TGame> FromException<TGame>(string msg, Exception exception) where TGame : class
    {
        return new Result<TGame>(game: null, $"{msg}:\n{exception}");
    }
}
