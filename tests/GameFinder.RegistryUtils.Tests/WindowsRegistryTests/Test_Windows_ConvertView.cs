using System.Runtime.InteropServices;

namespace GameFinder.RegistryUtils.Tests;

public partial class WindowsRegistryTests
{
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
}
