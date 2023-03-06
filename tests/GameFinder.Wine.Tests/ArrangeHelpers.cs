using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;

namespace GameFinder.Wine.Tests;

public partial class WineTests
{
    private static (IDirectoryInfo prefixDirectory, DefaultWinePrefixManager prefixManager) SetupWinePrefix(IFileSystem fs)
    {
        var location = DefaultWinePrefixManager
            .GetDefaultWinePrefixLocations(fs)
            .First();

        return (fs.Directory.CreateDirectory(location), new DefaultWinePrefixManager(fs));
    }

    private static (IDirectoryInfo prefixDirectory, DefaultWinePrefixManager
        prefixManager) SetupValidWinePrefix(MockFileSystem fs, string location)
    {
        var prefix = fs.Directory.CreateDirectory(location);
        fs.AddDirectory(fs.Path.Combine(prefix.FullName, "drive_c"));
        fs.AddEmptyFile(fs.Path.Combine(prefix.FullName, "system.reg"));
        fs.AddEmptyFile(fs.Path.Combine(prefix.FullName, "user.reg"));

        return (prefix, new DefaultWinePrefixManager(fs));
    }
}
