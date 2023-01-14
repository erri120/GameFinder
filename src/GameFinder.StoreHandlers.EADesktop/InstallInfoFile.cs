using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

[UsedImplicitly]
internal class InstallInfoFile
{
    public List<InstallInfo>? InstallInfos { get; set; }
    public Schema? Schema { get; set; }
}

[UsedImplicitly]
internal class InstallInfo
{
    public string? BaseInstallPath { get; set; }
    public string? BaseSlug { get; set; }
    public string? InstallCheck { get; set; }
    [JsonPropertyName("softwareId")]
    public string? SoftwareID { get; set; }
}

[UsedImplicitly]
internal class Schema
{
    public int Version { get; set; }
}
