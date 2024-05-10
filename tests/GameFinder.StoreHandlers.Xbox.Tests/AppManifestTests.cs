using System.Xml;
using System.Xml.Schema;
using GameFinder.StoreHandlers.Xbox.Serialization;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using TestUtils;
using Xunit.Abstractions;

namespace GameFinder.StoreHandlers.Xbox.Tests;

public class AppManifestTests
{
    private readonly ILogger _logger;
    public AppManifestTests(ITestOutputHelper output)
    {
        _logger = new XunitLogger(output);
    }

    [Fact]
    public void Test_ParseManifest()
    {
        var fileSystem = FileSystem.Shared;

        var file = fileSystem.GetKnownPath(KnownPath.EntryDirectory).Combine("files").Combine("appxmanifest.xml");
        using var stream = fileSystem.ReadFile(file);
        using var reader = XmlReader.Create(stream, new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreWhitespace = true,
            ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes,
        });

        var res = AppManifest.ParseAppManifest(
            _logger,
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
