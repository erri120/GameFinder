using System.Text.Json.Serialization;

namespace GameFinder.StoreHandlers.EGS;

[JsonSourceGenerationOptions(WriteIndented = false, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(ManifestFile))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
