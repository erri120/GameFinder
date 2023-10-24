using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a discriminate union between a reference type and an exception.
/// </summary>
/// <seealso cref="UnmanagedTypeOrException{TValue}"/>
/// <seealso cref="ValueTypeOrException{TValue}"/>
[PublicAPI]
public readonly struct ReferenceTypeOrException<TValue> where TValue : class
{
    private readonly TValue? _value;
    private readonly Exception? _exception;
    private readonly bool _hasValue;

    /// <summary>
    /// Creates a new instance of this union with a value.
    /// </summary>
    public ReferenceTypeOrException(TValue? value)
    {
        _value = value;
        _hasValue = true;
    }

    /// <summary>
    /// Creates a new instance of this union with an exception.
    /// </summary>
    public ReferenceTypeOrException(Exception exception)
    {
        _exception = exception;
        _hasValue = false;
    }

    /// <summary>
    /// Gets the stored value or throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>The stored value.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>false</c>.</exception>
    [Pure]
    [MustUseReturnValue]
    [StackTraceHidden]
    public TValue? GetValue() => _hasValue
        ? _value
        : throw new InvalidOperationException($"This instance doesn't contain a value but an exception: \"{_exception}\"");

    /// <summary>
    /// Tries to get the stored value and returns <c>true</c> if successful.
    /// </summary>
    [Pure]
    [MustUseReturnValue]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        value = _value;
        return _value is not null && _hasValue;
    }

    /// <summary>
    /// Gets the stored exception or throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <returns>The stored exception.</returns>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>true</c>.</exception>
    [Pure]
    [MustUseReturnValue]
    [StackTraceHidden]
    public Exception GetException() => !_hasValue
        ? _exception!
        : throw new InvalidOperationException($"This instance doesn't contain an exception but a value: \"{_value}\"");

    /// <summary>
    /// Throws the stored exception or throws an <see cref="InvalidOperationException"/>.
    /// </summary>
    /// <exception cref="Exception">Thrown if <see cref="HasValue"/> is <c>false</c>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasValue"/> is <c>true</c>.</exception>
    [ContractAnnotation("=> halt")]
    [StackTraceHidden]
    public void ThrowException()
    {
        if (!_hasValue)
            throw _exception!;
        throw new InvalidOperationException($"This instance doesn't contain an exception but a value: \"{_value}\"");
    }

    /// <summary>
    /// Gets whether this instance stores a value or an exception.
    /// </summary>
    /// <returns><c>true</c> if this instance stores a value, otherwise <c>false</c>.</returns>
    [Pure]
    [MustUseReturnValue]
    public bool HasValue() => _hasValue;
}

