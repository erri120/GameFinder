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
public readonly struct ReferenceTypeOrException<TValue> : IValueOrException<TValue>
    where TValue : class
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

    /// <inheritdoc/>
    [Pure]
    [MustUseReturnValue]
    [StackTraceHidden]
    public TValue? GetValue() => _hasValue
        ? _value
        : throw new InvalidOperationException($"This instance doesn't contain a value but an exception: \"{_exception}\"");

    /// <inheritdoc/>
    [Pure]
    [MustUseReturnValue]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value)
    {
        value = _value;
        return _value is not null && _hasValue;
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

