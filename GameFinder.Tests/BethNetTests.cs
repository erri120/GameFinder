using System.Linq;
using GameFinder.StoreHandlers.BethNet;
using Xunit;
using Xunit.Abstractions;

namespace GameFinder.Tests
{
    public class BethNetTests : AStoreHandlerTest<BethNetHandler, BethNetGame>
    {
        protected override BethNetHandler DoSetup()
        {
            Setup.SetupBethNet();
            return new BethNetHandler(Logger);
        }

        protected override void ChecksAfterFindingGames(BethNetHandler storeHandler)
        {
            base.ChecksAfterFindingGames(storeHandler);
            var game = storeHandler.Games.FirstOrDefault(x => x.ID.Equals(8));
            Assert.NotNull(game);
            Assert.Equal(8, game!.ID);
            Assert.Equal("Fallout Shelter", game!.Name);
            Assert.NotEqual(string.Empty, game!.Path);
        }

        protected override void DoCleanup()
        {
            //does not work atm
            Setup.CleanupBethNet();
        }

        public BethNetTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
