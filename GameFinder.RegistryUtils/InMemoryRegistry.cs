using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GameFinder.RegistryUtils;

/// <summary>
/// Implementation of <see cref="IRegistry"/> that is entirely in-memory for use in tests.
/// </summary>
[PublicAPI]
public sealed class InMemoryRegistry : IRegistry
{
    private readonly Dictionary<RegistryHive, InMemoryRegistryKey> _baseKeys;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public InMemoryRegistry()
    {
        _baseKeys = new Dictionary<RegistryHive, InMemoryRegistryKey>
        {
            {RegistryHive.ClassesRoot, new InMemoryRegistryKey(RegistryHive.ClassesRoot, "HKEY_CLASSES_ROOT")},
            {RegistryHive.CurrentUser, new InMemoryRegistryKey(RegistryHive.CurrentUser, "HKEY_CURRENT_USER")},
            {RegistryHive.LocalMachine, new InMemoryRegistryKey(RegistryHive.LocalMachine, "HKEY_LOCAL_MACHINE")},
            {RegistryHive.Users, new InMemoryRegistryKey(RegistryHive.Users, "HKEY_USERS")},
            {RegistryHive.CurrentConfig, new InMemoryRegistryKey(RegistryHive.CurrentConfig, "HKEY_CURRENT_CONFIG")},
            {RegistryHive.PerformanceData, new InMemoryRegistryKey(RegistryHive.PerformanceData, "HKEY_PERFORMANCE_DATA")}
        };
    }
    
    /// <summary>
    /// Adds a key to the registry.
    /// </summary>
    /// <param name="hive"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public InMemoryRegistryKey AddKey(RegistryHive hive, string key)
    {
        // normalize
        key = key.Replace('/', '\\');

        var keyNames = key.Split('\\', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var parent = _baseKeys[hive];

        foreach (var keyName in keyNames)
        {
            var child = parent.AddSubKey(hive, keyName);
            parent = child;
        }

        return parent;
    }
    
    /// <inheritdoc/>
    public IRegistryKey OpenBaseKey(RegistryHive hive, RegistryView view = RegistryView.Default)
    {
        return _baseKeys[hive];
    }
}

/// <summary>
/// Implementation of <see cref="IRegistryKey"/> that is entirely in-memory for use in tests.
/// </summary>
[PublicAPI]
public sealed class InMemoryRegistryKey : IRegistryKey
{
    private readonly RegistryHive _hive;
    private readonly string _key;
    private readonly InMemoryRegistryKey _parent;
    private readonly Dictionary<string, InMemoryRegistryKey> _children = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, object> _values = new(StringComparer.OrdinalIgnoreCase);

    internal InMemoryRegistryKey(RegistryHive hive, string key)
    {
        _hive = hive;
        _key = key;
        _parent = this;
    }

    internal InMemoryRegistryKey(RegistryHive hive, InMemoryRegistryKey parent, string key)
    {
        _hive = hive;
        _parent = parent;
        _key = key;
    }
    
    internal InMemoryRegistryKey AddSubKey(RegistryHive hive, string key)
    {
        if (_children.TryGetValue(key, out var child)) return child;
        
        child = new InMemoryRegistryKey(hive, this, key);
        _children.Add(key, child);
        
        return child;
    }

    /// <summary>
    /// Adds a value to the key.
    /// </summary>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    public void AddValue(string valueName, object value)
    {
        _values.Add(valueName, value);
    }

    /// <inheritdoc/>
    public IRegistryKey? OpenSubKey(string name)
    {
        // normalize
        name = name.Replace('/', '\\');

        var names = name.Split('\\', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (_children.TryGetValue(names[0], out var child))
        {
            return names.Length == 1 ? child : child.OpenSubKey(names.Skip(1).Aggregate((a, b) => $"{a}\\{b}"));
        }

        return null;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetSubKeyNames()
    {
        return _children.Keys;
    }

    /// <inheritdoc/>
    public IEnumerable<string> GetValueNames()
    {
        return _values.Keys;
    }

    /// <inheritdoc/>
    public RegistryValueKind GetValueKind(string? valueName)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public object? GetValue(string? valueName)
    {
        _values.TryGetValue(valueName ?? string.Empty, out var value);
        return value;
    }

    /// <inheritdoc/>
    public void Dispose() { }
}
