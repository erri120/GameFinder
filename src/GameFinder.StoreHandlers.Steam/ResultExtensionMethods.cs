using System;
using FluentResults;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam;

/// <summary>
/// Extension methods for <see cref="FluentResults"/>.
/// </summary>
[PublicAPI]
public static class ResultExtensionMethods
{
    public static TValue ValueOr<TValue>(this Result<TValue> result, Func<TValue> func)
    {
        return result.IsSuccess ? result.Value : func();
    }

    public static TValue ValueOr<TValue>(this Result<TValue> result, TValue other)
    {
        return result.IsSuccess ? result.Value : other;
    }
}
