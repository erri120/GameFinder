using System.Collections.Generic;
using System.Linq;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.Steam;

internal static class EnumerableExtensions
{
    public static Size Sum(this IEnumerable<Size> enumerable)
    {
        return enumerable.Aggregate(Size.Zero, (total, current) => total + current);
    }
}
