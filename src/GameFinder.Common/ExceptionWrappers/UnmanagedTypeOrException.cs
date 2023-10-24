using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents a discriminate union between an unmanaged type and an exception.
/// </summary>
/// <seealso cref="ValueTypeOrException{TValue}"/>
/// <seealso cref="ReferenceTypeOrException{TValue}"/>
[PublicAPI]
public readonly struct UnmanagedTypeOrException<TValue> : IValueOrException<TValue>
    where TValue : unmanaged
{
    private readonly TValue _value;
    private readonly Exception? _exception;
    private readonly bool _hasValue;

    /// <summary>
    /// Creates a new instance of this union with a value.
    /// </summary>
    public UnmanagedTypeOrException(TValue value)
    {
        _value = value;
        _hasValue = true;
    }

    /// <summary>
    /// Creates a new instance of this union with an exception.
    /// </summary>
    public UnmanagedTypeOrException(Exception exception)
    {
        _exception = exception;
        _hasValue = false;
    }

    /// <inheritdoc/>
    [Pure]
    [MustUseReturnValue]
    [StackTraceHidden]
    public TValue GetValue() => _hasValue
        ? _value
        : throw new InvalidOperationException($"This instance doesn't contain a value but an exception: \"{_exception}\"");

    /// <inheritdoc/>
    [Pure]
    [MustUseReturnValue]
    public bool TryGetValue(out TValue value)
    {
        value = _value;
        return _hasValue;
    }

    /// <inheritdoc/>
    [Pure]
    [MustUseReturnValue]
    [StackTraceHidden]
    public Exception GetException() => !_hasValue
        ? _exception!
        : throw new InvalidOperationException($"This instance doesn't contain an exception but a value: \"{_value}\"");

    /// <inheritdoc/>
    [ContractAnnotation("=> halt")]
    [StackTraceHidden]
    public void ThrowException()
    {
        if (!_hasValue)
            throw _exception!;
        throw new InvalidOperationException($"This instance doesn't contain an exception but a value: \"{_value}\"");
    }

    /// <inheritdoc/>
    [Pure]
    [MustUseReturnValue]
    public bool HasValue() => _hasValue;
}
