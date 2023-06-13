using GameFinder.Common;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingVirtualDrive(InMemoryFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupWinePrefix(fs);
        var virtualDriveDirectory = prefixDirectory.CombineUnchecked("drive_c");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsMessage())
            .Which
            .AsMessage()
            .Should()
            .Be($"Virtual C: drive does not exist at {virtualDriveDirectory}");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingSystemRegistryFile(InMemoryFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupWinePrefix(fs);
        var virtualDriveDirectory = prefixDirectory.CombineUnchecked("drive_c");
        fs.AddDirectory(virtualDriveDirectory);

        var systemRegistryFile = prefixDirectory.CombineUnchecked("system.reg");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsMessage())
            .Which
            .AsMessage()
            .Should()
            .Be($"System registry file does not exist at {systemRegistryFile}");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingUserRegistryFile(InMemoryFileSystem fs)
    {
        var (prefixDirectory, prefixManager) = SetupWinePrefix(fs);
        var virtualDriveDirectory = prefixDirectory.CombineUnchecked("drive_c");
        fs.AddDirectory(virtualDriveDirectory);

        var systemRegistryFile = prefixDirectory.CombineUnchecked("system.reg");
        fs.AddEmptyFile(systemRegistryFile);

        var userRegistryFile = prefixDirectory.CombineUnchecked("user.reg");

        prefixManager.FindPrefixes().Should()
            .ContainSingle(result => result.IsMessage())
            .Which
            .AsMessage()
            .Should()
            .Be($"User registry file does not exist at {userRegistryFile}");
    }
}
