using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.EGS;

internal class AbsolutePathConverter : JsonConverter<AbsolutePath>
{
    public override AbsolutePath Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return FileSystem.Shared.FromFullPath(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, AbsolutePath value,JsonSerializerOptions options)
    {
        var spanLength = value.GetFullPathLength();
        var span = spanLength < 512
            ? stackalloc char[spanLength]
            : new char[spanLength];
        value.GetFullPath(span);
        writer.WriteStringValue(span);
    }
}
