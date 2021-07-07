using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Origin
{
    [PublicAPI]
    public class OriginGame : AStoreGame
    {
        public override StoreType StoreType => StoreType.Origin;
    }
}
