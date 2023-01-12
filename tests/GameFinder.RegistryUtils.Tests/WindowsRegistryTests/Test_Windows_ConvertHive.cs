using System.Runtime.InteropServices;

namespace GameFinder.RegistryUtils.Tests;

public partial class WindowsRegistryTests
{
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
}
