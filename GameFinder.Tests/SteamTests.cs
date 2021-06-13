using System.IO;
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
            base.ChecksAfterFindingGames(storeHandler);
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

        [Fact]
        public void TestConfigParsing()
        {
            const string file = "files\\steam\\config.vdf";
            Assert.True(File.Exists(file));

            var res = SteamHandler.ParseSteamConfig(file);
            var paths = res.Value;
            
            Assert.False(res.HasErrors);
            Assert.Equal(3, paths.Count);
            Assert.Contains(paths, x => x.Equals("F:\\SteamLibrary"));
            Assert.Contains(paths, x => x.Equals("M:\\SteamLibrary"));
            Assert.Contains(paths, x => x.Equals("E:\\SteamLibrary"));
        }

        [Theory]
        [InlineData("files\\steam\\libraryfolders-old.vdf")]
        [InlineData("files\\steam\\libraryfolders-new.vdf")]
        public void TestLibraryFoldersParsing(string file)
        {
            Assert.True(File.Exists(file));

            var res = SteamHandler.ParseLibraryFolders(file);
            var paths = res.Value;
            
            Assert.False(res.HasErrors);
            Assert.Equal(2, paths.Count);
            Assert.Contains(paths, x => x.Equals("F:\\SteamLibrary"));
            Assert.Contains(paths, x => x.Equals("E:\\SteamLibrary"));
        }
        
        [Fact]
        public void TestAcfFileParsing()
        {
            const string file = "files\\steam\\appmanifest_8930.acf";
            Assert.True(File.Exists(file));

            var res = SteamHandler.ParseAcfFile(file);
            Assert.False(res.HasErrors);
            
            var game = res.Value;
            Assert.Equal(8930, game.ID);
            Assert.Equal("Sid Meier's Civilization V", game.Name);
            Assert.Equal("Sid Meier's Civilization V", game.Path);
            Assert.Equal(9235434479, game.SizeOnDisk);
            Assert.Equal(86404, game.BytesDownloaded);
            Assert.Equal(86403, game.BytesToDownload);
            Assert.Equal(11499, game.BytesToStage);
            Assert.Equal(114992, game.BytesStaged);
            Assert.Equal(((long) 1600350073).ToDateTime(), game.LastUpdated);
        }
    }
}
