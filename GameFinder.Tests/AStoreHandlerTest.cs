using Xunit;

namespace GameFinder.Tests
{
    public abstract class AStoreHandlerTest<TStoreHandler, TGame> 
        where TStoreHandler : AStoreHandler<TGame>, new()
        where TGame : AStoreGame
    {
        [Fact]
        protected void TestStoreHandler()
        {
            var storeHandler = DoSetup();
            ChecksBeforeFindingGames(storeHandler);
            Assert.True(storeHandler.FindAllGames());
            Assert.NotEmpty(storeHandler.Games);
            ChecksAfterFindingGames(storeHandler);
            DoCleanup();
        }

        protected virtual TStoreHandler DoSetup()
        {
            return new TStoreHandler();
        }
        
        protected virtual void ChecksBeforeFindingGames(TStoreHandler storeHandler) { }
        protected virtual void ChecksAfterFindingGames(TStoreHandler storeHandler) { }
        protected virtual void DoCleanup() { }
    }
}
