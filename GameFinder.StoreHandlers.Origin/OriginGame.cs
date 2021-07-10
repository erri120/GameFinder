using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Origin
{
    [PublicAPI]
    public class OriginGame : AStoreGame
    {
        public override StoreType StoreType => StoreType.Origin;
        
        public string? Id { get; set; }
        
        public List<LocalizedString>? GameTitles { get; set; }

        #region Overrides
        
        /// <inheritdoc />
        public override int CompareTo(AStoreGame? other)
        {
            return other switch
            {
                null => 1,
                OriginGame originGame => string.CompareOrdinal(Id, originGame.Id),
                _ => base.CompareTo(other)
            };
        }
        
        /// <inheritdoc />
        public override bool Equals(AStoreGame? other)
        {
            return other switch
            {
                null => false,
                OriginGame originGame => string.Equals(Id, originGame.Id),
                _ => base.Equals(other)
            };
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj switch
            {
                null => false,
                OriginGame originGame => Equals(originGame),
                AStoreGame aStoreGame => base.Equals(aStoreGame),
                _ => false
            };
        }
        
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id == null ? 0 : Id.GetHashCode();
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return $"{base.ToString()} ({Id})";
        }
        
        #endregion
    }
    
    //TODO: use records
    [PublicAPI]
    public struct LocalizedString
    {
        public string? Locale;
        public string? Value;

        public LocalizedString(string? locale, string? value)
        {
            Locale = locale;
            Value = value;
        }
    }
}
