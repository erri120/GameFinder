using System;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Origin
{
    [PublicAPI]
    public class OriginHandler : AStoreHandler<OriginGame>
    {
        public override StoreType StoreType => StoreType.Origin;

        private readonly bool _useLocalFiles;
        private readonly bool _useApi;

        private readonly string? _localContentPath;
        
        public OriginHandler(bool useLocalFiles, bool useApi)
        {
            _useLocalFiles = useLocalFiles;
            _useApi = useApi;

            if (!_useLocalFiles && !_useApi)
                throw new ArgumentException("Local ");
        }

        public OriginHandler(string localContentPath, bool useLocalFiles, bool useApi) : this(useLocalFiles, useApi)
        {
            _localContentPath = localContentPath;
        }
        
        public override Result<bool> FindAllGames()
        {
            throw new System.NotImplementedException();
        }
    }
}
