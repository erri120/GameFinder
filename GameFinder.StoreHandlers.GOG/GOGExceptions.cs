using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.GOG
{
    [PublicAPI]
    public class GOGException : StoreHandlerException
    {
        public GOGException(string message) : base(StoreType.GOG, message) { }

        public GOGException(string message, Exception e) : base(StoreType.GOG, message, e) { }
    }

    [PublicAPI]
    public sealed class GOGNotFoundException : GOGException
    {
        public GOGNotFoundException([NotNull] string message) : base(message) { }

        public GOGNotFoundException([NotNull] string message, [NotNull] Exception e) : base(message, e) { }
    }
}
