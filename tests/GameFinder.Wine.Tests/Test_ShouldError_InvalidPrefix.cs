using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingVirtualDrive(MockFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupWinePrefix(fs);
        var virtualDriveDirectory = fs.Path.Combine(prefixDirectory.FullName, "drive_c");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsT1)
            .Which
            .AsT1
            .Should()
            .Be(PrefixDiscoveryError
                .From($"Virtual C: drive does not exist at {virtualDriveDirectory}"));
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingSystemRegistryFile(MockFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupWinePrefix(fs);
        var virtualDriveDirectory = fs.Path.Combine(prefixDirectory.FullName, "drive_c");
        fs.AddDirectory(virtualDriveDirectory);

        var systemRegistryFile = fs.Path.Combine(prefixDirectory.FullName, "system.reg");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsT1)
            .Which
            .AsT1
            .Should()
            .Be(PrefixDiscoveryError
                .From($"System registry file does not exist at {systemRegistryFile}"));
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingUserRegistryFile(MockFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupWinePrefix(fs);
        var virtualDriveDirectory = fs.Path.Combine(prefixDirectory.FullName, "drive_c");
        fs.AddDirectory(virtualDriveDirectory);

        var systemRegistryFile = fs.Path.Combine(prefixDirectory.FullName, "system.reg");
        fs.AddEmptyFile(systemRegistryFile);

        var userRegistryFile = fs.Path.Combine(prefixDirectory.FullName, "user.reg");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsT1)
            .Which
            .AsT1
            .Should()
            .Be(PrefixDiscoveryError
                .From($"User registry file does not exist at {userRegistryFile}"));
    }
}
