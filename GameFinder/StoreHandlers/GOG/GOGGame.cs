using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.GOG
{
    [PublicAPI]
    public class GOGGame : AStoreGame
    {
        /// <inheritdoc cref="AStoreGame.StoreType"/>
        public override StoreType StoreType => StoreType.GOG;

        public int GameID { get; internal set; } = -1;

        public int ProductID { get; internal set; } = -1;

        public long BuildID { get; internal set; } = -1;
        
        public string? EXE { get; internal set; }
        
        public string? EXEFile { get; internal set; }
        
        public DateTime InstallationDate { get; internal set; } = DateTime.UnixEpoch;
        
        public string? InstallerLanguage { get; internal set; }
        
        public string? LangCode { get; internal set; }
        
        public string? Language { get; internal set; }
        
        public string? LaunchCommand { get; internal set; }
        
        public string? LaunchParam { get; internal set; }
        
        public string? SaveGameFolder { get; internal set; }
        
        public string? StartMenu { get; internal set; }
        
        public string? StartMenuLink { get; internal set; }
        
        public string? SupportLink { get; internal set; }
        
        public string? UninstallCommand { get; internal set; }
        
        public string? Version { get; internal set; }
        
        public string? WorkingDir { get; internal set; }
        
#region Overrides
        
        /// <inheritdoc />
        public override int CompareTo(AStoreGame? other)
        {
            return other switch
            {
                null => 1,
                GOGGame game => GameID.CompareTo(game.GameID),
                _ => base.CompareTo(other)
            };
        }
        
        /// <inheritdoc />
        public override bool Equals(AStoreGame? other)
        {
            return other switch
            {
                null => false,
                GOGGame game => GameID.Equals(game.GameID),
                _ => base.Equals(other)
            };
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj switch
            {
                null => false,
                GOGGame game => Equals(game),
                AStoreGame aStoreGame => base.Equals(aStoreGame),
                _ => false
            };
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return GameID;
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()} ({GameID})";
        }
        
        #endregion
    }
}
