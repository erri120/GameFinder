using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameFinder.Common;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

[PublicAPI]
public class EADesktopHandler : AHandler<EADesktopGame, string>
{
    public const int SupportedSchemaVersion = 21;
    internal const string InstallInfoFileName = "IS.json";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.Strict,
    };

    private readonly IFileSystem _fileSystem;

    public SchemaPolicy SchemaPolicy { get; set; } = SchemaPolicy.Warn;

    [SupportedOSPlatform("windows")]
    public EADesktopHandler() : this(new FileSystem()) { }

    public EADesktopHandler(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    /// <inheritdoc/>
    public override IEnumerable<Result<EADesktopGame>> FindAllGames()
    {
        var dataFolder = GetDataFolder(_fileSystem);
        if (!dataFolder.Exists)
        {
            yield return Result.FromError<EADesktopGame>($"Data folder {dataFolder} does not exist!");
            yield break;
        }

        var installInfoFile = FindInstallInfoFile(dataFolder);
        if (installInfoFile is null)
        {
            yield return Result.FromError<EADesktopGame>($"Unable to find IS.json inside data folder {dataFolder}");
            yield break;
        }

        foreach (var result in ParseInstallInfoFile(installInfoFile, SchemaPolicy))
        {
            yield return result;
        }
    }

    /// <inheritdoc/>
    public override IDictionary<string, EADesktopGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.SoftwareID, game => game,StringComparer.OrdinalIgnoreCase);
    }

    internal static IDirectoryInfo GetDataFolder(IFileSystem fileSystem)
    {
        return fileSystem.DirectoryInfo.New(fileSystem.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "EA Desktop"
        ));
    }

    internal static IFileInfo GetInstallInfoFile(IDirectoryInfo parentFolder)
    {
        var fileSystem = parentFolder.FileSystem;

        return fileSystem.FileInfo.New(fileSystem.Path.Combine(
            parentFolder.FullName,
            InstallInfoFileName
        ));
    }

    internal static IFileInfo? FindInstallInfoFile(IDirectoryInfo dataFolder)
    {
        return dataFolder
            .EnumerateFiles(InstallInfoFileName, SearchOption.AllDirectories)
            .FirstOrDefault();
    }

    internal static IEnumerable<Result<EADesktopGame>> ParseInstallInfoFile(IFileInfo installInfoFile, SchemaPolicy schemaPolicy)
    {
        try
        {
            return ParseInstallInfoFileInner(installInfoFile, schemaPolicy);
        }
        catch (Exception e)
        {
            return new[]
            {
                Result.FromException<EADesktopGame>($"Exception while parsing InstallInfoFile {installInfoFile.FullName}", e),
            };
        }
    }

    private static IEnumerable<Result<EADesktopGame>> ParseInstallInfoFileInner(IFileInfo installInfoFile, SchemaPolicy schemaPolicy)
    {
        using var stream = installInfoFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        var installInfoFileContents = JsonSerializer.Deserialize<InstallInfoFile>(stream, JsonSerializerOptions);

        if (installInfoFileContents is null)
        {
            yield return Result.FromError<EADesktopGame>($"Unable to deserialize InstallInfoFile {installInfoFile.FullName}");
            yield break;
        }

        var schemaVersionNullable = installInfoFileContents.Schema?.Version;
        if (!schemaVersionNullable.HasValue)
        {
            yield return Result.FromError<EADesktopGame>($"InstallInfoFile {installInfoFile.FullName} does not have a schema version!");
            yield break;
        }

        var schemaVersion = schemaVersionNullable.Value;
        var (schemaMessage, isSchemaError) = CreateSchemaVersionMessage(schemaPolicy, schemaVersion, installInfoFile.FullName);
        if (schemaMessage is not null)
        {
            yield return Result.FromError<EADesktopGame>(schemaMessage);
            if (isSchemaError) yield break;
        }

        var installInfos = installInfoFileContents.InstallInfos;
        if (installInfos is null || installInfos.Count == 0)
        {
            yield return Result.FromError<EADesktopGame>($"InstallInfoFile {installInfoFile.FullName} does not have any infos!");
            yield break;
        }

        for (var i = 0; i < installInfos.Count; i++)
        {
            yield return InstallInfoToGame(installInfos[i], i, installInfoFile.FullName);
        }
    }

    internal static (string? message, bool isError) CreateSchemaVersionMessage(
        SchemaPolicy schemaPolicy, int schemaVersion, string installInfoFilePath)
    {
        if (schemaVersion == SupportedSchemaVersion) return (null, false);

        return schemaPolicy switch
        {
            SchemaPolicy.Warn => (
                $"InstallInfoFile {installInfoFilePath} has a schema version " +
                $"{schemaVersion.ToString(CultureInfo.InvariantCulture)} but this library only supports schema version " +
                $"{SupportedSchemaVersion.ToString(CultureInfo.InvariantCulture)}. " +
                $"This message is a WARNING because the consumer of this library has set {nameof(SchemaPolicy)} to {nameof(SchemaPolicy.Warn)}",
                false),
            SchemaPolicy.Error => (
                $"InstallInfoFile {installInfoFilePath} has a schema version " +
                $"{schemaVersion.ToString(CultureInfo.InvariantCulture)} but this library only supports schema version " +
                $"{SupportedSchemaVersion.ToString(CultureInfo.InvariantCulture)}. " +
                $"This is an ERROR because the consumer of this library has set {nameof(SchemaPolicy)} to {nameof(SchemaPolicy.Error)}",
                true),
            SchemaPolicy.Ignore => (null, false),
            _ => throw new ArgumentOutOfRangeException(nameof(schemaPolicy), schemaPolicy, message: null),
        };
    }

    internal static Result<EADesktopGame> InstallInfoToGame(InstallInfo installInfo, int i, string installInfoFilePath)
    {
        var num = i.ToString(CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(installInfo.SoftwareID))
        {
            return Result.FromError<EADesktopGame>($"InstallInfo #{num} does not have the value \"softwareId\"");
        }

        var softwareId = installInfo.SoftwareID;

        if (string.IsNullOrEmpty(installInfo.BaseSlug))
        {
            return Result.FromError<EADesktopGame>($"InstallInfo #{num} for {softwareId} does not have the value \"baseSlug\"");
        }

        var baseSlug = installInfo.BaseSlug;

        if (string.IsNullOrEmpty(installInfo.BaseInstallPath))
        {
            return Result.FromError<EADesktopGame>($"InstallInfo #{num} for {softwareId} ({baseSlug}) does not have the value \"baseInstallPath\"");
        }

        if (string.IsNullOrEmpty(installInfo.InstallCheck))
        {
            return Result.FromError<EADesktopGame>($"InstallInfo #{num} for {softwareId} ({baseSlug}) does not have the value \"installCheck\"");
        }

        var baseInstallPath = installInfo.BaseInstallPath;
        if (baseInstallPath.EndsWith('\\'))
            baseInstallPath = baseInstallPath[..^1];

        var game = new EADesktopGame(
            softwareId,
            baseSlug,
            baseInstallPath,
            installInfo.InstallCheck,
            installInfoFilePath);

        return Result.FromGame(game);
    }
}
