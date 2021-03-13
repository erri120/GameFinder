using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam
{
    /// <summary>
    /// Steam Game
    /// </summary>
    [PublicAPI]
    public class SteamGame : AStoreGame
    {
        /// <inheritdoc cref="AStoreGame.StoreType"/>
        public override StoreType StoreType => StoreType.Steam;

        /// <summary>
        /// Steam ID of the game
        /// </summary>
        public int ID { get; internal set; } = -1;

        /// <summary>
        /// Time when the game was last updated
        /// </summary>
        public DateTime LastUpdated { get; internal set; } = DateTime.UnixEpoch;

        /// <summary>
        /// Size of the game on disk in bytes
        /// </summary>
        public long SizeOnDisk { get; internal set; } = -1;

        /// <summary>
        /// Amount of bytes to download
        /// </summary>
        public long BytesToDownload { get; internal set; } = -1;
        
        /// <summary>
        /// Amount of bytes already downloaded
        /// </summary>
        public long BytesDownloaded { get; internal set; } = -1;
        
        /// <summary>
        /// Amount of bytes to stage
        /// </summary>
        public long BytesToStage { get; internal set; } = -1;
        
        /// <summary>
        /// Amount of bytes already staged
        /// </summary>
        public long BytesStaged { get; internal set; } = -1;

        #region Overrides
        
        /// <inheritdoc />
        public override int CompareTo(AStoreGame? other)
        {
            return other switch
            {
                null => 1,
                SteamGame steamGame => ID.CompareTo(steamGame.ID),
                _ => base.CompareTo(other)
            };
        }
        
        /// <inheritdoc />
        public override bool Equals(AStoreGame? other)
        {
            return other switch
            {
                null => false,
                SteamGame steamGame => ID.Equals(steamGame.ID),
                _ => base.Equals(other)
            };
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj switch
            {
                null => false,
                SteamGame steamGame => Equals(steamGame),
                AStoreGame aStoreGame => base.Equals(aStoreGame),
                _ => false
            };
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ID;
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()} ({ID})";
        }
        
        #endregion
    }
}
