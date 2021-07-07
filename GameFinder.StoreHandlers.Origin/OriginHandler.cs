using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Origin
{
    [PublicAPI]
    public class OriginHandler : AStoreHandler<OriginGame>
    {
        public override StoreType StoreType => StoreType.Origin;
        
        public override Result<bool> FindAllGames()
        {
            throw new System.NotImplementedException();
        }
    }
}
