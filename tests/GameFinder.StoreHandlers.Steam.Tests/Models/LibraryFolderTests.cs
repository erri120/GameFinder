using GameFinder.StoreHandlers.Steam.Models;
using GameFinder.StoreHandlers.Steam.Models.ValueTypes;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam.Tests.Models;

public class LibraryFolderTests
{
    [Fact]
    public void Test_Sizes()
    {
        var fixture = new Fixture();
        fixture.Customize<AppId>(composer => composer.FromFactory<Guid>(x => AppId.From((uint)x.GetHashCode())));

        var fs = new InMemoryFileSystem();
        var libraryFolder = new LibraryFolder
        {
            Path = fs.GetKnownPath(KnownPath.TempDirectory),
            TotalDiskSize = Size.GB * 100,
            AppSizes = new Dictionary<AppId, Size>
            {
                { fixture.Create<AppId>(), Size.GB * 2 },
                { fixture.Create<AppId>(), Size.GB * 8 },
            },
        };

        libraryFolder.GetTotalSizeOfInstalledApps().Should().Be(Size.GB * 10);
        libraryFolder.GetFreeSpaceEstimate().Should().Be(Size.GB * 90);
    }
}
