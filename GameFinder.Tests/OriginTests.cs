using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Web;
using GameFinder.StoreHandlers.Origin;
using Xunit;

namespace GameFinder.Tests;

[SuppressMessage("ReSharper", "ParameterOnlyUsedForPreconditionCheck.Local")]
public class OriginTests
{
    [Fact]
    public void Test_ShouldWork()
    {
        var fs = new MockFileSystem();
        var manifestDir = OriginHandler.GetManifestDir(fs);

        fs.AddDirectory(manifestDir.FullName);

        var expectedGames = new List<OriginGame>
        {
            new("Origin.OFR.50.0001456", "C:\\Games\\Titanfall2"),
            new("Origin.OFR.50.0003803", "C:\\Games\\Slay the Spire")
        };

        foreach (var game in expectedGames)
        {
            var manifest = fs.Path.Combine(manifestDir.FullName, $"{game.Id}.mfst");
            fs.AddFile(manifest, $"?id={HttpUtility.UrlEncode(game.Id)}&dipInstallPath={HttpUtility.UrlEncode(game.InstallPath)}");
        }

        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToList();
        var actualGames = results.Select(tuple =>
        {
            var (game, error) = tuple;
            Assert.True(game is not null, error);
            return game;
        }).ToList();

        Assert.Equal(expectedGames.Count, actualGames.Count);
        Assert.Equal(expectedGames, actualGames);
    }

    [Fact]
    public void Test_ShouldWork_WithDuplicateKeys()
    {
        var fs = new MockFileSystem();
        var manifestDir = OriginHandler.GetManifestDir(fs);

        fs.AddDirectory(manifestDir.FullName);

        var manifest = fs.Path.Combine(manifestDir.FullName, "manifest.mfst");
        fs.AddFile(manifest, "?id=foo&ID=foo&dipInstallPath=bar&dipinstallpath=bar");

        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToList();
        Assert.Collection(results, result =>
        {
            Assert.Null(result.Error);
            Assert.Equal(new OriginGame("foo", "bar"), result.Game);
        });
    }

    [Fact]
    public void Test_ShouldError_NoManifestDir()
    {
        var fs = new MockFileSystem();
        var manifestDir = OriginHandler.GetManifestDir(fs);

        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToList();
        Assert.Collection(results,
            result =>
            {
                Assert.Null(result.Game);
                Assert.Equal($"Manifest folder {manifestDir.FullName} does not exist!", result.Error);
            });
    }

    [Fact]
    public void Test_ShouldError_NoManifests()
    {
        var fs = new MockFileSystem();
        var manifestDir = OriginHandler.GetManifestDir(fs);
        fs.AddDirectory(manifestDir.FullName);

        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToList();
        Assert.Collection(results,
            result =>
            {
                Assert.Null(result.Game);
                Assert.Equal($"Manifest folder {manifestDir.FullName} does not contain any .mfst files", result.Error);
            });
    }

    [Fact]
    public void Test_ShouldError_MissingValues()
    {
        var fs = new MockFileSystem();
        var manifestDir = OriginHandler.GetManifestDir(fs);

        fs.AddDirectory(manifestDir.FullName);

        var badManifest1 = fs.Path.Combine(manifestDir.FullName, "no-id.mfst");
        var badManifest2 = fs.Path.Combine(manifestDir.FullName, "no-dipInstallPath.mfst");
        fs.AddFile(badManifest1, "?dipInstallPath=foo");
        fs.AddFile(badManifest2, "?id=foo");

        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToList();
        Assert.Collection(results,
            result =>
            {
                Assert.Null(result.Game);
                Assert.Equal($"Manifest {badManifest1} does not have a value \"id\"", result.Error);
            },
            result =>
            {
                Assert.Null(result.Game);
                Assert.Equal($"Manifest {badManifest2} does not have a value \"dipInstallPath\"", result.Error);
            });
    }

    [Fact]
    public void Test_ShouldSkip_SteamGames()
    {
        var fs = new MockFileSystem();
        var manifestDir = OriginHandler.GetManifestDir(fs);

        fs.AddDirectory(manifestDir.FullName);

        var manifest = fs.Path.Combine(manifestDir.FullName, "steam.mfst");
        fs.AddFile(manifest, "?id=foo@steam&dipInstallPath=bar");

        var handler = new OriginHandler(fs);

        var results = handler.FindAllGames().ToList();
        Assert.Empty(results);
    }
}
