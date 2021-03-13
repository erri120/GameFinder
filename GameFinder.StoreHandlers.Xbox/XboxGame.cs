using System;
using JetBrains.Annotations;
using FileAttributes = Windows.Storage.FileAttributes;

namespace GameFinder.StoreHandlers.Xbox
{
    [PublicAPI]
    public class XboxGame : AStoreGame
    {
        public override StoreType StoreType => StoreType.Xbox;
        
        public string? DisplayName { get; internal set; }
        
        public string? Description { get; internal set; }
        
        public string? FamilyName { get; internal set; }

        public string FullName { get; internal set; } = string.Empty;

        public string? Publisher { get; internal set; }
        
        public string? PublisherId { get; internal set; }
        
        public DateTimeOffset InstalledDate { get; internal set; }
        
        public string? InstalledLocationNameFolderRelativeId { get; internal set; }
        
        public string? InstalledLocationName { get; internal set; }
        
        public string? InstalledLocationDisplayType { get; internal set; }
        
        public string? InstalledLocationDisplayName { get; internal set; }
        
        public DateTimeOffset InstalledLocationDateCreated { get; internal set; }
        
        public FileAttributes InstalledLocationAttributes { get; internal set; }
        
        public Uri? Logo { get; internal set; }
        
        public string? PublisherDisplayName { get; internal set; }

        #region Overrides

        /// <inheritdoc />
        public override int CompareTo(AStoreGame? other)
        {
            return other switch
            {
                null => 1,
                XboxGame xboxGame => string.Compare(FullName, xboxGame.FullName, StringComparison.Ordinal),
                _ => base.CompareTo(other)
            };
        }
        
        /// <inheritdoc />
        public override bool Equals(AStoreGame? other)
        {
            return other switch
            {
                null => false,
                XboxGame xboxGame => FullName.Equals(xboxGame.FullName, StringComparison.Ordinal),
                _ => base.Equals(other)
            };
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj switch
            {
                null => false,
                XboxGame xboxGame => Equals(xboxGame),
                AStoreGame aStoreGame => base.Equals(aStoreGame),
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override string ToString()
        {
            return $"{DisplayName} ({Name})";
        }
        
        #endregion


    }
}
