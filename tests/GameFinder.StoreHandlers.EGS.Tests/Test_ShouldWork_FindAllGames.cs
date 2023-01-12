﻿using System.IO.Abstractions.TestingHelpers;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EGS.Tests.AutoData;
using TestUtils;

namespace GameFinder.StoreHandlers.EGS.Tests;

public partial class EGSTests
{
    [Theory, EGSAutoData]
    public void Test_ShouldWork_FindAllGames(MockFileSystem fs, InMemoryRegistry registry)
    {
        var (handler, manifestDir) = SetupHandler(fs, registry);
        var expectedGames = SetupGames(fs, manifestDir);

        var results = handler.FindAllGames().ToArray();
        var games = results.ShouldOnlyBeGames();

        games.Should().Equal(expectedGames);
    }
}