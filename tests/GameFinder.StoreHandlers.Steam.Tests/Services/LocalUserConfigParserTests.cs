using FluentResults.Extensions.FluentAssertions;
using GameFinder.StoreHandlers.Steam.Services;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests.Services;

public class LocalUserConfigParserTests
{
    [Theory, AutoFileSystem]
    public async Task Test_Success(AbsolutePath configPath)
    {
        var expected = ArrangeHelper.CreateLocalUserConfig(configPath);

        var writeResult = LocalUserConfigWriter.Write(expected, configPath);
        writeResult.Should().BeSuccess();

        var result = LocalUserConfigParser.ParseConfigFile(expected.User, configPath);
        result.Should().BeSuccess().And.HaveValue(expected);
    }
}
