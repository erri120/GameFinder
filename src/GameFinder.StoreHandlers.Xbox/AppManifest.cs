using System.Xml.Serialization;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Xbox;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[XmlRoot(ElementName="Identity", Namespace="http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
public class Identity
{
    [XmlAttribute(AttributeName = "Name", Namespace = "")]
    public string Name { get; set; } = null!;
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[XmlRoot(ElementName="Properties", Namespace="http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
public class Properties
{
    [XmlElement(ElementName = "DisplayName", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public string DisplayName { get; set; } = null!;
}

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[XmlRoot(ElementName="Package", Namespace="http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
public class Package
{
    [XmlElement(ElementName = "Identity", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public Identity Identity { get; set; } = null!;

    [XmlElement(ElementName = "Properties", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public Properties Properties { get; set; } = null!;
}
