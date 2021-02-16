using System;

namespace GameFinder
{
    internal static class Utils
    {
        internal static bool ContainsCaseInsensitive(this string s, string value)
        {
            return s.Contains(value, StringComparison.OrdinalIgnoreCase);
        }

        internal static DateTime ToDateTime(this long timeStamp)
        {
            var offset = DateTimeOffset.FromUnixTimeSeconds(timeStamp);
            return offset.UtcDateTime;
        }
    }
}
