using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EGS
{
    [PublicAPI]
    public class EGSException : StoreHandlerException
    {
        internal EGSException(string message) : base(StoreType.EpicGamesStore, message) { }

        internal EGSException(string message, Exception e) : base(StoreType.EpicGamesStore, message, e) { }
    }
}
