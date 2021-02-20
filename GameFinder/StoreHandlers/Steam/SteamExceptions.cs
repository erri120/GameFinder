using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam
{
    [PublicAPI]
    public class SteamException : StoreHandlerException
    {
        public SteamException(string message) : base(StoreType.Steam, message) { }

        public SteamException(string message, Exception e) : base(StoreType.Steam, message, e) { }
    }

    [PublicAPI]
    public sealed class SteamNotFoundException : SteamException
    {
        public SteamNotFoundException([NotNull] string message) : base(message) { }

        public SteamNotFoundException([NotNull] string message, [NotNull] Exception e) : base(message, e) { }
    }

    [PublicAPI]
    public sealed class SteamUniverseNotFoundException : SteamException
    {
        public SteamUniverseNotFoundException([NotNull] string message) : base(message) { }

        public SteamUniverseNotFoundException([NotNull] string message, [NotNull] Exception e) : base(message, e) { }
    }

    [PublicAPI]
    public sealed class SteamGameNotFoundException : SteamException
    {
        public SteamGameNotFoundException([NotNull] string message) : base(message) { }

        public SteamGameNotFoundException([NotNull] string message, [NotNull] Exception e) : base(message, e) { }
    }
    
    [PublicAPI]
    public sealed class SteamVdfParsingException : SteamException
    {
        public readonly string Line;

        public SteamVdfParsingException(string line, [NotNull] string message) : base(message)
        {
            Line = line;
        }

        public SteamVdfParsingException(string line, [NotNull] string message, [NotNull] Exception e) : base(message, e)
        {
            Line = line;
        }
    }
}
