using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_GetVirtualDrivePath(MockFileSystem fs, string prefixDirectory)
    {
        var winePrefix = new WinePrefix(prefixDirectory);

        winePrefix.GetVirtualDrivePath(fs.Path).Should().Be(fs.Path.Combine(prefixDirectory, "drive_c"));
        winePrefix.GetVirtualDrivePath().Should().Be(Path.Combine(prefixDirectory, "drive_c"));
    }

    [Theory, AutoData]
    public void Test_ShouldWork_GetSystemRegistryFile(MockFileSystem fs, string prefixDirectory)
    {
        var winePrefix = new WinePrefix(prefixDirectory);

        winePrefix.GetSystemRegistryFile(fs.Path).Should().Be(fs.Path.Combine(prefixDirectory, "system.reg"));
        winePrefix.GetSystemRegistryFile().Should().Be(Path.Combine(prefixDirectory, "system.reg"));
    }

    [Theory, AutoData]
    public void Test_ShouldWork_GetUserRegistryFile(MockFileSystem fs, string prefixDirectory)
    {
        var winePrefix = new WinePrefix(prefixDirectory);

        winePrefix.GetUserRegistryFile(fs.Path).Should().Be(fs.Path.Combine(prefixDirectory, "user.reg"));
        winePrefix.GetUserRegistryFile().Should().Be(Path.Combine(prefixDirectory, "user.reg"));
    }
}
