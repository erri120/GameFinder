using System.IO.Abstractions.TestingHelpers;
using GameFinder.Wine.Bottles;

namespace GameFinder.Wine.Tests.Bottles;

public partial class BottlesTests
{
    private static (string prefixDirectory, BottlesWinePrefixManager prefixManager) CreateBottle
        (MockFileSystem fs, string bottleName)
    {
        var bottlesDirectory = fs.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "bottles"
        );

        var prefixDirectory = fs.Path.Combine(bottlesDirectory, "bottles", bottleName);
        fs.AddDirectory(prefixDirectory);

        var prefixManager = new BottlesWinePrefixManager(fs);
        return (prefixDirectory, prefixManager);
    }

    private static (string prefixDirectory, BottlesWinePrefixManager prefixManager) SetupValidBottlesPrefix
        (MockFileSystem fs, string bottleName)
    {
        var (prefixDirectory, prefixManager) = CreateBottle(fs, bottleName);

        fs.AddDirectory(fs.Path.Combine(prefixDirectory, "drive_c"));
        fs.AddEmptyFile(fs.Path.Combine(prefixDirectory, "system.reg"));
        fs.AddEmptyFile(fs.Path.Combine(prefixDirectory, "user.reg"));
        fs.AddEmptyFile(fs.Path.Combine(prefixDirectory, "bottle.yml"));

        return (prefixDirectory, prefixManager);
    }
}
