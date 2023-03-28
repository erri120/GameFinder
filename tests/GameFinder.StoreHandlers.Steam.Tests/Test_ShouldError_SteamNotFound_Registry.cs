using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_SteamNotFound_Registry1(InMemoryFileSystem fs,
        InMemoryRegistry registry)
    {
        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be("Unable to find Steam in the registry and one of the default paths");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_SteamNotFound_Registry2(InMemoryFileSystem fs,
        InMemoryRegistry registry)
    {
        registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be("Unable to find Steam in the registry and one of the default paths");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_SteamNotFound_Registry3(InMemoryFileSystem fs,
        InMemoryRegistry registry, string directoryName)
    {
        var key = registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        var steamPath = fs.GetKnownPath(KnownPath.TempDirectory).CombineUnchecked(directoryName);
        key.AddValue("SteamPath", steamPath.GetFullPath());

        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Unable to find Steam in one of the default paths and the path from the registry does not exist: {steamPath}");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_SteamNotFound_Registry4(InMemoryFileSystem fs,
        InMemoryRegistry registry, string directoryName)
    {
        var key = registry.AddKey(RegistryHive.CurrentUser, SteamHandler.RegKey);
        var steamPath = fs.GetKnownPath(KnownPath.TempDirectory).CombineUnchecked(directoryName);
        key.AddValue("SteamPath", steamPath.GetFullPath());

        fs.AddDirectory(steamPath);
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(steamPath);

        var handler = new SteamHandler(fs, registry);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Unable to find Steam in one of the default paths and the path from the registry is not a valid Steam installation because {libraryFoldersFile} does not exist");
    }
}
