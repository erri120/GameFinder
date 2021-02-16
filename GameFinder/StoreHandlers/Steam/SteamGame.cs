using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Steam
{
    [PublicAPI]
    public class SteamGame : AStoreGame
    {
        public override StoreType StoreType => StoreType.Steam;

        public int ID { get; internal set; } = -1;

        public DateTime LastUpdated { get; internal set; } = DateTime.UnixEpoch;
        
        public long SizeOnDisk { get; internal set; }
        
        public long BytesToDownload { get; internal set; }
        
        public long BytesDownloaded { get; internal set; }
        
        public long BytesToStage { get; internal set; }
        
        public long BytesStaged { get; internal set; }
        
        public override string ToString()
        {
            return $"{Name} ({ID})";
        }
    }
}
