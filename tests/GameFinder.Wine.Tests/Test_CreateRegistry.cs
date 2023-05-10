using GameFinder.RegistryUtils;
using NexusMods.Paths;
using NexusMods.Paths.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    [Theory, AutoFileSystem]
    public void Test_CreateRegistry(InMemoryFileSystem fs, AbsolutePath configurationDirectory)
    {
        fs.AddDirectory(configurationDirectory);
        var prefix = new WinePrefix { ConfigurationDirectory = configurationDirectory };

        fs.AddFile(configurationDirectory.CombineUnchecked("system.reg"), """
WINE REGISTRY VERSION 2
;; something

[foo\\bar\\baz] 1235123
# something
@="1"
"2"="3"

[baz\\bar\\foo] 1234123
"4"="5"
""");

        var registry = prefix.CreateRegistry(fs);
        using var baseKey = registry.OpenBaseKey(RegistryHive.LocalMachine);
        using var subKey1 = baseKey.OpenSubKey("foo\\bar\\baz");
        subKey1.Should().NotBeNull();

        subKey1!.GetValue(valueName: null).Should().Be("1");
        subKey1.GetValue("2").Should().Be("3");

        using var subKey2 = baseKey.OpenSubKey("baz\\bar\\foo");
        subKey2.Should().NotBeNull();

        subKey2!.GetValue("4").Should().Be("5");
    }
}
