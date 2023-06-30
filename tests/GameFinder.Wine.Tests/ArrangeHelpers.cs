using NexusMods.Paths;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    private static (AbsolutePath prefixDirectory, DefaultWinePrefixManager prefixManager) SetupWinePrefix(InMemoryFileSystem fs)
    {
        var location = DefaultWinePrefixManager
            .GetDefaultWinePrefixLocations(fs)
            .First();

        fs.AddDirectory(location);
        return (location, new DefaultWinePrefixManager(fs));
    }

    private static (AbsolutePath prefixDirectory, DefaultWinePrefixManager prefixManager) SetupValidWinePrefix(InMemoryFileSystem fs, AbsolutePath location)
    {
        fs.AddDirectory(location);
        fs.AddDirectory(location.Combine("drive_c"));
        fs.AddEmptyFile(location.Combine("system.reg"));
        fs.AddEmptyFile(location.Combine("user.reg"));

        return (location, new DefaultWinePrefixManager(fs));
    }
}
