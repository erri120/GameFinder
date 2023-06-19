

using FluentResults.Extensions.FluentAssertions;
using GameFinder.StoreHandlers.Steam.Services;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests.Services;

public class LibraryFoldersManifestParserTests
{
    [Theory, AutoFileSystem]
    public void Test_Everything(AbsolutePath manifestFilePath)
    {
        var expected = ArrangeHelper.CreateLibraryFoldersManifest(manifestFilePath);

        var writeResult = LibraryFoldersManifestWriter.Write(expected, manifestFilePath);
        writeResult.Should().BeSuccess();

        var result = LibraryFoldersManifestParser.ParseManifestFile(manifestFilePath);
        result.Should().BeSuccess().And.HaveValue(expected);
    }
}
