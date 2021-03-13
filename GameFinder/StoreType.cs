using System.ComponentModel;
using JetBrains.Annotations;

namespace GameFinder
{
    [PublicAPI]
    public enum StoreType
    {
        /// <summary>
        /// Steam
        /// </summary>
        [Description("Steam")]
        Steam,
        
        /// <summary>
        /// GOG
        /// </summary>
        [Description("GOG")]
        GOG,
        
        /// <summary>
        /// Bethesda Launcher
        /// </summary>
        [Description("Bethesda.net Launcher")]
        BethNet,
        
        /// <summary>
        /// Origin
        /// </summary>
        [Description("Origin")]
        Origin,
        
        /// <summary>
        /// Epic Games store
        /// </summary>
        [Description("Epic Games Store")]
        EpicGamesStore,
        
        /// <summary>
        /// Xbox Game Pass
        /// </summary>
        [Description("Xbox Game Pass")]
        Xbox
    }
}
