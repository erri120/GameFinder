using System.Collections.Generic;
using JetBrains.Annotations;
#if NET5_0
using System.Text.Json.Serialization;
#else
using Newtonsoft.Json;
#endif

namespace GameFinder.StoreHandlers.Xbox.DTO
{
    [PublicAPI]
    public class TitleHistoryResponse
    {
        [PublicAPI]
        public class Title
        {
#if NET5_0
            [JsonPropertyName("pfn")]
#else 
            [JsonProperty("pfn")]
#endif
            public string? PackageFamilyName { get; set; }
#if NET5_0
            [JsonPropertyName("type")]
#else 
            [JsonProperty("type")]
#endif
            public string? Type { get; set; }
        }
        
#if NET5_0
        [JsonPropertyName("titles")]
#else 
        [JsonProperty("titles")]
#endif
        public List<Title>? Titles { get; set; }
    }
}
