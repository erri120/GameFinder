using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EADesktop.Crypto;
using JetBrains.Annotations;
using NexusMods.Paths;
using OneOf;

namespace GameFinder.StoreHandlers.EADesktop;

/// <summary>
/// Handler for finding games installed with the EA app.
/// </summary>
[PublicAPI]
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
        TypeInfoResolver = SourceGenerationContext.Default,
    };

    private readonly IRegistry _registry;
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
    /// <param name="registry">
    /// The implementation of <see cref="IRegistry"/> to use. For a shared instance
    /// use <see cref="WindowsRegistry.Shared"/> on Windows. For tests either use
    /// <see cref="InMemoryRegistry"/>, a custom implementation or just a mock
    /// of the interface.
    /// </param>
    /// <param name="fileSystem">
    /// The implementation of <see cref="IFileSystem"/> to use. For a shared instance use
    /// <see cref="FileSystem.Shared"/>. For tests either use <see cref="InMemoryFileSystem"/>,
    /// a custom implementation or just a mock of the interface.
    /// </param>
    /// <param name="hardwareInfoProvider">
    /// The implementation of <see cref="IHardwareInfoProvider"/> to use. Currently only
    /// <see cref="GameFinder.StoreHandlers.EADesktop.Crypto.Windows.HardwareInfoProvider"/>
    /// is available and is Windows-only.
    /// </param>
    public EADesktopHandler(IRegistry registry, IFileSystem fileSystem, IHardwareInfoProvider hardwareInfoProvider)
    {
        _registry = registry;
        _fileSystem = fileSystem;
        _hardwareInfoProvider = hardwareInfoProvider;
    }

    /// <inheritdoc/>
    public override IEqualityComparer<EADesktopGameId> IdEqualityComparer => EADesktopGameIdComparer.Default;

    /// <inheritdoc/>
    public override Func<EADesktopGame, EADesktopGameId> IdSelector => game => game.EADesktopGameId;

    /// <inheritdoc/>
    public override IEnumerable<OneOf<EADesktopGame, ErrorMessage>> FindAllGames()
    {
        var dataFolder = GetDataFolder(_fileSystem);
        if (!_fileSystem.DirectoryExists(dataFolder))
        {
            yield return new ErrorMessage($"Data folder {dataFolder} does not exist!");
            yield break;
        }

        var installInfoFile = GetInstallInfoFile(dataFolder);
        if (!_fileSystem.FileExists(installInfoFile))
        {
            yield return new ErrorMessage($"File does not exist: {installInfoFile.GetFullPath()}");
            yield break;
        }

        var decryptionResult = DecryptInstallInfoFile(_fileSystem, installInfoFile, _hardwareInfoProvider);
        if (decryptionResult.TryGetError(out var error))
        {
            yield return error;
            yield break;
        }

        var plaintext = decryptionResult.AsT0;
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

    internal static OneOf<string, ErrorMessage> DecryptInstallInfoFile(IFileSystem fileSystem, AbsolutePath installInfoFile, IHardwareInfoProvider hardwareInfoProvider)
    {
        try
        {
            using var stream = fileSystem.ReadFile(installInfoFile);
            var cipherText = new byte[stream.Length];
            var unused = stream.Read(cipherText);

            var key = Decryption.CreateDecryptionKey(hardwareInfoProvider);

            var iv = Decryption.CreateDecryptionIV();
            var plainText = Decryption.DecryptFile(cipherText, key, iv);
            return plainText;
        }
        catch (Exception e)
        {
            return new ErrorMessage(e, $"Exception while decrypting file {installInfoFile.GetFullPath()}");
        }
    }

    internal IEnumerable<OneOf<EADesktopGame, ErrorMessage>> ParseInstallInfoFile(string plaintext, AbsolutePath installInfoFile, SchemaPolicy schemaPolicy)
    {
        try
        {
            return ParseInstallInfoFileInner(plaintext, installInfoFile, schemaPolicy);
        }
        catch (Exception e)
        {
            return new OneOf<EADesktopGame, ErrorMessage>[]
            {
                new ErrorMessage(e, $"Exception while parsing InstallInfoFile {installInfoFile.GetFullPath()}"),
            };
        }
    }


    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with \'RequiresUnreferencedCodeAttribute\' require dynamic access otherwise can break functionality when trimming application code",
        Justification = $"{nameof(JsonSerializerOptions)} uses {nameof(SourceGenerationContext)} for type information.")]
    private IEnumerable<OneOf<EADesktopGame, ErrorMessage>> ParseInstallInfoFileInner(string plaintext, AbsolutePath installInfoFile, SchemaPolicy schemaPolicy)
    {
        var installInfoFileContents = JsonSerializer.Deserialize<InstallInfoFile>(plaintext, JsonSerializerOptions);

        if (installInfoFileContents is null)
        {
            yield return new ErrorMessage($"Unable to deserialize InstallInfoFile {installInfoFile.GetFullPath()}");
            yield break;
        }

        var schemaVersionNullable = installInfoFileContents.Schema?.Version;
        if (!schemaVersionNullable.HasValue)
        {
            yield return new ErrorMessage($"InstallInfoFile {installInfoFile.GetFullPath()} does not have a schema version!");
            yield break;
        }

        var schemaVersion = schemaVersionNullable.Value;
        var (schemaMessage, isSchemaError) = CreateSchemaVersionMessage(schemaPolicy, schemaVersion, installInfoFile);
        if (schemaMessage is not null)
        {
            yield return new ErrorMessage(schemaMessage);
            if (isSchemaError) yield break;
        }

        var installInfos = installInfoFileContents.InstallInfos;
        if (installInfos is null || installInfos.Count == 0)
        {
            yield return new ErrorMessage($"InstallInfoFile {installInfoFile.GetFullPath()} does not have any infos!");
            yield break;
        }

        for (var i = 0; i < installInfos.Count; i++)
        {
            yield return InstallInfoToGame(_registry, _fileSystem, installInfos[i], i, installInfoFile);
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

    internal static OneOf<EADesktopGame, ErrorMessage> InstallInfoToGame(IRegistry registry, IFileSystem fileSystem, InstallInfo installInfo, int i, AbsolutePath installInfoFilePath)
    {
        var num = i.ToString(CultureInfo.InvariantCulture);

        if (string.IsNullOrEmpty(installInfo.SoftwareId))
        {
            return new ErrorMessage($"InstallInfo #{num} does not have the value \"softwareId\"");
        }

        var softwareId = installInfo.SoftwareId;

        var baseSlug = installInfo.BaseSlug ?? "";

        if (!string.IsNullOrEmpty(installInfo.DLCSubPath))
        {
            return new ErrorMessage($"InstallInfo #{num} for {softwareId} ({baseSlug}) is a DLC");
        }

        if (string.IsNullOrEmpty(installInfo.BaseInstallPath))
        {
            return new ErrorMessage($"InstallInfo #{num} for {softwareId} ({baseSlug}) does not have the value \"baseInstallPath\"");
        }

        var executableCheck = installInfo.ExecutableCheck;
        var sRegKey = "";
        if (executableCheck.StartsWith('['))
        {
            int j = executableCheck.IndexOf(']');
            if (j > 1)
                sRegKey = executableCheck.Substring(1, j - 1);
        }
        var name = "";
        if (!string.IsNullOrEmpty(sRegKey))
        {
            try
            {
                var k = sRegKey.IndexOf('\\', StringComparison.Ordinal);
                var l = sRegKey.LastIndexOf('\\');
                if (k > 1 && l > k)
                {
                    var regRoot = registry.OpenBaseKey(RegistryHelpers.RegistryHiveFromString(sRegKey[..k].ToUpperInvariant()), RegistryView.Registry32);
                    var sSubKey = sRegKey[(k + 1)..l];
                    var subKey = regRoot.OpenSubKey(sSubKey);
                    subKey?.TryGetString("DisplayName", out name);
                }
            }
            catch (Exception) { }
        }
        if (string.IsNullOrEmpty(name))
        {
            name = baseSlug;
        }

        var baseInstallPath = installInfo.BaseInstallPath;
        var game = new EADesktopGame(
            EADesktopGameId.From(softwareId),
            name,
            fileSystem.FromFullPath(SanitizeInputPath(baseInstallPath)));

        return game;
    }
}
