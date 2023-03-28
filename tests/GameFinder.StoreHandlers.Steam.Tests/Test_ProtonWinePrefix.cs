using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.StoreHandlers.Steam.Tests;

public partial class SteamTests
{
    [Theory, AutoFileSystem]
    public void Test_SteamGame_GetProtonPrefix(InMemoryFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        var wineDirectory = protonDirectory.CombineUnchecked("pfx");

        protonPrefix.ProtonDirectory.Should().Be(protonDirectory);
        protonPrefix.ConfigurationDirectory.Should().Be(wineDirectory);
    }

    [Theory, AutoFileSystem]
    public void Test_ProtonPrefix_GetConfigInfoFile(InMemoryFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        protonPrefix.GetConfigInfoFile().Should().Be(protonDirectory.CombineUnchecked("config_info"));
    }

    [Theory, AutoFileSystem]
    public void Test_ProtonPrefix_GetVersionFile(InMemoryFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        protonPrefix.GetVersionFile().Should().Be(protonDirectory.CombineUnchecked("version"));
    }

    [Theory, AutoFileSystem]
    public void Test_ProtonPrefix_GetLaunchCommand(InMemoryFileSystem fs, int appId, string name)
    {
        var (protonDirectory, protonPrefix) = SetupProtonPrefix(fs, appId, name);
        protonPrefix.GetLaunchCommandFile().Should().Be(protonDirectory.CombineUnchecked("launch_command"));
    }
}
