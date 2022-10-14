using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GameFinder.RegistryUtils;

[PublicAPI]
public enum RegistryHive
{
    ClassesRoot,
    CurrentUser,
    LocalMachine,
    Users,
    PerformanceData,
    CurrentConfig,
}

[PublicAPI]
public enum RegistryView
{
    Default,
    Registry64,
    Registry32
}

[PublicAPI]
public enum RegistryValueKind
{
    String,
    ExpandString,
    Binary,
    DWord,
    MultiString,
    QWord,
    Unknown,
    None
}

[PublicAPI]
public interface IRegistry
{
    IRegistryKey OpenBaseKey(RegistryHive hive, RegistryView view = RegistryView.Default);
}

[PublicAPI]
public interface IRegistryKey : IDisposable
{
    IRegistryKey? OpenSubKey(string name);

    IEnumerable<string> GetSubKeyNames();

    IEnumerable<string> GetValueNames();

    RegistryValueKind GetValueKind(string? valueName);
    
    object? GetValue(string? valueName);

    public bool TryGetValue(string? valueName, [MaybeNullWhen(false)] out object value)
    {
        value = GetValue(valueName);
        return value is not null;
    }

    public long? GetQWord(string? valueName)
    {
        var value = GetValue(valueName);
        return (long?)value;
    }

    public bool TryGetQWord(string? valueName, out long value)
    {
        value = default;

        var obj = GetValue(valueName);
        if (obj is not long qword) return false;

        value = qword;
        return true;

    }

    public string? GetString(string? valueName)
    {
        var value = GetValue(valueName);
        return (string?)value;
    }

    public bool TryGetString(string? valueName, [MaybeNullWhen(false)] out string value)
    {
        value = null;
        
        var obj = GetValue(valueName);
        if (obj is not string s) return false;
        
        value = s;
        return true;
    }
}