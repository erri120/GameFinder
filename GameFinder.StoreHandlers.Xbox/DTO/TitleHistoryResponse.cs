using System.Collections.Generic;
using JetBrains.Annotations;
using System.Text.Json.Serialization;

namespace GameFinder.StoreHandlers.Xbox.DTO
{
    [PublicAPI]
    public class TitleHistoryResponse
    {
        [PublicAPI]
        public class Title
        {
            [JsonPropertyName("pfn")]
            public string? PackageFamilyName { get; set; }
            [JsonPropertyName("type")]
            public string? Type { get; set; }
        }
        
        [JsonPropertyName("titles")]
        public List<Title>? Titles { get; set; }
    }
}
