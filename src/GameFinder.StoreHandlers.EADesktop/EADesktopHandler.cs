using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Runtime.Versioning;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

[PublicAPI]
public class EADesktopHandler : AHandler<EADesktopGame, string>
{
    private readonly IFileSystem _fileSystem;
    private readonly IRegistry _registry;

    [SupportedOSPlatform("windows")]
    public EADesktopHandler() : this(new FileSystem(), new WindowsRegistry()) { }

    public EADesktopHandler(IFileSystem fileSystem, IRegistry registry)
    {
        _fileSystem = fileSystem;
        _registry = registry;
    }

    /// <inheritdoc/>
    public override IEnumerable<Result<EADesktopGame>> FindAllGames()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override IDictionary<string, EADesktopGame> FindAllGamesById(out string[] errors)
    {
        throw new NotImplementedException();
    }
}
