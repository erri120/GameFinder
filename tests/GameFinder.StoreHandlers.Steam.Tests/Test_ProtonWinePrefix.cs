using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoData]
    public void Test_SteamGame_GetProtonPrefix(MockFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        var wineDirectory = fs.Path.Combine(protonDirectory, "pfx");

        protonPrefix.ProtonDirectory.Should().Be(protonDirectory);
        protonPrefix.ConfigurationDirectory.Should().Be(wineDirectory);
    }

    [Theory, AutoData]
    public void Test_ProtonPrefix_GetConfigInfoFile(MockFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        protonPrefix.GetConfigInfoFile(fs).Should().Be(fs.Path.Combine(protonDirectory, "config_info"));
    }

    [Theory, AutoData]
    public void Test_ProtonPrefix_GetVersionFile(MockFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        protonPrefix.GetVersionFile(fs).Should().Be(fs.Path.Combine(protonDirectory, "version"));
    }

    [Theory, AutoData]
    public void Test_ProtonPrefix_GetLaunchCommand(MockFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        protonPrefix.GetLaunchCommandFile(fs).Should().Be(fs.Path.Combine(protonDirectory, "launch_command"));
    }
}
