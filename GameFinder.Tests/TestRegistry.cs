using GameFinder.RegistryUtils;
using Xunit;

namespace GameFinder.Tests;

public class TestRegistry
{
    private static InMemoryRegistryKey SetupKey()
    {
        var registry = new InMemoryRegistry();
        var key = registry.AddKey(RegistryHive.CurrentUser, "foo/bar/baz");
        return key;
    }

    [Fact]
    public void Test_GetName()
    {
        var key = SetupKey();
        Assert.Equal("HKEY_CURRENT_USER\\foo\\bar\\baz", key.GetName());
    }

    [Fact]
    public void Test_GetValue()
    {
        var key = SetupKey();
        key.AddValue("Name", "Peter Griffin");
        Assert.Equal("Peter Griffin", key.GetValue("name"));
    }

    [Fact]
    public void Test_GetString()
    {
        var key = SetupKey();
        key.AddValue("Name", "Peter Griffin");
        Assert.Equal("Peter Griffin", ((IRegistryKey)key).GetString("name"));
    }
}
