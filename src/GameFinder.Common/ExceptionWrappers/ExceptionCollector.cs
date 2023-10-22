using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace GameFinder.Common;

/// <summary>
/// Represents an exception collector.
/// </summary>
[PublicAPI]
public sealed class ExceptionCollector
{
    private readonly List<Exception> _exceptions = new();

    /// <summary>
    /// Checks whether this instance contains any exceptions.
    /// </summary>
    /// <returns><c>true</c> if this instance contains any exceptions.</returns>
    /// <seealso cref="GetExceptions"/>
    [Pure]
    [MustUseReturnValue]
    public bool HasExceptions() => _exceptions.Count != 0;

    /// <summary>
    /// Returns all collected exceptions in insertion order.
    /// </summary>
    /// <seealso cref="HasExceptions"/>
    [Pure]
    [MustUseReturnValue]
    public IReadOnlyList<Exception> GetExceptions() => _exceptions;

    /// <summary>
    /// Throws all collected exceptions using <see cref="ExceptionCollectorAggregatedException"/>.
    /// </summary>
    /// <exception cref="ExceptionCollectorAggregatedException"></exception>
    [ContractAnnotation("=> halt")]
    public void ThrowAllExceptions()
    {
        throw ExceptionCollectorAggregatedException.Create(_exceptions);
    }

    /// <summary>
    /// Calls the given <paramref name="operation"/> and returns a new
    /// instance of <see cref="ReferenceTypeOrException{TValue}"/> that
    /// contains either the result of <paramref name="operation"/> or
    /// an exception thrown during execution.
    /// </summary>
    /// <param name="operation">The operation to call.</param>
    /// <typeparam name="TValue">The result type of <paramref name="operation"/>.</typeparam>
    [StackTraceHidden]
    public ReferenceTypeOrException<TValue> WrapReferenceType<TValue>(
        [InstantHandle] Func<TValue> operation) where TValue : class
    {
        try
        {
            return new ReferenceTypeOrException<TValue>(operation());
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
            return new ReferenceTypeOrException<TValue>(e);
        }
    }

    /// <summary>
    /// Calls the given <paramref name="operation"/> with the provided
    /// <paramref name="state"/> and returns a new
    /// instance of <see cref="ReferenceTypeOrException{TValue}"/> that
    /// contains either the result of <paramref name="operation"/> or
    /// an exception thrown during execution.
    /// </summary>
    /// <param name="state">The state to be passed to <paramref name="operation"/>.</param>
    /// <param name="operation">The operation to call.</param>
    /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
    /// <typeparam name="TValue">The result type of <paramref name="operation"/>.</typeparam>
    [StackTraceHidden]
    public ReferenceTypeOrException<TValue> WrapReferenceType<TState, TValue>(
        TState state, [InstantHandle] Func<TState, TValue> operation) where TValue : class
    {
        try
        {
            return new ReferenceTypeOrException<TValue>(operation(state));
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
            return new ReferenceTypeOrException<TValue>(e);
        }
    }

    /// <summary>
    /// Calls the given <paramref name="operation"/> and returns a new
    /// instance of <see cref="ValueTypeOrException{TValue}"/> that
    /// contains either the result of <paramref name="operation"/> or
    /// an exception thrown during execution.
    /// </summary>
    /// <param name="operation">The operation to call.</param>
    /// <typeparam name="TValue">The result type of <paramref name="operation"/>.</typeparam>
    [StackTraceHidden]
    public ValueTypeOrException<TValue> WrapValueType<TValue>(
        [InstantHandle] Func<TValue> operation) where TValue : struct
    {
        try
        {
            return new ValueTypeOrException<TValue>(operation());
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
            return new ValueTypeOrException<TValue>(e);
        }
    }

    /// <summary>
    /// Calls the given <paramref name="operation"/> with the provided
    /// <paramref name="state"/> and returns a new
    /// instance of <see cref="ValueTypeOrException{TValue}"/> that
    /// contains either the result of <paramref name="operation"/> or
    /// an exception thrown during execution.
    /// </summary>
    /// <param name="state">The state to be passed to <paramref name="operation"/>.</param>
    /// <param name="operation">The operation to call.</param>
    /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
    /// <typeparam name="TValue">The result type of <paramref name="operation"/>.</typeparam>
    [StackTraceHidden]
    public ValueTypeOrException<TValue> WrapValueType<TState, TValue>(
        TState state, [InstantHandle] Func<TState, TValue> operation) where TValue : struct
    {
        try
        {
            return new ValueTypeOrException<TValue>(operation(state));
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
            return new ValueTypeOrException<TValue>(e);
        }
    }

    /// <summary>
    /// Calls the given <paramref name="operation"/> and returns a new
    /// instance of <see cref="UnmanagedTypeOrException{TValue}"/> that
    /// contains either the result of <paramref name="operation"/> or
    /// an exception thrown during execution.
    /// </summary>
    /// <param name="operation">The operation to call.</param>
    /// <typeparam name="TValue">The result type of <paramref name="operation"/>.</typeparam>
    [StackTraceHidden]
    public UnmanagedTypeOrException<TValue> WrapUnmanagedType<TValue>(
        [InstantHandle] Func<TValue> operation) where TValue : unmanaged
    {
        try
        {
            return new UnmanagedTypeOrException<TValue>(operation());
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
            return new UnmanagedTypeOrException<TValue>(e);
        }
    }

    /// <summary>
    /// Calls the given <paramref name="operation"/> with the provided
    /// <paramref name="state"/> and returns a new
    /// instance of <see cref="UnmanagedTypeOrException{TValue}"/> that
    /// contains either the result of <paramref name="operation"/> or
    /// an exception thrown during execution.
    /// </summary>
    /// <param name="state">The state to be passed to <paramref name="operation"/>.</param>
    /// <param name="operation">The operation to call.</param>
    /// <typeparam name="TState">The type of <paramref name="state"/>.</typeparam>
    /// <typeparam name="TValue">The result type of <paramref name="operation"/>.</typeparam>
    [StackTraceHidden]
    public UnmanagedTypeOrException<TValue> WrapUnmanagedType<TState, TValue>(
        TState state, [InstantHandle] Func<TState, TValue> operation) where TValue : unmanaged
    {
        try
        {
            return new UnmanagedTypeOrException<TValue>(operation(state));
        }
        catch (Exception e)
        {
            _exceptions.Add(e);
            return new UnmanagedTypeOrException<TValue>(e);
        }
    }
}
