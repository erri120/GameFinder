using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GameFinder.StoreHandlers.Origin;
using Xunit;
using Xunit.Abstractions;

namespace GameFinder.Tests
{
    public class OriginTests : AStoreHandlerTest<OriginHandler, OriginGame>
    {
        protected override OriginHandler DoSetup()
        {
            var localContentPath = Setup.SetupOrigin();
            var handler = new OriginHandler(localContentPath, true, true, Logger);
            return handler;
        }

        protected override void ChecksAfterFindingGames(OriginHandler storeHandler)
        {
            var game = storeHandler.Games.FirstOrDefault(x => x.Id == "Origin.OFR.50.0001131");
            Assert.NotNull(game);

            Assert.False(string.IsNullOrWhiteSpace(game?.Id));
            Assert.False(string.IsNullOrWhiteSpace(game?.Name));
            Assert.False(string.IsNullOrWhiteSpace(game?.Path));
        }

        [Theory]
        [InlineData("Origin.OFR.50.0001131")]
        public async Task TestApiResponse(string id)
        {
            var res = await OriginHandler.GetApiResponse(id, Logger);
            Assert.NotNull(res);
        }

        [Fact]
        public void TestManifestParsing()
        {
            var path = Path.Combine(Setup.GetCurrentDir(), "files", "origin", "Dragon Age Inquisition", "__Installer",
                "installerdata.xml");
            Assert.True(File.Exists(path));

            var res = OriginHandler.GetManifest(path, Logger);
            Assert.NotNull(res);
        }
        
        public OriginTests(ITestOutputHelper output) : base(output)
        {
        }
    }
}
