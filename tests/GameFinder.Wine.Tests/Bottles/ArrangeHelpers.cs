using GameFinder.Wine.Bottles;
using NexusMods.Paths;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    private static (AbsolutePath prefixDirectory, BottlesWinePrefixManager prefixManager) CreateBottle
        (InMemoryFileSystem fs, string bottleName)
    {
        var bottlesDirectory = fs
            .GetKnownPath(KnownPath.LocalApplicationDataDirectory)
            .CombineUnchecked("bottles");

        var prefixDirectory = bottlesDirectory.CombineUnchecked("bottles").CombineUnchecked(bottleName);
        fs.AddDirectory(prefixDirectory);

        var prefixManager = new BottlesWinePrefixManager(fs);
        return (prefixDirectory, prefixManager);
    }

    private static (AbsolutePath prefixDirectory, BottlesWinePrefixManager prefixManager) SetupValidBottlesPrefix
        (InMemoryFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = CreateBottle(fs, bottleName);

        fs.AddDirectory(prefixDirectory);
        fs.AddDirectory(prefixDirectory.CombineUnchecked("drive_c"));
        fs.AddEmptyFile(prefixDirectory.CombineUnchecked("system.reg"));
        fs.AddEmptyFile(prefixDirectory.CombineUnchecked("user.reg"));
        fs.AddEmptyFile(prefixDirectory.CombineUnchecked("bottle.yml"));

        return (prefixDirectory, prefixManager);
    }
}
