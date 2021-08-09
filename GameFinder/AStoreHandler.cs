using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace GameFinder
{
    /// <summary>
    /// Abstract class for Store Handlers
    /// </summary>
    /// <typeparam name="TGame"><see cref="AStoreGame"/> of the Store Handler</typeparam>
    [PublicAPI]
    public abstract class AStoreHandler<TGame> where TGame : AStoreGame
    {
        /// <summary>
        /// List of all found Games
        /// </summary>
        public List<TGame> Games { get; } = new();
        
        /// <summary>
        /// Store Type
        /// </summary>
        public abstract StoreType StoreType { get; }

        /// <summary>
        /// Find all games
        /// </summary>
        /// <returns></returns>
        public abstract bool FindAllGames();

        protected readonly ILogger Logger;
        
        protected AStoreHandler(ILogger logger)
        {
            Logger = logger;
        }
    }
}
