using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EGS;
using Xunit;

namespace GameFinder.Tests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class EGSTests
{
    [Fact]
    public void Test_ShouldWork()
    {
        var fs = new MockFileSystem();

        var manifestDir = fs.Path.GetTempPath();

        var expectedGames = new EGSGame[]
        {
            new("b8538c739273426aa35a98220e258d55", "Unreal Tournament", "C:\\Games\\UnrealTournament"),
            new("3257e06c28764231acd93049f3774ed6", "Subnautica", "C:\\Games\\Subnautica")
        };

        foreach (var game in expectedGames)
        {
            var mockData = @$"{{
    ""CatalogItemId"": ""{game.CatalogItemId}"",
    ""DisplayName"": ""{game.DisplayName}"",
    ""InstallLocation"": ""{game.InstallLocation.Replace("\\", "\\\\")}""
}}";

            fs.AddFile($"{fs.Path.Combine(manifestDir, $"{game.CatalogItemId}.item")}", mockData);
        }

        var registry = new InMemoryRegistry();

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue("ModSdkMetadataDir", manifestDir);

        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        var actualGames = results.Select(tuple =>
        {
            var (game, error) = tuple;
            Assert.True(game is not null, error);
            return game;
        }).ToArray();

        Assert.Equal(expectedGames.Length, actualGames.Length);
        Assert.Equal(expectedGames, actualGames);
    }

    [Fact]
    public void Test_ShouldError_MissingDirectory_DefaultPath()
    {
        var registry = new InMemoryRegistry();
        var fs = new MockFileSystem();

        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"The manifest directory {EGSHandler.GetDefaultManifestsPath(fs)} does not exist!", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_MissingDirectory_Registry()
    {
        var registry = new InMemoryRegistry();

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue("ModSdkMetadataDir", "C:\\foo");

        var fs = new MockFileSystem();

        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal("The manifest directory C:\\foo does not exist!", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_InvalidManifest()
    {
        var fs = new MockFileSystem();

        var manifestDir = fs.Path.GetTempPath();

        var randomBytes = new byte[128];
        Random.Shared.NextBytes(randomBytes);

        fs.AddFile(fs.Path.Combine(manifestDir, "brokenManifest.item"), new MockFileData(randomBytes));

        var registry = new InMemoryRegistry();

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue("ModSdkMetadataDir", manifestDir);

        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.StartsWith("Unable to deserialize file", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_MissingValues()
    {
        var fs = new MockFileSystem();

        var manifestDir = fs.Path.GetTempPath();

        var manifest1 = fs.Path.Combine(manifestDir, "no-id.item");
        var manifest2 = fs.Path.Combine(manifestDir, "no-name.item");
        var manifest3 = fs.Path.Combine(manifestDir, "no-path.item");

        fs.AddFile(manifest1, "{}");
        fs.AddFile(manifest2, @"{ ""CatalogItemId"": ""foo"" }");
        fs.AddFile(manifest3, @"{ ""CatalogItemId"": ""foo"", ""DisplayName"": ""bar"" }");

        var registry = new InMemoryRegistry();

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue("ModSdkMetadataDir", manifestDir);

        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Manifest {manifest1} does not have a value \"CatalogItemId\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Manifest {manifest2} does not have a value \"DisplayName\"", result.Error);
        }, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"Manifest {manifest3} does not have a value \"InstallLocation\"", result.Error);
        });
    }

    [Fact]
    public void Test_ShouldError_NoManifests()
    {
        var fs = new MockFileSystem();

        var manifestDir = fs.Path.GetTempPath();

        fs.AddDirectory(manifestDir);

        var registry = new InMemoryRegistry();

        var regKey = registry.AddKey(RegistryHive.CurrentUser, EGSHandler.RegKey);
        regKey.AddValue("ModSdkMetadataDir", manifestDir);

        var handler = new EGSHandler(registry, fs);

        var results = handler.FindAllGames().ToArray();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Game);
            Assert.Equal($"The manifest directory {manifestDir} does not contain any .item files", result.Error);
        });
    }
}
