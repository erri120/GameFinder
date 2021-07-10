using GameFinder.StoreHandlers.Xbox;
using Xunit.Abstractions;

namespace GameFinder.Tests
{
    public class XboxTests : AStoreHandlerTest<XboxHandler, XboxGame>
    {
        protected override XboxHandler DoSetup()
        {
            return new XboxHandler(Logger);
        }

        protected override void ChecksAfterFindingGames(XboxHandler storeHandler)
        {
            //don't check if we found any games when using CI because it does not have any UWP apps installed
            if (TestUtils.IsCI)
                return;
            base.ChecksAfterFindingGames(storeHandler);
        }

        public XboxTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
