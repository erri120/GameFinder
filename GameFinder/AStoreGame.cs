using JetBrains.Annotations;

namespace GameFinder
{
    /// <summary>
    /// Abstract class of a Store Game
    /// </summary>
    [PublicAPI]
    public abstract class AStoreGame
    {
        /// <summary>
        /// Name of the Game
        /// </summary>
        public virtual string Name { get; internal set; } = string.Empty;

        /// <summary>
        /// Path to the Game
        /// </summary>
        public virtual string Path { get; internal set; } = string.Empty;

        /// <summary>
        /// Store Type
        /// </summary>
        public abstract StoreType StoreType { get; }

        /// <summary>
        /// String representation if the Game
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
