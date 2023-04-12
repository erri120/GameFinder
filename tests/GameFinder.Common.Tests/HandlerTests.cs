using NexusMods.Paths;

namespace GameFinder.Common.Tests;

public class HandlerTests
{
    [SkippableTheory]
    [InlineData(" / ", "/", true)]
    [InlineData(" C:\\ ", "C:\\", false)]
    [InlineData("\\foo\\bar", "/foo/bar", true)]
    [InlineData("C:/foo/bar", "C:\\foo\\bar", false)]
    [InlineData("/foo/", "/foo", true)]
    [InlineData("/foo//////////////", "/foo", true)]
    [InlineData("/", "/", true)]
    [InlineData("C:\\foo\\", "C:\\foo", false)]
    [InlineData("C:\\foo\\\\\\\\\\\\\\\\\\\\\\\\", "C:\\foo", false)]
    [InlineData("C:\\", "C:\\", false)]
    public void Test_SanitizeInputPath(string input, string expected, bool linux)
    {
        Skip.IfNot(OperatingSystem.IsLinux() && linux);

        var actual = Utils.SanitizeInputPath(input);
        actual.Should().Be(expected);

        var fs = new InMemoryFileSystem();
        fs
            .Invoking(x => x.FromFullPath(actual))
            .Should().NotThrow();
    }
}
