using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using CommandLine;
using GameFinder.Common;
using GameFinder.Launcher.Heroic;
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

[assembly: ExcludeFromCodeCoverage]
namespace GameFinder.Example;

public static class Program
{
    private static NLogLoggerProvider _provider = null!;
    private static NLogLoggerFactory _loggerFactory = null!;

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
                new ConsoleWordHighlightingRule("DEBUG", ConsoleOutputColor.Gray, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("INFO", ConsoleOutputColor.Cyan, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("ERROR", ConsoleOutputColor.Red, ConsoleOutputColor.NoChange),
                new ConsoleWordHighlightingRule("WARNING", ConsoleOutputColor.Yellow, ConsoleOutputColor.NoChange),
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
        _loggerFactory = new NLogLoggerFactory();

        var logger = _loggerFactory.CreateLogger(nameof(Program));

        Parser.Default
            .ParseArguments<Options>(args)
            .WithParsed(x => Run(x, logger));
    }

    private static void Run(Options options, ILogger logger)
    {
        var realFileSystem = FileSystem.Shared;

        var logFile = realFileSystem.GetKnownPath(KnownPath.CurrentDirectory).Combine("log.log");
        if (realFileSystem.FileExists(logFile)) realFileSystem.DeleteFile(logFile);

        logger.LogInformation("Operating System: {OSDescription}", RuntimeInformation.OSDescription);

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
                logger.LogDebug("EA Decryption Key: {DecryptionKey}", sDecryptionKey);

                RunEADesktopHandler(realFileSystem, hardwareInfoProvider);
            }
        }

        if (OperatingSystem.IsLinux())
        {
            if (options.Steam) RunSteamHandler(realFileSystem, registry: null);
            if (options.Heroic) RunHeroicGOGHandler(realFileSystem);

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

        if (OperatingSystem.IsMacOS())
        {
            if (options.Steam)
                RunSteamHandler(realFileSystem, null);
        }
    }

    private static void RunGOGHandler(IRegistry registry, IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(GOGHandler));
        var handler = new GOGHandler(registry, fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunHeroicGOGHandler(IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(HeroicGOGHandler));
        var handler = new HeroicGOGHandler(fileSystem, logger);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunEGSHandler(IRegistry registry, IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(EGSHandler));
        var handler = new EGSHandler(registry, fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunOriginHandler(IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(OriginHandler));
        var handler = new OriginHandler(fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunEADesktopHandler(
        IFileSystem fileSystem,
        IHardwareInfoProvider hardwareInfoProvider)
    {
        var logger = _provider.CreateLogger(nameof(EADesktopHandler));
        var handler = new EADesktopHandler(fileSystem, hardwareInfoProvider);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:RequiresUnreferencedCodeAttribute",
        Justification = "Required types are preserved using TrimmerRootDescriptor file.")]
    private static void RunXboxHandler(IFileSystem fileSystem)
    {
        var logger = _provider.CreateLogger(nameof(XboxHandler));
        var handler = new XboxHandler(fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunSteamHandler(IFileSystem fileSystem, IRegistry? registry)
    {
        var logger = _provider.CreateLogger(nameof(SteamHandler));
        var handler = new SteamHandler(fileSystem, registry, _loggerFactory.CreateLogger<SteamHandler>());
        LogGamesAndErrors(handler.FindAllGames(), logger, game =>
        {
            if (!OperatingSystem.IsLinux()) return;
            var protonPrefix = game.GetProtonPrefix();
            if (protonPrefix is null) return;
            logger.LogInformation("Proton Directory for this game: {}", protonPrefix.ProtonDirectory.GetFullPath());
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
            }, error =>
            {
                logger.LogError("{Error}", error);
            });
        }

        return res;
    }

    private static void LogGamesAndErrors<TGame>(IEnumerable<OneOf<TGame, ErrorMessage>> results, ILogger logger, Action<TGame>? action = null)
        where TGame : class
    {
        foreach (var result in results)
        {
            result.Switch(game =>
            {
                logger.LogInformation("Found {Game}", game);
                action?.Invoke(game);
            }, error =>
            {
                logger.LogError("{Error}", error);
            });
        }
    }
}
