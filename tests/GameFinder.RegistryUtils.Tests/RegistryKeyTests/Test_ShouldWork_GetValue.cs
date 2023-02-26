using GameFinder.RegistryUtils.Tests.AutoData;

namespace GameFinder.RegistryUtils.Tests;

public partial class RegistryKeyTests
{
    [Theory, RegistryAutoData]
    public void Test_ShouldWork_GetValue(InMemoryRegistryKey registryKey, string valueName, string value)
    {
        registryKey.AddValue(valueName, value);

        var key = (IRegistryKey)registryKey;
        key.GetValue(valueName).Should().Be(value);
        key.GetString(valueName).Should().Be(value);
    }
}
