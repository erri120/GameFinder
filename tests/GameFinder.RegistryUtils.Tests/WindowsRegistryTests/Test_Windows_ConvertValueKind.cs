using System.Runtime.InteropServices;

namespace GameFinder.RegistryUtils.Tests;

public partial class WindowsRegistryTests
{
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
