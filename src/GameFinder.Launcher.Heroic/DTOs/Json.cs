using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameFinder.Launcher.Heroic.DTOs;

internal record Installed(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("executable")] string Executable,
    [property: JsonPropertyName("install_path")] string InstallPath,
    [property: JsonPropertyName("install_size")] string InstallSize,
    [property: JsonPropertyName("is_dlc")] bool IsDlc,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("appName")] string AppName,
    [property: JsonPropertyName("installedDLCs")] IReadOnlyList<object> InstalledDLCs,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("versionEtag")] string VersionEtag,
    [property: JsonPropertyName("buildId")] string BuildId,
    [property: JsonPropertyName("pinnedVersion")] bool PinnedVersion
);

internal record Root(
    [property: JsonPropertyName("installed")] IReadOnlyList<Installed> Installed
);



