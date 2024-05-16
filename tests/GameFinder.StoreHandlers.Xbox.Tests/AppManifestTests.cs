using System.Xml;
using System.Xml.Schema;
using GameFinder.StoreHandlers.Xbox.Serialization;
using TestUtils;
using Xunit.Abstractions;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public class AppManifestTests : TestWrapper
{
    public AppManifestTests(ITestOutputHelper output) : base(output) { }

    [Fact]
    public void Test_ParseManifest()
    {
        var file = GetTestFile("appxmanifest.xml");
        using var stream = FileSystem.ReadFile(file);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
        });

        var res = AppManifest.ParseAppManifest(
            Logger,
            reader,
            file
        );

        res.Should().NotBeNull();
        res.Should().Be(new AppManifest.Package
        {
            Identity = new AppManifest.Identity
            {
                // ReSharper disable once StringLiteralTypo
                Name = "BethesdaSoftworks.FalloutNewVegas",
            },
            Properties = new AppManifest.Properties
            {
                DisplayName = "Fallout: New Vegas Ultimate Edition (PC)",
            },
        });
    }
}
