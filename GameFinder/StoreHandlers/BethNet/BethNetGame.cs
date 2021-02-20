using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.BethNet
{
    [PublicAPI]
    public class BethNetGame : AStoreGame
    {
        public override StoreType StoreType => StoreType.BethNet;

        public long ID { get; internal set; } = -1;

        #region Overrides
        
        /// <inheritdoc />
        public override int CompareTo(AStoreGame? other)
        {
            return other switch
            {
                null => 1,
                BethNetGame game => ID.CompareTo(game.ID),
                _ => base.CompareTo(other)
            };
        }
        
        /// <inheritdoc />
        public override bool Equals(AStoreGame? other)
        {
            return other switch
            {
                null => false,
                BethNetGame game => ID.Equals(game.ID),
                _ => base.Equals(other)
            };
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj switch
            {
                null => false,
                BethNetGame game => Equals(game),
                AStoreGame aStoreGame => base.Equals(aStoreGame),
                _ => false
            };
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()} ({ID})";
        }
        
        #endregion
    }
}
