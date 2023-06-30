using FluentResults.Extensions.FluentAssertions;
using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using GameFinder.StoreHandlers.Steam.Services;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests.Services;

public class WorkshopManifestParserTests
{
    [Theory, AutoFileSystem]
    public void Test_Success_OnlyRequired(AbsolutePath manifestFilePath)
    {
        var expected = new WorkshopManifest
        {
            ManifestPath = manifestFilePath,
            AppId = AppId.From(262060),
        };

        var writeResult = WorkshopManifestWriter.Write(expected, manifestFilePath);
        writeResult.Should().BeSuccess();

        var result = WorkshopManifestParser.ParseManifestFile(manifestFilePath);
        result.Should().BeSuccess().And.HaveValue(expected);
    }

    [Theory, AutoFileSystem]
    public void Test_Success_Everything(AbsolutePath manifestFilePath)
    {
        var expected = ArrangeHelper.CreateWorkshopManifest(manifestFilePath);

        var writeResult = WorkshopManifestWriter.Write(expected, manifestFilePath);
        writeResult.Should().BeSuccess();

        var result = WorkshopManifestParser.ParseManifestFile(manifestFilePath);
        result.Should().BeSuccess().And.HaveValue(expected);
    }
}
