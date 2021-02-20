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

        internal static string RemoveQuotes(this string s)
        {
            var pos1 = s.IndexOf('"');
            var pos2 = s.LastIndexOf('"');

            if (pos1 == -1 || pos2 == -1) return s;

            var newString = s.Substring(pos1+1, pos2-1);
            return newString;
        }
    }
}
