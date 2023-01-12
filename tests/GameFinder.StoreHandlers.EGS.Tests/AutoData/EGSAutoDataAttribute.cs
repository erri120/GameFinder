namespace GameFinder.StoreHandlers.EGS.Tests.AutoData;

public class EGSAutoDataAttribute : AutoDataAttribute
{
    public EGSAutoDataAttribute() : base(() =>
    {
        var ret = new Fixture();
        ret.Customizations.Add(new FileSystemBuilder());
        return ret;
    }) { }
}
