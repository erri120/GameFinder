namespace GameFinder.RegistryUtils.Tests;

public partial class RegistryKeyTests
{
    [Theory, AutoData]
    public void Test_ShouldWork_GetName(InMemoryRegistry registry, RegistryHive hive, string keyName)
    {
        var key = (IRegistryKey)registry.AddKey(hive, keyName);
        key.GetName().Should().Be($"{hive.RegistryHiveToString()}\\{keyName}");
    }

    [Theory, AutoData]
    public void Test_ShouldWork_GetName_Nested(InMemoryRegistry registry,
        RegistryHive hive, string parentName, string keyName)
    {
        var key = (IRegistryKey)registry.AddKey(hive, $"{parentName}\\{keyName}");
        key.GetName().Should().Be($"{hive.RegistryHiveToString()}\\{parentName}\\{keyName}");
    }
}
