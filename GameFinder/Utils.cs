using System;
using System.IO;
using System.Text;
using System.Text.Json;

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

        internal static T? FromJson<T>(string file) where T : class
        {
            if (!File.Exists(file))
                throw new FileNotFoundException(null, file);

            var jsonText = File.ReadAllText(file, Encoding.UTF8);
            var value = JsonSerializer.Deserialize<T>(jsonText, new JsonSerializerOptions
            {
                IgnoreNullValues = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });
            return value;
        }
    }
}
