using GameFinder.StoreHandlers.Xbox;

namespace GameFinder.Tests
{
    public class XboxTests : AStoreHandlerTest<XboxHandler, XboxGame>
    {
        protected override void ChecksAfterFindingGames(XboxHandler storeHandler)
        {
            //don't check if we found any games when using CI because it does not have any UWP apps installed
            if (TestUtils.IsCI)
                return;
            base.ChecksAfterFindingGames(storeHandler);
        }
    }
}
