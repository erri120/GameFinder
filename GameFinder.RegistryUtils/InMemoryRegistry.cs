using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GameFinder.RegistryUtils;

[PublicAPI]
public sealed class InMemoryRegistry : IRegistry
{
    private readonly Dictionary<RegistryHive, InMemoryRegistryKey> _baseKeys;

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
        var child = new InMemoryRegistryKey(hive, this, key);
        _children.Add(key, child);
        return child;
    }

    public void AddValue(string valueName, object value)
    {
        _values.Add(valueName, value);
    }

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

    public IEnumerable<string> GetSubKeyNames()
    {
        return _children.Keys;
    }

    public IEnumerable<string> GetValueNames()
    {
        return _values.Keys;
    }

    public RegistryValueKind GetValueKind(string? valueName)
    {
        throw new NotImplementedException();
    }

    public object? GetValue(string? valueName)
    {
        _values.TryGetValue(valueName ?? string.Empty, out var value);
        return value;
    }

    /// <inheritdoc/>
    public void Dispose() { }
}
