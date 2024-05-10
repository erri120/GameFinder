using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using Xunit.Abstractions;

namespace TestUtils;

public class TestWrapper
{
    protected readonly ILogger Logger;
    protected readonly IFileSystem FileSystem;

    protected TestWrapper(ITestOutputHelper output)
    {
        Logger = new XunitLogger(output);
        FileSystem = NexusMods.Paths.FileSystem.Shared;
    }

    protected AbsolutePath GetTestFile(RelativePath fileName)
    {
        return FileSystem.GetKnownPath(KnownPath.EntryDirectory).Combine("files").Combine(fileName);
    }
}
