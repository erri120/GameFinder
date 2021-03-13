using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Xbox
{
    [PublicAPI]
    public class XboxException : StoreHandlerException
    {
        internal XboxException(string message) : base(StoreType.Xbox, message) { }

        internal XboxException(string message, Exception e) : base(StoreType.Xbox, message, e) { }
    }

    [PublicAPI]
    public class XboxAppNotFoundException : XboxException
    {
        internal XboxAppNotFoundException(string message) : base(message) { }
    }
}
