using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameFinder.Common;
using GameFinder.StoreHandlers.EADesktop.Crypto;
using GameFinder.StoreHandlers.EADesktop.Crypto.Windows;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Handler for finding games installed with EA Desktop.
/// </summary>
[PublicAPI]
public class EADesktopHandler : AHandler<EADesktopGame, string>
{
    internal const string AllUsersFolderName = "530c11479fe252fc5aabc24935b9776d4900eb3ba58fdc271e0d6229413ad40e";
    internal const string InstallInfoFileName = "IS";

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.Strict,
    };

    private readonly IFileSystem _fileSystem;
    private readonly IHardwareInfoProvider _hardwareInfoProvider;

    /// <summary>
    /// The supported schema version of this handler. You can change the schema policy with
    /// <see cref="SchemaPolicy"/>.
    /// </summary>
    public const int SupportedSchemaVersion = 21;

    /// <summary>
    /// Policy to use when the schema version does not match <see cref="SupportedSchemaVersion"/>.
    /// The default behavior is <see cref="EADesktop.SchemaPolicy.Warn"/>.
    /// </summary>
    public SchemaPolicy SchemaPolicy { get; set; } = SchemaPolicy.Warn;

    /// <summary>
    /// Default constructor that uses the real filesystem <see cref="FileSystem"/> and
    /// real hardware information <see cref="HardwareInfoProvider"/>.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public EADesktopHandler() : this(new FileSystem(), new HardwareInfoProvider()) { }

    /// <summary>
    /// Constructor for specifying the <see cref="IFileSystem"/> and <see cref="IHardwareInfoProvider"/>.
    /// Use this constructor if you want to run tests.
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <param name="hardwareInfoProvider"></param>
    public EADesktopHandler(IFileSystem fileSystem, IHardwareInfoProvider hardwareInfoProvider)
    {
        _fileSystem = fileSystem;
        _hardwareInfoProvider = hardwareInfoProvider;
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

        var installInfoFile = GetInstallInfoFile(dataFolder);
        if (!installInfoFile.Exists)
        {
            yield return Result.FromError<EADesktopGame>($"File does not exist: {installInfoFile.FullName}");
            yield break;
        }

        var decryptionResult = DecryptInstallInfoFile(installInfoFile, _hardwareInfoProvider);
        var (plaintext, decryptionError) = decryptionResult;
        if (plaintext is null)
        {
            yield return Result.FromError<EADesktopGame>(decryptionError ?? $"Error decryption file {installInfoFile.FullName}");
            yield break;
        }

        foreach (var result in ParseInstallInfoFile(plaintext, installInfoFile, SchemaPolicy))
        {
            yield return result;
        }
    }

    /// <inheritdoc/>
    public override IDictionary<string, EADesktopGame> FindAllGamesById(out string[] errors)
    {
        var (games, allErrors) = FindAllGames().SplitResults();
        errors = allErrors;

        return games.CustomToDictionary(game => game.SoftwareID, game => game, StringComparer.OrdinalIgnoreCase);
    }

    internal static IDirectoryInfo GetDataFolder(IFileSystem fileSystem)
    {
        return fileSystem.DirectoryInfo.New(fileSystem.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "EA Desktop"
        ));
    }

    internal static IFileInfo GetInstallInfoFile(IDirectoryInfo dataFolder)
    {
        var fileSystem = dataFolder.FileSystem;

        return fileSystem.FileInfo.New(fileSystem.Path.Combine(
            dataFolder.FullName,
            AllUsersFolderName,
            InstallInfoFileName
        ));
    }

    internal static Result<string> DecryptInstallInfoFile(IFileInfo installInfoFile, IHardwareInfoProvider hardwareInfoProvider)
    {
        try
        {
            var cipherText = installInfoFile.FileSystem.File.ReadAllBytes(installInfoFile.FullName);
            var key = Decryption.CreateDecryptionKey(hardwareInfoProvider);

            var iv = Decryption.CreateDecryptionIV();
            var plainText = Decryption.DecryptFile(cipherText, key, iv);
            return Result.FromGame(plainText);
        }
        catch (Exception e)
        {
            return Result.FromException<string>($"Exception while decrypting file {installInfoFile.FullName}", e);
        }
    }

    internal static IEnumerable<Result<EADesktopGame>> ParseInstallInfoFile(string plaintext, IFileInfo installInfoFile, SchemaPolicy schemaPolicy)
    {
        try
        {
            return ParseInstallInfoFileInner(plaintext, installInfoFile, schemaPolicy);
        }
        catch (Exception e)
        {
            return new[]
            {
                Result.FromException<EADesktopGame>($"Exception while parsing InstallInfoFile {installInfoFile.FullName}", e),
            };
        }
    }

    private static IEnumerable<Result<EADesktopGame>> ParseInstallInfoFileInner(string plaintext, IFileInfo installInfoFile, SchemaPolicy schemaPolicy)
    {
        var installInfoFileContents = JsonSerializer.Deserialize<InstallInfoFile>(plaintext, JsonSerializerOptions);

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

        var baseInstallPath = installInfo.BaseInstallPath;
        if (baseInstallPath.EndsWith('\\'))
            baseInstallPath = baseInstallPath[..^1];

        var game = new EADesktopGame(
            softwareId,
            baseSlug,
            baseInstallPath);

        return Result.FromGame(game);
    }
}
