using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingLibraries(InMemoryFileSystem fs)
    {
        var handler = new SteamHandler(fs, registry: null);

        var steamPath = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(steamPath);

        fs.AddDirectory(steamPath);
        fs.AddFile(libraryFoldersFile, @"
""libraryfolders""
{
}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Found no Steam Libraries in {libraryFoldersFile}");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingLibrary(InMemoryFileSystem fs, string libraryName)
    {
        var handler = new SteamHandler(fs, registry: null);

        var steamPath = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(steamPath);

        var libraryPath = fs.GetKnownPath(KnownPath.TempDirectory)
            .CombineUnchecked(libraryName);

        fs.AddDirectory(steamPath);
        fs.AddFile(libraryFoldersFile, @$"
""libraryfolders""
{{
    ""0""
    {{
        ""path""    ""{libraryPath.GetFullPath().ToEscapedString()}""
    }}
}}
");

        var steamAppsPath = libraryPath.CombineUnchecked("steamapps");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Steam Library {steamAppsPath} does not exist!");
    }

    [Theory, AutoFileSystem]
    public void Test_ShouldError_MissingManifests(InMemoryFileSystem fs)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Library folder {basePath} does not contain any manifests");
    }
}
