using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameFinder.Common;
using GameFinder.StoreHandlers.EADesktop.Crypto;
using JetBrains.Annotations;
using NexusMods.Paths;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Handler for finding games installed with EA Desktop.
/// </summary>
[PublicAPI]
[RequiresUnreferencedCode("Calls System.Text.Json.JsonSerializer.Deserialize<TValue>(String, JsonSerializerOptions)")]
public class EADesktopHandler : AHandler<EADesktopGame, EADesktopGameId>
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
    /// Constructor.
    /// </summary>
    /// <param name="fileSystem"></param>
    /// <param name="hardwareInfoProvider"></param>
    public EADesktopHandler(IFileSystem fileSystem, IHardwareInfoProvider hardwareInfoProvider)
    {
        _fileSystem = fileSystem;
        _hardwareInfoProvider = hardwareInfoProvider;
    }

    /// <inheritdoc/>
    protected override IEqualityComparer<EADesktopGameId> IdEqualityComparer => EADesktopGameIdComparer.Default;

    /// <inheritdoc/>
    public override Func<EADesktopGame, EADesktopGameId> IdSelector => game => game.EADesktopGameId;

    /// <inheritdoc/>
    public override IEnumerable<Result<EADesktopGame>> FindAllGames()
    {
        var dataFolder = GetDataFolder(_fileSystem);
        if (!_fileSystem.DirectoryExists(dataFolder))
        {
            yield return Result.FromError<EADesktopGame>($"Data folder {dataFolder} does not exist!");
            yield break;
        }

        var installInfoFile = GetInstallInfoFile(dataFolder);
        if (!_fileSystem.FileExists(installInfoFile))
        {
            yield return Result.FromError<EADesktopGame>($"File does not exist: {installInfoFile.GetFullPath()}");
            yield break;
        }

        var decryptionResult = DecryptInstallInfoFile(_fileSystem, installInfoFile, _hardwareInfoProvider);
        var (plaintext, decryptionError) = decryptionResult;
        if (plaintext is null)
        {
            yield return Result.FromError<EADesktopGame>(decryptionError ?? $"Error decryption file {installInfoFile.GetFullPath()}");
            yield break;
        }

        foreach (var result in ParseInstallInfoFile(plaintext, installInfoFile, SchemaPolicy))
        {
            yield return result;
        }
    }

    internal static AbsolutePath GetDataFolder(IFileSystem fileSystem)
    {
        return fileSystem
            .GetKnownPath(KnownPath.CommonApplicationDataDirectory)
            .CombineUnchecked("EA Desktop");
    }

    internal static AbsolutePath GetInstallInfoFile(AbsolutePath dataFolder)
    {
        return dataFolder
            .CombineUnchecked(AllUsersFolderName)
            .CombineUnchecked(InstallInfoFileName);
    }

    internal static Result<string> DecryptInstallInfoFile(IFileSystem fileSystem, AbsolutePath installInfoFile, IHardwareInfoProvider hardwareInfoProvider)
    {
        try
        {
            using var stream = fileSystem.ReadFile(installInfoFile);
            var cipherText = new byte[stream.Length];
            var count = stream.Read(cipherText);

            var key = Decryption.CreateDecryptionKey(hardwareInfoProvider);

            var iv = Decryption.CreateDecryptionIV();
            var plainText = Decryption.DecryptFile(cipherText, key, iv);
            return Result.FromGame(plainText);
        }
        catch (Exception e)
        {
            return Result.FromException<string>($"Exception while decrypting file {installInfoFile.GetFullPath()}", e);
        }
    }

    internal IEnumerable<Result<EADesktopGame>> ParseInstallInfoFile(string plaintext, AbsolutePath installInfoFile, SchemaPolicy schemaPolicy)
    {
        try
        {
            return ParseInstallInfoFileInner(plaintext, installInfoFile, schemaPolicy);
        }
        catch (Exception e)
        {
            return new[]
            {
                Result.FromException<EADesktopGame>($"Exception while parsing InstallInfoFile {installInfoFile.GetFullPath()}", e),
            };
        }
    }

    private IEnumerable<Result<EADesktopGame>> ParseInstallInfoFileInner(string plaintext, AbsolutePath installInfoFile, SchemaPolicy schemaPolicy)
    {
        var installInfoFileContents = JsonSerializer.Deserialize<InstallInfoFile>(plaintext, JsonSerializerOptions);

        if (installInfoFileContents is null)
        {
            yield return Result.FromError<EADesktopGame>($"Unable to deserialize InstallInfoFile {installInfoFile.GetFullPath()}");
            yield break;
        }

        var schemaVersionNullable = installInfoFileContents.Schema?.Version;
        if (!schemaVersionNullable.HasValue)
        {
            yield return Result.FromError<EADesktopGame>($"InstallInfoFile {installInfoFile.GetFullPath()} does not have a schema version!");
            yield break;
        }

        var schemaVersion = schemaVersionNullable.Value;
        var (schemaMessage, isSchemaError) = CreateSchemaVersionMessage(schemaPolicy, schemaVersion, installInfoFile);
        if (schemaMessage is not null)
        {
            yield return Result.FromError<EADesktopGame>(schemaMessage);
            if (isSchemaError) yield break;
        }

        var installInfos = installInfoFileContents.InstallInfos;
        if (installInfos is null || installInfos.Count == 0)
        {
            yield return Result.FromError<EADesktopGame>($"InstallInfoFile {installInfoFile.GetFullPath()} does not have any infos!");
            yield break;
        }

        for (var i = 0; i < installInfos.Count; i++)
        {
            yield return InstallInfoToGame(_fileSystem, installInfos[i], i, installInfoFile);
        }
    }

    internal static (string? message, bool isError) CreateSchemaVersionMessage(
        SchemaPolicy schemaPolicy, int schemaVersion, AbsolutePath installInfoFilePath)
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

    internal static Result<EADesktopGame> InstallInfoToGame(IFileSystem fileSystem, InstallInfo installInfo, int i, AbsolutePath installInfoFilePath)
    {
        var num = i.ToString(CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(installInfo.SoftwareId))
        {
            return Result.FromError<EADesktopGame>($"InstallInfo #{num} does not have the value \"softwareId\"");
        }

        var softwareId = installInfo.SoftwareId;

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
        var game = new EADesktopGame(
            EADesktopGameId.From(softwareId),
            baseSlug,
            fileSystem.FromFullPath(SanitizeInputPath(baseInstallPath)));

        return Result.FromGame(game);
    }
}
