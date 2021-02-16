using JetBrains.Annotations;

namespace GameFinder
{
    [PublicAPI]
    public abstract class AStoreGame
    {
        public virtual string Name { get; internal set; } = string.Empty;

        public virtual string Path { get; internal set; } = string.Empty;

        public abstract StoreType StoreType { get; }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
