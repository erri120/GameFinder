namespace GameFinder.RegistryUtils.Tests.AutoData;

public class RegistryAutoDataAttribute : AutoDataAttribute
{
    public RegistryAutoDataAttribute() : base(() =>
    {
        var ret = new Fixture();
        ret.Customizations.Add(new RegistryKeyBuilder());
        return ret;
    })
    { }
}
