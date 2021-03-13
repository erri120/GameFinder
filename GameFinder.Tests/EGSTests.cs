using System.Linq;
using GameFinder.StoreHandlers.EGS;
using Xunit;

namespace GameFinder.Tests
{
    public class EGSTests : AStoreHandlerTest<EGSHandler, EGSGame>
    {
        protected override EGSHandler DoSetup()
        {
            var manifestDir = Setup.SetupEpicGamesStore();
            return new EGSHandler(manifestDir);
        }

        protected override void ChecksAfterFindingGames(EGSHandler storeHandler)
        {
            base.ChecksAfterFindingGames(storeHandler);
            var game = storeHandler.Games.FirstOrDefault(x =>
                x.InstallationGuid != null && x.InstallationGuid.Equals("8AAFB83044E76B812D3D8C9652E8C13C"));
            Assert.NotNull(game);
        }
    }
}
