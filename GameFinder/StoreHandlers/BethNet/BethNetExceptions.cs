using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.BethNet
{
    [PublicAPI]
    public class BethNetException : StoreHandlerException
    {
        public BethNetException([NotNull] string message) : base(StoreType.BethNet, message) { }

        public BethNetException([NotNull] string message, [NotNull] Exception e) : base(StoreType.BethNet, message, e) { }
    }

    [PublicAPI]
    public sealed class BethNetNotFoundException : BethNetException
    {
        public BethNetNotFoundException([NotNull] string message) : base(message) { }

        public BethNetNotFoundException([NotNull] string message, [NotNull] Exception e) : base(message, e) { }
    }

    [PublicAPI]
    public sealed class BethNetGameNotFoundException : BethNetException
    {
        public BethNetGameNotFoundException([NotNull] string message) : base(message) { }

        public BethNetGameNotFoundException([NotNull] string message, [NotNull] Exception e) : base(message, e) { }
    }
}
