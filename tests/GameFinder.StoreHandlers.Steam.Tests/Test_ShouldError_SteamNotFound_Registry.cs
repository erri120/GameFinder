using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldError_SteamNotFound_Registry1(MockFileSystem fs,
        InMemoryRegistry registry)
    {
        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be("Unable to find Steam in the registry and one of the default paths");
    }

    [Theory, AutoData]
    public void Test_ShouldError_SteamNotFound_Registry2(MockFileSystem fs,
        InMemoryRegistry registry)
    {
        registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be("Unable to find Steam in the registry and one of the default paths");
    }

    [Theory, AutoData]
    public void Test_ShouldError_SteamNotFound_Registry3(MockFileSystem fs,
        InMemoryRegistry registry, string directoryName)
    {
        var key = registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        var steamPath = fs.Path.Combine(fs.Path.GetTempPath(), directoryName);
        key.AddValue("SteamPath", steamPath);

        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Unable to find Steam in one of the default paths and the path from the registry does not exist: {steamPath}");
    }

    [Theory, AutoData]
    public void Test_ShouldError_SteamNotFound_Registry4(MockFileSystem fs,
        InMemoryRegistry registry, string directoryName)
    {
        var key = registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        var steamPath = fs.Path.Combine(fs.Path.GetTempPath(), directoryName);
        key.AddValue("SteamPath", steamPath);

        fs.AddDirectory(steamPath);
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(fs.DirectoryInfo.New(steamPath));

        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Unable to find Steam in one of the default paths and the path from the registry is not a valid Steam installation because {libraryFoldersFile.FullName} does not exist");
    }
}
