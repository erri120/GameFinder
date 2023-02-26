using AutoFixture.Kernel;
using TestUtils;

namespace GameFinder.RegistryUtils.Tests.AutoData;

public class RegistryKeyBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        var type = request.ExtractType();
        if (type != typeof(IRegistryKey) && type != typeof(InMemoryRegistryKey)) return new NoSpecimen();

        var registry = context.Create<InMemoryRegistry>();
        var hive = context.Create<RegistryHive>();
        var parentKeyName = context.Create<string>();
        var keyName = context.Create<string>();

        var key = registry.AddKey(hive, $"{parentKeyName}\\{keyName}");
        return key;
    }
}
