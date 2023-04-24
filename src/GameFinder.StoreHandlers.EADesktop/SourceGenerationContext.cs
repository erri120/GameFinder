using System.Text.Json.Serialization;

namespace GameFinder.StoreHandlers.EADesktop;

[JsonSourceGenerationOptions(WriteIndented = false, GenerationMode = JsonSourceGenerationMode.Default)]
[JsonSerializable(typeof(InstallInfoFile))]
internal partial class SourceGenerationContext : JsonSerializerContext { }
