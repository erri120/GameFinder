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
            .Combine("bottles");

        var prefixDirectory = bottlesDirectory.Combine("bottles").Combine(bottleName);
        fs.AddDirectory(prefixDirectory);

        var prefixManager = new BottlesWinePrefixManager(fs);
        return (prefixDirectory, prefixManager);
    }

    private static (AbsolutePath prefixDirectory, BottlesWinePrefixManager prefixManager) SetupValidBottlesPrefix
        (InMemoryFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = CreateBottle(fs, bottleName);

        fs.AddDirectory(prefixDirectory);
        fs.AddDirectory(prefixDirectory.Combine("drive_c"));
        fs.AddEmptyFile(prefixDirectory.Combine("system.reg"));
        fs.AddEmptyFile(prefixDirectory.Combine("user.reg"));
        fs.AddEmptyFile(prefixDirectory.Combine("bottle.yml"));

        return (prefixDirectory, prefixManager);
    }
}
