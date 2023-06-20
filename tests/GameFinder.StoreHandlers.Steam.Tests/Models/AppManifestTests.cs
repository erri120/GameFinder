using System.Globalization;
using FluentResults.Extensions.FluentAssertions;
using GameFinder.StoreHandlers.Steam.Services;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests.Models;

public class AppManifestTests
{
    [Theory, AutoFileSystem]
    public void Test_Equality(AbsolutePath manifestPath)
    {
        var value = ArrangeHelper.CreateAppManifest(manifestPath);
        value.Equals(value).Should().BeTrue();
        value.GetHashCode().Should().Be(value.GetHashCode());
    }

    [Theory, AutoFileSystem]
    public void Test_Reload(AbsolutePath manifestPath)
    {
        var value = ArrangeHelper.CreateAppManifest(manifestPath);
        AppManifestWriter.Write(value, manifestPath).Should().BeSuccess();

        var result = value.Reload();
        result.Should().BeSuccess().And.HaveValue(value);
    }
}
