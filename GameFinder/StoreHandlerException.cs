using System;
using JetBrains.Annotations;

namespace GameFinder
{
    /// <summary>
    /// Exception that occured in one of the <see cref="AStoreHandler{TGame}"/>.
    /// </summary>
    [PublicAPI]
    public class StoreHandlerException : Exception
    {
        /// <summary>
        /// <see cref="StoreType"/> of the <see cref="AStoreHandler{TGame}"/> with the Exception.
        /// </summary>
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
