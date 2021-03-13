using System;
using System.IO;
using System.Text;
#if NET5_0
using System.Text.Json;
#endif
#if NETSTANDARD2_1
using Newtonsoft.Json;
#endif

namespace GameFinder
{
    internal static class Utils
    {
#if NET5_0
        internal static JsonSerializerOptions DefaultSerializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };
#endif
        
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

#if NET5_0
            var value = JsonSerializer.Deserialize<T>(jsonText, DefaultSerializerOptions);
            return value;
#endif
#if NETSTANDARD2_1
            var serializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            using var textReader = new StringReader(jsonText);
            using var reader = new JsonTextReader(textReader);
            var value = serializer.Deserialize<T>(reader);
            return value;
#endif
        }
    }
}
