using System.Collections.Generic;
#if NET5_0
using System.Text.Json.Serialization;
#endif
#if NETSTANDARD2_1
using Newtonsoft.Json;
#endif
using JetBrains.Annotations;

#pragma warning disable 1591

namespace GameFinder.StoreHandlers.EGS
{
    [PublicAPI]
    public interface IEGSManifest
    {
        public string? AppVersionString { get; set; }
        
        public string? LaunchCommand { get; set; }
        
        public string? LaunchExecutable { get; set; }
        
        public string? ManifestLocation { get; set; }
        
        public bool IsApplication { get; set; }
        
        public bool IsExecutable { get; set; }
        
        public bool IsManaged { get; set; }
        
        public bool NeedsValidation { get; set; }
        
        public bool RequiresAuth { get; set; }
        
        public bool CanRunOffline { get; set; }
        
        public bool AllowMultipleInstances { get; set; }
        
        public string? AppName { get; set; }
        
        public string? CatalogItemId { get; set; }
        
        public string? CatalogNamespace { get; set; }
        
        public List<string>? AppCategories { get; set; }
        
        public string? DisplayName { get; set; }
        
        public string? FullAppName { get; set; }
        
        public string? InstallationGuid { get; set; }
        
        public string? InstallLocation { get; set; }
        
        public string? InstallSessionId { get; set; }
        
        public string? StagingLocation { get; set; }
        
        public string? TechnicalType { get; set; }
        
        public long InstallSize { get; set; }
        
        public string? MainGameAppName { get; set; }
        
        public string? MandatoryAppFolderName { get; set; }
    }
    
    internal class EGSManifestFile : IEGSManifest
    {
        public int FormatVersion { get; set; }
        
        public string? AppVersionString { get; set; }
        public string? LaunchCommand { get; set; }
        public string? LaunchExecutable { get; set; }
        public string? ManifestLocation { get; set; }
#if NET5_0
        [JsonPropertyName("bIsApplication")]
#else 
        [JsonProperty("bIsApplication")]
#endif
        public bool IsApplication { get; set; }
#if NET5_0
        [JsonPropertyName("bIsExecutable")]
#else 
        [JsonProperty("bIsExecutable")]
#endif
        public bool IsExecutable { get; set; }
#if NET5_0
        [JsonPropertyName("bIsManaged")]
#else 
        [JsonProperty("bIsManaged")]
#endif
        public bool IsManaged { get; set; }
#if NET5_0
        [JsonPropertyName("bNeedsValidation")]
#else 
        [JsonProperty("bNeedsValidation")]
#endif
        public bool NeedsValidation { get; set; }
#if NET5_0
        [JsonPropertyName("bRequiresAuth")]
#else 
        [JsonProperty("bRequiresAuth")]
#endif
        public bool RequiresAuth { get; set; }
#if NET5_0
        [JsonPropertyName("bCanRunOffline")]
#else 
        [JsonProperty("bCanRunOffline")]
#endif
        public bool CanRunOffline { get; set; }
#if NET5_0
        [JsonPropertyName("bAllowMultipleInstances")]
#else 
        [JsonProperty("bAllowMultipleInstances")]
#endif
        public bool AllowMultipleInstances { get; set; }
        public string? AppName { get; set; }
        public string? CatalogItemId { get; set; }
        public string? CatalogNamespace { get; set; }
        public List<string>? AppCategories { get; set; }
        public string? DisplayName { get; set; }
        public string? FullAppName { get; set; }
        public string? InstallationGuid { get; set; }
        public string? InstallLocation { get; set; }
        public string? InstallSessionId { get; set; }
        public string? StagingLocation { get; set; }
        public string? TechnicalType { get; set; }
        public long InstallSize { get; set; }
        public string? MainGameAppName { get; set; }
        public string? MandatoryAppFolderName { get; set; }
    }
}
