using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Interface for types that represents a discriminate union between a type and an exception.
/// </summary>
/// <remarks>
/// The implementations of this interface that the library offers are all value types. As such,
/// using the interface instead of an implementation will result in boxing of the value types.
/// </remarks>
/// <seealso cref="ReferenceTypeOrException{TValue}"/>
/// <seealso cref="UnmanagedTypeOrException{TValue}"/>
/// <seealso cref="ValueTypeOrException{TValue}"/>
[PublicAPI]
public interface IValueOrException<TValue>
{
    /// <summary>
    /// Gets the stored value or throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>The stored value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>false</c>.</exception>
    [Pure]
    [MustUseReturnValue]
    public TValue? GetValue();

    /// <summary>
    /// Tries to get the stored value and returns <c>true</c> if successful.
    /// </summary>
    [Pure]
    [MustUseReturnValue]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value);

    /// <summary>
    /// Gets the stored exception or throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>The stored exception.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>true</c>.</exception>
    [Pure]
    [MustUseReturnValue]
    public Exception GetException();

    /// <summary>
    /// Throws the stored exception or throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <exception cref="Exception">Thrown if <see cref="HasValue"/> is <c>false</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>true</c>.</exception>
    [ContractAnnotation("=> halt")]
    public void ThrowException();

    /// <summary>
    /// Gets whether this instance stores a value or an exception.
    /// </summary>
    /// <returns><c>true</c> if this instance stores a value, otherwise <c>false</c>.</returns>
    [Pure]
    [MustUseReturnValue]
    public bool HasValue();
}
