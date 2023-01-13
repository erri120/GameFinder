using System.IO.Abstractions.TestingHelpers;
using TestUtils;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_ShouldError_MissingLibraries(MockFileSystem fs)
    {
        var handler = new SteamHandler(fs, registry: null);

        var steamPath = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(steamPath);

        fs.AddDirectory(steamPath.FullName);
        fs.AddFile(libraryFoldersFile.FullName, @"
""libraryfolders""
{
}
");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Found no Steam Libraries in {libraryFoldersFile.FullName}");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingLibrary(MockFileSystem fs, string libraryName)
    {
        var handler = new SteamHandler(fs, registry: null);

        var steamPath = SteamHandler.GetDefaultSteamDirectories(fs).First();
        var libraryFoldersFile = SteamHandler.GetLibraryFoldersFile(steamPath);

        var libraryPath = fs.Path.Combine(fs.Path.GetTempPath(), libraryName);

        fs.AddDirectory(steamPath.FullName);
        fs.AddFile(libraryFoldersFile.FullName, @$"
""libraryfolders""
{{
    ""0""
    {{
        ""path""    ""{libraryPath.ToEscapedString()}""
    }}
}}
");

        var steamAppsPath = fs.Path.Combine(libraryPath, "steamapps");

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Steam Library {steamAppsPath} does not exist!");
    }

    [Theory, AutoData]
    public void Test_ShouldError_MissingManifests(MockFileSystem fs)
    {
        var (handler, basePath, _) = SetupHandler(fs);

        var error = handler.ShouldOnlyBeOneError();
        error.Should().Be($"Library folder {basePath} does not contain any manifests");
    }
}
