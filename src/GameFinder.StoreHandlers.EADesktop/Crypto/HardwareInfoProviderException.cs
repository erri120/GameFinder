using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop.Crypto;

/// <summary>
/// Represents an Exception thrown by <see cref="IHardwareInfoProvider"/>.
/// </summary>
[PublicAPI]
public class HardwareInfoProviderException : Exception
{
    private readonly string _msg;
    private readonly Exception? _inner;

    internal HardwareInfoProviderException(string msg, Exception? inner) : base(msg, inner)
    {
        _msg = msg;
        _inner = inner;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return _inner is null ? _msg : $"{_msg}\n{_inner}";
    }
}
