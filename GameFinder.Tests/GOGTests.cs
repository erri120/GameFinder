using System.Linq;
using GameFinder.StoreHandlers.GOG;
using Xunit;

namespace GameFinder.Tests
{
    public class GOGTests : AStoreHandlerTest<GOGHandler, GOGGame>
    {
        protected override GOGHandler DoSetup()
        {
            Setup.SetupGOG();
            return new GOGHandler();
        }

        protected override void ChecksAfterFindingGames(GOGHandler storeHandler)
        {
            base.ChecksAfterFindingGames(storeHandler);
            var game = storeHandler.Games.FirstOrDefault(x => x.GameID.Equals(1971477531));
            Assert.NotNull(game);
            Assert.Equal(1971477531, game!.GameID);
            Assert.Equal(1971477531, game!.ProductID);
            Assert.Equal(54099623651166556, game!.BuildID);
            Assert.Equal("Gwent", game!.Name);
            Assert.Equal("english", game!.InstallerLanguage);
            Assert.Equal("english", game!.Language);
            Assert.Equal("en-US", game!.LangCode);
            Assert.NotNull(game.EXE);
            Assert.NotNull(game.LaunchCommand);
            Assert.Equal("Gwent", game!.StartMenu);
            Assert.NotNull(game.StartMenuLink);
            Assert.NotNull(game.UninstallCommand);
            Assert.Equal("8.2", game!.Version);
            Assert.NotNull(game.WorkingDir);
        }

        protected override void DoCleanup()
        {
            Setup.CleanupGOG();
        }
    }
}
