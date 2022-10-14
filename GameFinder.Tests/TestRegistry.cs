using GameFinder.RegistryUtils;
using Xunit;

namespace GameFinder.Tests;

public class TestRegistry
{
    [Fact]
    public void TestInMemoryRegistry()
    {
        var registry = new InMemoryRegistry();

        var baz = registry.AddKey(RegistryHive.LocalMachine, "foo/bar\\baz");
        baz.AddValue("name", "Peter Griffin");
        baz.AddValue("age", (long)40);

        var baseKey = registry.OpenBaseKey(RegistryHive.LocalMachine);
        var subKey = baseKey.OpenSubKey("foo/bar/baz");
        Assert.NotNull(subKey);        
        Assert.Equal("Peter Griffin", subKey.GetString("name"));
        Assert.Equal(40, subKey.GetQWord("age"));
    }
}