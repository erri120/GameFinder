using System.Collections.Generic;
using JetBrains.Annotations;

namespace GameFinder
{
    [PublicAPI]
    public abstract class AStoreHandler<TGame> where TGame : AStoreGame
    {
        public List<TGame> Games { get; } = new();
        
        public abstract StoreType StoreType { get; }

        public abstract bool Init();

        public abstract bool FindAllGames();
    }
}
