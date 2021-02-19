using GameFinder.StoreHandlers.Steam;
using Xunit;

namespace GameFinder.Tests
{
    public class SteamTests : AStoreHandlerTest<SteamHandler, SteamGame>
    {
        protected override SteamHandler DoSetup()
        {
            var steamDir = Setup.SetupSteam();
            return new SteamHandler(steamDir);
        }

        protected override void ChecksAfterFindingGames(SteamHandler storeHandler)
        {
            Assert.True(storeHandler.TryGetByID(72850, out var skyrim));
            Assert.NotNull(skyrim);
            Assert.Equal(72850, skyrim!.ID);
            Assert.Equal("The Elder Scrolls V: Skyrim", skyrim!.Name);
            Assert.Equal(9255977546, skyrim!.SizeOnDisk);
            Assert.Equal(7315266464, skyrim!.BytesDownloaded);
            Assert.Equal(7315266464, skyrim!.BytesToDownload);
            Assert.Equal(9255977546, skyrim!.BytesStaged);
            Assert.Equal(9255977546, skyrim!.BytesToStage);

            var lastUpdated = Utils.ToDateTime(1611048743);
            Assert.Equal(lastUpdated, skyrim!.LastUpdated);
        }
    }
}
