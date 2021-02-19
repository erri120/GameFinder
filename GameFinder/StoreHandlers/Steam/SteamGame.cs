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
        
        /// <inheritdoc cref="AStoreGame.ToString"/>
        public override string ToString()
        {
            return $"{Name} ({ID})";
        }
    }
}
