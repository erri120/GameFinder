using System.Collections.Generic;
using System.Text.Json.Serialization;
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
        [JsonPropertyName("bIsApplication")]
        public bool IsApplication { get; set; }
        [JsonPropertyName("bIsExecutable")]
        public bool IsExecutable { get; set; }
        [JsonPropertyName("bIsManaged")]
        public bool IsManaged { get; set; }
        [JsonPropertyName("bNeedsValidation")]
        public bool NeedsValidation { get; set; }
        [JsonPropertyName("bRequiresAuth")]
        public bool RequiresAuth { get; set; }
        [JsonPropertyName("bCanRunOffline")]
        public bool CanRunOffline { get; set; }
        [JsonPropertyName("bAllowMultipleInstances")]
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
