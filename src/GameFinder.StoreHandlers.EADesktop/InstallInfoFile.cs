using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

[UsedImplicitly]
internal record InstallInfoFile(
    List<InstallInfo>? InstallInfos,
    Schema? Schema);

[UsedImplicitly]
internal record InstallInfo(
    string? BaseInstallPath,
    string? BaseSlug,
    string? DLCSubPath,
    string? InstallCheck,
    [property: JsonPropertyName("softwareId")]
    string? SoftwareId,
    string? ExecutableCheck);

[UsedImplicitly]
internal record Schema(int Version);
