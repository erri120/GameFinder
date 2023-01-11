using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using GameFinder.RegistryUtils;
using Xunit;

namespace GameFinder.Tests;

public class RegistryTests
{
    private static InMemoryRegistryKey SetupKey()
    {
        var registry = new InMemoryRegistry();
        var key = registry.AddKey(RegistryHive.CurrentUser, "foo/bar/baz");
        return key;
    }

    [Fact]
    public void Test_GetName()
    {
        var key = SetupKey();
        key.GetName().Should().Be("HKEY_CURRENT_USER\\foo\\bar\\baz");
    }

    [Fact]
    public void Test_GetValue()
    {
        var key = SetupKey();
        key.AddValue("Name", "Peter Griffin");
        key.GetValue("name").Should().Be("Peter Griffin");
    }

    [Fact]
    public void Test_GetString()
    {
        var key = SetupKey();
        key.AddValue("Name", "Peter Griffin");

        var registryKey = (IRegistryKey)key;
        registryKey.GetString("name").Should().Be("Peter Griffin");
    }

    [Fact]
    public void Test_Windows_ConvertHive()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        InteropHelpers.Convert(RegistryHive.ClassesRoot).Should().Be(Microsoft.Win32.RegistryHive.ClassesRoot);
        InteropHelpers.Convert(RegistryHive.CurrentUser).Should().Be(Microsoft.Win32.RegistryHive.CurrentUser);
        InteropHelpers.Convert(RegistryHive.LocalMachine).Should().Be(Microsoft.Win32.RegistryHive.LocalMachine);
        InteropHelpers.Convert(RegistryHive.Users).Should().Be(Microsoft.Win32.RegistryHive.Users);
        InteropHelpers.Convert(RegistryHive.PerformanceData).Should().Be(Microsoft.Win32.RegistryHive.PerformanceData);
        InteropHelpers.Convert(RegistryHive.CurrentConfig).Should().Be(Microsoft.Win32.RegistryHive.CurrentConfig);

        #pragma warning disable CA1416
        var invalidConvert = () => InteropHelpers.Convert((RegistryHive)int.MaxValue);
        #pragma warning restore CA1416

        invalidConvert.Should().Throw<ArgumentOutOfRangeException>();
    }


    [Fact]
    public void Test_Windows_ConvertView()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        InteropHelpers.Convert(RegistryView.Default).Should().Be(Microsoft.Win32.RegistryView.Default);
        InteropHelpers.Convert(RegistryView.Registry64).Should().Be(Microsoft.Win32.RegistryView.Registry64);
        InteropHelpers.Convert(RegistryView.Registry32).Should().Be(Microsoft.Win32.RegistryView.Registry32);

#pragma warning disable CA1416
        var invalidConvert = () => InteropHelpers.Convert((RegistryView)int.MaxValue);
#pragma warning restore CA1416

        invalidConvert.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Test_Windows_ConvertValueKind()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.None).Should().Be(RegistryValueKind.None);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.Unknown).Should().Be(RegistryValueKind.Unknown);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.String).Should().Be(RegistryValueKind.String);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.ExpandString).Should().Be(RegistryValueKind.ExpandString);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.Binary).Should().Be(RegistryValueKind.Binary);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.DWord).Should().Be(RegistryValueKind.DWord);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.MultiString).Should().Be(RegistryValueKind.MultiString);
        InteropHelpers.Convert(Microsoft.Win32.RegistryValueKind.QWord).Should().Be(RegistryValueKind.QWord);

#pragma warning disable CA1416
        var invalidConvert = () => InteropHelpers.Convert((Microsoft.Win32.RegistryValueKind)int.MaxValue);
#pragma warning restore CA1416

        invalidConvert.Should().Throw<ArgumentOutOfRangeException>();
    }
}
