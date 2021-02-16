using GameFinder.StoreHandlers.Steam;
using Xunit;

namespace GameFinder.Tests
{
    public class SteamTests
    {
        public SteamTests()
        {
            Setup.SetupSteam();
        }
        
        [Fact]
        public void TestSteamHandler()
        {
            var steamHandler = new SteamHandler();
            Assert.True(steamHandler.Init());
            Assert.NotNull(steamHandler.SteamPath);
            Assert.True(steamHandler.FindAllGames());
            Assert.NotEmpty(steamHandler.Games);
        }
    }
}
