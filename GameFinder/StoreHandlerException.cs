using System;
using JetBrains.Annotations;

namespace GameFinder
{
    [PublicAPI]
    public class StoreHandlerException : Exception
    {
        public StoreType StoreType { get; internal set; }
        
        public StoreHandlerException(StoreType storeType, string message) : base(message)
        {
            StoreType = storeType;
        }

        public StoreHandlerException(StoreType storeType, string message, Exception e) : base(message, e)
        {
            StoreType = storeType;
        }
    }
}
