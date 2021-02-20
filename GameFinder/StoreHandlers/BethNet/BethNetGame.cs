using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.BethNet
{
    [PublicAPI]
    public class BethNetGame : AStoreGame
    {
        public override StoreType StoreType => StoreType.BethNet;

        public long ID { get; internal set; } = -1;

        /// <inheritdoc cref="AStoreGame.ToString"/>
        public override string ToString()
        {
            return $"{base.ToString()} ({ID})";
        }
    }
}
