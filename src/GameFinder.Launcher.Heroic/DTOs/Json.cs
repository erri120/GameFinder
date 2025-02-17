using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GameFinder.Launcher.Heroic.DTOs;

internal record Installed(
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("executable")] string Executable,
    [property: JsonPropertyName("install_path")] string InstallPath,
    [property: JsonPropertyName("install_size")] string InstallSize,
    [property: JsonPropertyName("is_dlc")] bool IsDlc,
    [property: JsonPropertyName("version")] string? Version,
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

internal record LinuxGameConfig(
    [property: JsonPropertyName("autoInstallDxvk")] bool AutoInstallDxvk,
    [property: JsonPropertyName("autoInstallDxvkNvapi")] bool AutoInstallDxvkNvapi,
    [property: JsonPropertyName("autoInstallVkd3d")] bool AutoInstallVkd3d,
    [property: JsonPropertyName("preferSystemLibs")] bool PreferSystemLibs,
    [property: JsonPropertyName("enableEsync")] bool EnableEsync,
    [property: JsonPropertyName("enableMsync")] bool EnableMsync,
    [property: JsonPropertyName("enableFsync")] bool EnableFsync,
    [property: JsonPropertyName("nvidiaPrime")] bool NvidiaPrime,
    [property: JsonPropertyName("enviromentOptions")] IReadOnlyList<object> EnviromentOptions,
    [property: JsonPropertyName("wrapperOptions")] IReadOnlyList<object> WrapperOptions,
    [property: JsonPropertyName("showFps")] bool ShowFps,
    [property: JsonPropertyName("useGameMode")] bool UseGameMode,
    [property: JsonPropertyName("battlEyeRuntime")] bool BattlEyeRuntime,
    [property: JsonPropertyName("eacRuntime")] bool EacRuntime,
    [property: JsonPropertyName("language")] string Language,
    [property: JsonPropertyName("beforeLaunchScriptPath")] string BeforeLaunchScriptPath,
    [property: JsonPropertyName("afterLaunchScriptPath")] string AfterLaunchScriptPath,
    [property: JsonPropertyName("wineVersion")] WineVersion WineVersion,
    [property: JsonPropertyName("winePrefix")] string WinePrefix,
    [property: JsonPropertyName("wineCrossoverBottle")] string WineCrossoverBottle
);

public record WineVersion(
    [property: JsonPropertyName("bin")] string Bin,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type
);



