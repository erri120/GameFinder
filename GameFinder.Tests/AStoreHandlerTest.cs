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
            var res = storeHandler.FindAllGames();
            Assert.True(res.IsOk(), res.ErrorsToString());
            ChecksAfterFindingGames(storeHandler);
            DoCleanup();
        }

        protected virtual TStoreHandler DoSetup()
        {
            return new TStoreHandler();
        }
        
        protected virtual void ChecksBeforeFindingGames(TStoreHandler storeHandler) { }

        protected virtual void ChecksAfterFindingGames(TStoreHandler storeHandler)
        {
            Assert.NotEmpty(storeHandler.Games);
        }
        
        protected virtual void DoCleanup() { }
    }
}
