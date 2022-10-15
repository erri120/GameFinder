using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Runtime.InteropServices;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EGS;
using Xunit;

namespace GameFinder.Tests;

public class EGSTests
{
    private static (InMemoryRegistry registry, MockFileSystem fileSystem, List<EGSGame> expectedGames) SetupTest()
    {
        var expectedGames = new List<EGSGame>
        {
            new("b8538c739273426aa35a98220e258d55", "Unreal Tournament", "C:\\Games\\UnrealTournament"),
            new("3257e06c28764231acd93049f3774ed6", "Subnautica", "C:\\Games\\Subnautica")
        };
        
        var registry = new InMemoryRegistry();

        var regKey = registry.AddKey(RegistryHive.CurrentUser, @"Software\Epic Games\EOS");

        var baseDir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "C:\\ModSdkMetadataDir"
            : "/ModSdkMetadataDir";
        
        regKey.AddValue("ModSdkMetadataDir", baseDir);
        
        var fileSystem = new MockFileSystem();

        foreach (var expectedGame in expectedGames)
        {
            var mockData = @$"{{
    ""CatalogItemId"": ""{expectedGame.CatalogItemId}"",
    ""DisplayName"": ""{expectedGame.DisplayName}"",
    ""InstallLocation"": ""{expectedGame.InstallLocation.Replace("\\", "\\\\")}""
}}";

            fileSystem.AddFile($"{Path.Combine(baseDir, $"{expectedGame.CatalogItemId}.item")}", new MockFileData(mockData));
        }

        return (registry, fileSystem, expectedGames);
    }

    [Fact]
    private void TestFindAllGames()
    {
        var (registry, fileSystem, expectedGames) = SetupTest();

        var handler = new EGSHandler(registry, fileSystem);
        
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
}

