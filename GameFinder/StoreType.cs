using System.ComponentModel;
using JetBrains.Annotations;

namespace GameFinder
{
    [PublicAPI]
    public enum StoreType
    {
        [Description("Steam")]
        Steam,
        [Description("GOG")]
        GOG,
        [Description("Bethesda Launcher")]
        BethNet,
        [Description("Origin")]
        Origin,
        [Description("Epic Games Store")]
        EpicGamesStore
    }
}
