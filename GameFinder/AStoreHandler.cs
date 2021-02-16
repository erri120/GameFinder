using System.Collections.Generic;
using JetBrains.Annotations;

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
        /// Initialize the Store Handler
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        /// Find all games, <see cref="Init"/> has to be called beforehand!
        /// </summary>
        /// <returns></returns>
        public abstract bool FindAllGames();

        /// <summary>
        /// Get Game by ID
        /// </summary>
        /// <param name="id">ID of the game</param>
        /// <returns></returns>
        public abstract TGame? GetByID(int id);
    }
}
