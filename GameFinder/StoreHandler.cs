using System;
using JetBrains.Annotations;

namespace GameFinder
{
    [PublicAPI]
    public class StoreHandler
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Lazy<StoreHandler> _instance = new(() => new StoreHandler(), true);
        public static StoreHandler Instance => _instance.Value;

        public StoreHandler()
        {
            
        }
    }
}
