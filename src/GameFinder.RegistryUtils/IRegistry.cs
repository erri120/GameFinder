using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace GameFinder.RegistryUtils;

/// <inheritdoc cref="Microsoft.Win32.RegistryHive"/>
[PublicAPI]
public enum RegistryHive
{
    /// <inheritdoc cref="Microsoft.Win32.RegistryHive.ClassesRoot"/>
    ClassesRoot,
    /// <inheritdoc cref="Microsoft.Win32.RegistryHive.CurrentUser"/>
    CurrentUser,
    /// <inheritdoc cref="Microsoft.Win32.RegistryHive.LocalMachine"/>
    LocalMachine,
    /// <inheritdoc cref="Microsoft.Win32.RegistryHive.Users"/>
    Users,
    /// <inheritdoc cref="Microsoft.Win32.RegistryHive.PerformanceData"/>
    PerformanceData,
    /// <inheritdoc cref="Microsoft.Win32.RegistryHive.CurrentConfig"/>
    CurrentConfig,
}

/// <inheritdoc cref="Microsoft.Win32.RegistryView"/>
[PublicAPI]
public enum RegistryView
{
    /// <inheritdoc cref="Microsoft.Win32.RegistryView.Default"/>
    Default,
    /// <inheritdoc cref="Microsoft.Win32.RegistryView.Registry64"/>
    Registry64,
    /// <inheritdoc cref="Microsoft.Win32.RegistryView.Registry32"/>
    Registry32,
}

/// <inheritdoc cref="Microsoft.Win32.RegistryValueKind"/>
[PublicAPI]
public enum RegistryValueKind
{
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.String"/>
    String,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.ExpandString"/>
    ExpandString,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.Binary"/>
    Binary,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.DWord"/>
    DWord,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.MultiString"/>
    MultiString,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.QWord"/>
    QWord,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.Unknown"/>
    Unknown,
    /// <inheritdoc cref="Microsoft.Win32.RegistryValueKind.None"/>
    None,
}

/// <summary>
/// Represents the Windows Registry. Use either <see cref="WindowsRegistry"/> or <see cref="InMemoryRegistry"/>
/// depending on your needs.
/// </summary>
[PublicAPI]
public interface IRegistry
{
    /// <summary>
    /// Opens the base key of a hive.
    /// </summary>
    /// <param name="hive"></param>
    /// <param name="view"></param>
    /// <returns></returns>
    IRegistryKey OpenBaseKey(RegistryHive hive, RegistryView view = RegistryView.Default);
}

/// <summary>
/// Represents a key in the registry.
/// </summary>
[PublicAPI]
public interface IRegistryKey : IDisposable
{
    /// <summary>
    /// Opens a sub-key.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IRegistryKey? OpenSubKey(string name);

    /// <summary>
    /// Returns the names of all sub-keys.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetSubKeyNames();

    /// <summary>
    /// Returns the names of all values.
    /// </summary>
    /// <returns></returns>
    IEnumerable<string> GetValueNames();

    /// <summary>
    /// Returns the <see cref="RegistryValueKind"/> of a value.
    /// </summary>
    /// <param name="valueName"></param>
    /// <returns></returns>
    RegistryValueKind GetValueKind(string? valueName);

    string GetName();

    /// <summary>
    /// Returns a value.
    /// </summary>
    /// <param name="valueName"></param>
    /// <returns></returns>
    object? GetValue(string? valueName);

    /// <summary>
    /// Tries to get a value.
    /// </summary>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetValue(string? valueName, [MaybeNullWhen(false)] out object value)
    {
        value = GetValue(valueName);
        return value is not null;
    }

    /// <summary>
    /// Gets a string.
    /// </summary>
    /// <param name="valueName"></param>
    /// <returns></returns>
    public string? GetString(string? valueName)
    {
        var value = GetValue(valueName);
        return (string?)value;
    }

    /// <summary>
    /// Tries to get a string.
    /// </summary>
    /// <param name="valueName"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetString(string? valueName, [MaybeNullWhen(false)] out string value)
    {
        value = null;

        var obj = GetValue(valueName);
        if (obj is not string s) return false;

        value = s;
        return true;
    }
}
