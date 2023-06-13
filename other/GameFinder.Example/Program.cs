using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using CommandLine;
using GameFinder.Common;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.EADesktop;
using GameFinder.StoreHandlers.EADesktop.Crypto;
using GameFinder.StoreHandlers.EADesktop.Crypto.Windows;
using GameFinder.StoreHandlers.EGS;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Origin;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Xbox;
using GameFinder.Wine;
using GameFinder.Wine.Bottles;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using OneOf;
using FileSystem = NexusMods.Paths.FileSystem;
using IFileSystem = NexusMods.Paths.IFileSystem;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GameFinder.Example;

public static class Program
{
#if DEBUG
    public static readonly MessageLevel DefaultLogLevel = 0;
#else
    public static readonly MessageLevel DefaultLogLevel = 2;
#endif
    private static MessageLevel _logLevel = DefaultLogLevel;
    private static NLogLoggerProvider _provider = null!;

    public static void Main(string[] args)
    {
        var config = new LoggingConfiguration();

        var coloredConsoleTarget = new ColoredConsoleTarget("coloredConsole")
        {
            DetectConsoleAvailable = true,
            EnableAnsiOutput = OperatingSystem.IsLinux(), // windows hates this
            UseDefaultRowHighlightingRules = false,
            WordHighlightingRules =
            {
                new ConsoleWordHighlightingRule
                {
                    Regex = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}\.\d+",
                    CompileRegex = true,
                    ForegroundColor = ConsoleOutputColor.Gray,
                },
                new ConsoleWordHighlightingRule("TRACE", ConsoleOutputColor.DarkGray, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("DEBUG", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("INFO", ConsoleOutputColor.Cyan, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("WARN", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("ERROR", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("FATAL", ConsoleOutputColor.DarkRed, ConsoleOutputColor.NoChange),
            },
            Layout = "${longdate}|${level:uppercase=true}|${logger}|${message:withexception=true}",
        };

        var fileTarget = new FileTarget("file")
        {
            FileName = "log.log",
        };

        config.AddRuleForAllLevels(coloredConsoleTarget);
        config.AddRuleForAllLevels(fileTarget);

        LogManager.Configuration = config;
        _provider = new NLogLoggerProvider();

        var logger = _provider.CreateLogger(nameof(Program));

        Parser.Default
            .ParseArguments<Options>(args)
            .WithParsed(x => Run(x, logger));
    }

    [SuppressMessage("Design", "MA0051:Method is too long")]
    private static void Run(Options options, ILogger logger)
    {
        var realFileSystem = FileSystem.Shared;

        var logFile = realFileSystem.GetKnownPath(KnownPath.CurrentDirectory).CombineUnchecked("log.log");
        if (realFileSystem.FileExists(logFile)) realFileSystem.DeleteFile(logFile);

        _logLevel = (options.LogLevel >= (int)MessageLevel.Trace &&
            options.LogLevel <= (int)MessageLevel.None) ? (MessageLevel)options.LogLevel : DefaultLogLevel;

        Log(logger, MessageLevel.Info, "Operating System: {OSDescription}", RuntimeInformation.OSDescription);

        if (OperatingSystem.IsWindows())
        {
            var windowsRegistry = WindowsRegistry.Shared;
            if (options.Steam) RunSteamHandler(realFileSystem, windowsRegistry);
            if (options.GOG) RunGOGHandler(windowsRegistry, realFileSystem);
            if (options.EGS) RunEGSHandler(windowsRegistry, realFileSystem);
            if (options.Origin) RunOriginHandler(realFileSystem);
            if (options.Xbox) RunXboxHandler(realFileSystem);
            if (options.EADesktop)
            {
                var hardwareInfoProvider = new HardwareInfoProvider();
                var decryptionKey = Decryption.CreateDecryptionKey(new HardwareInfoProvider());
                var sDecryptionKey = Convert.ToHexString(decryptionKey).ToLower(CultureInfo.InvariantCulture);
                Log(logger, MessageLevel.Debug, "EA Decryption Key: {DecryptionKey}", sDecryptionKey);

                RunEADesktopHandler(realFileSystem, hardwareInfoProvider);
            }
        }

        if (OperatingSystem.IsLinux())
        {
            if (options.Steam) RunSteamHandler(realFileSystem, registry: null);
            var winePrefixes = new List<AWinePrefix>();

            if (options.Wine)
            {
                var prefixManager = new DefaultWinePrefixManager(realFileSystem);
                winePrefixes.AddRange(LogWinePrefixes(prefixManager, _provider.CreateLogger("Wine")));
            }

            if (options.Bottles)
            {
                var prefixManager = new BottlesWinePrefixManager(realFileSystem);
                winePrefixes.AddRange(LogWinePrefixes(prefixManager, _provider.CreateLogger("Bottles")));
            }

            foreach (var winePrefix in winePrefixes)
            {
                var wineFileSystem = winePrefix.CreateOverlayFileSystem(realFileSystem);
                var wineRegistry = winePrefix.CreateRegistry(realFileSystem);

                if (options.GOG) RunGOGHandler(wineRegistry, wineFileSystem);
                if (options.EGS) RunEGSHandler(wineRegistry, wineFileSystem);
                if (options.Origin) RunOriginHandler(wineFileSystem);
                if (options.Xbox) RunXboxHandler(wineFileSystem);
            }
        }
    }

    private static void RunGOGHandler(IRegistry registry, IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(GOGHandler));
        var handler = new GOGHandler(registry, fileSystem);
        LogGamesAndMessages(handler.FindAllGames(), logger);
    }

    private static void RunEGSHandler(IRegistry registry, IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(EGSHandler));
        var handler = new EGSHandler(registry, fileSystem);
        LogGamesAndMessages(handler.FindAllGames(), logger);
    }

    private static void RunOriginHandler(IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(OriginHandler));
        var handler = new OriginHandler(fileSystem);
        LogGamesAndMessages(handler.FindAllGames(), logger);
    }

    private static void RunEADesktopHandler(
        IFileSystem fileSystem,
        IHardwareInfoProvider hardwareInfoProvider)
    {
        var logger = _provider.CreateLogger(nameof(EADesktopHandler));
        var handler = new EADesktopHandler(fileSystem, hardwareInfoProvider);
        LogGamesAndMessages(handler.FindAllGames(), logger);
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:RequiresUnreferencedCodeAttribute",
        Justification = "Required types are preserved using TrimmerRootDescriptor file.")]
    private static void RunXboxHandler(IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(XboxHandler));
        var handler = new XboxHandler(fileSystem);
        LogGamesAndMessages(handler.FindAllGames(), logger);
    }

    private static void RunSteamHandler(IFileSystem fileSystem, IRegistry? registry)
    {
        var logger = _provider.CreateLogger(nameof(SteamHandler));
        var handler = new SteamHandler(fileSystem, registry);
        LogGamesAndMessages(handler.FindAllGames(), logger, game =>
        {
            if (!OperatingSystem.IsLinux()) return;
            var protonPrefix = game.GetProtonPrefix();
            if (!fileSystem.DirectoryExists(protonPrefix.ConfigurationDirectory)) return;
            Log(logger, MessageLevel.Info, "Proton Directory for this game: {}", protonPrefix.ProtonDirectory.GetFullPath());
        });
    }

    private static List<AWinePrefix> LogWinePrefixes<TWinePrefix>(IWinePrefixManager<TWinePrefix> prefixManager, ILogger logger)
    where TWinePrefix : AWinePrefix
    {
        var res = new List<AWinePrefix>();

        foreach (var result in prefixManager.FindPrefixes())
        {
            result.Switch(prefix =>
            {
                logger.LogInformation("Found wine prefix at {PrefixConfigurationDirectory}", prefix.ConfigurationDirectory);
                res.Add(prefix);
            }, message =>
            {
                Log(logger, message.Level, "{Message}", message);
            });
        }

        return res;
    }

    private static void LogGamesAndMessages<TGame>(IEnumerable<OneOf<TGame, LogMessage>> results, ILogger logger, Action<TGame>? action = null)
        where TGame : class
    {
        foreach (var result in results)
        {
            result.Switch(game =>
            {
                Log(logger, MessageLevel.Info, "Found {Game}", game);
                action?.Invoke(game);
            }, message =>
            {
                Log(logger, message.Level, "{Message}", message);
            });
        }
    }

    private static void Log(ILogger logger, MessageLevel msgLevel, string? message, params object?[] args)
    {
        if (_logLevel <= msgLevel)
            logger.Log((Microsoft.Extensions.Logging.LogLevel)msgLevel, message, args);
    }
}
