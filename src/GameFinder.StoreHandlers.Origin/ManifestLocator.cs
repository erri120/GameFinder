using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Origin;

/// <summary>
/// Locator methods.
/// </summary>
public static class ManifestLocator
{
    /// <summary>
    /// Returns the path to the default manifest directory.
    /// </summary>
    public static AbsolutePath GetManifestDirectory(IFileSystem fileSystem)
    {
        return fileSystem
            .GetKnownPath(KnownPath.CommonApplicationDataDirectory)
            .Combine("Origin")
            .Combine("LocalContent");
    }
}
