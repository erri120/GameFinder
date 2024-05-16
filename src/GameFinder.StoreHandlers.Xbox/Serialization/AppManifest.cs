using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Xbox.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
[PublicAPI]
public static class AppManifest
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [XmlRoot(ElementName = "Identity", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public record Identity
    {
        [XmlAttribute(AttributeName = "Name", Namespace = "")]
        public required string Name { get; init; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [XmlRoot(ElementName = "Properties", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public record Properties
    {
        [XmlElement(ElementName = "DisplayName", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
        public required string DisplayName { get; init; }
    }

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    [XmlRoot(ElementName = "Package", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public record Package
    {
        [XmlElement(ElementName = "Identity", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
        public required Identity Identity { get; init; }

        [XmlElement(ElementName = "Properties", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
        public required Properties Properties { get; init; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Deserializes the data in the provided <see cref="XmlReader"/> and returns a <see cref="Package"/>.
    /// </summary>
    [RequiresUnreferencedCode($"Requires {nameof(Package)} to not be trimmed for System.Xml.Serialization.XmlSerializer.Deserialize(XmlReader)")]
    public static Package? ParseAppManifest(
        ILogger logger,
        XmlReader xmlReader,
        AbsolutePath manifestFilePath)
    {
        try
        {
            var obj = new XmlSerializer(typeof(Package)).Deserialize(xmlReader);
            if (obj is Package package) return package;

            LogMessages.ManifestDeserializationFailed(logger, manifestFilePath, obj, obj?.GetType());
            return null;

        }
        catch (Exception e)
        {
            LogMessages.ExceptionWhileParsingManifest(logger, e, manifestFilePath);
            return null;
        }
    }
}
