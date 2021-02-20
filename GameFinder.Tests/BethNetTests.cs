using System.Linq;
using GameFinder.StoreHandlers.BethNet;
using Xunit;

namespace GameFinder.Tests
{
    public class BethNetTests : AStoreHandlerTest<BethNetHandler, BethNetGame>
    {
        protected override BethNetHandler DoSetup()
        {
            Setup.SetupBethNet();
            return new BethNetHandler();
        }

        protected override void ChecksAfterFindingGames(BethNetHandler storeHandler)
        {
            var game = storeHandler.Games.FirstOrDefault(x => x.ID.Equals(8));
            Assert.NotNull(game);
            Assert.Equal(8, game!.ID);
            Assert.Equal("Fallout Shelter", game!.Name);
            Assert.NotEqual(string.Empty, game!.Path);
        }

        protected override void DoCleanup()
        {
            Setup.CleanupBethNet();
        }
    }
}
