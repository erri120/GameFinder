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
using GameFinder.Wine;
using GameFinder.Wine.Bottles;
using Microsoft.Extensions.Logging;
using NexusMods.Paths;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using FileSystem = NexusMods.Paths.FileSystem;
using IFileSystem = NexusMods.Paths.IFileSystem;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GameFinder.Example;

public static class Program
{
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
            Layout = "${longdate}|${level:uppercase=true}|${message:withexception=true}",
        };

        var fileTarget = new FileTarget("file")
        {
            FileName = "log.log",
        };

        config.AddRuleForAllLevels(coloredConsoleTarget);
        config.AddRuleForAllLevels(fileTarget);

        LogManager.Configuration = config;

        var logger = new NLogLoggerProvider().CreateLogger(nameof(Program));

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

        logger.LogInformation($"Operating System: {RuntimeInformation.OSDescription}");

        if (OperatingSystem.IsWindows())
        {

            var windowsRegistry = new WindowsRegistry();
            if (options.Steam) RunSteamHandler(realFileSystem, windowsRegistry, logger);
            if (options.GOG) RunGOGHandler(windowsRegistry, realFileSystem, logger);
            if (options.EGS) RunEGSHandler(windowsRegistry, realFileSystem, logger);
            if (options.Origin) RunOriginHandler(realFileSystem, logger);
            if (options.EADesktop)
            {
                var hardwareInfoProvider = new HardwareInfoProvider();
                var decryptionKey = Decryption.CreateDecryptionKey(new HardwareInfoProvider());
                var sDecryptionKey = Convert.ToHexString(decryptionKey).ToLower(CultureInfo.InvariantCulture);
                logger.LogDebug("EA Decryption Key: {}", sDecryptionKey);

                RunEADesktopHandler(realFileSystem, hardwareInfoProvider, logger);
            }
        }

        if (OperatingSystem.IsLinux())
        {
            if (options.Steam) RunSteamHandler(realFileSystem, registry: null, logger);
            var winePrefixes = new List<AWinePrefix>();

            if (options.Wine)
            {
                var prefixManager = new DefaultWinePrefixManager(realFileSystem);
                winePrefixes.AddRange(LogWinePrefixes(prefixManager, logger));
            }

            if (options.Bottles)
            {
                var prefixManager = new BottlesWinePrefixManager(realFileSystem);
                winePrefixes.AddRange(LogWinePrefixes(prefixManager, logger));
            }

            foreach (var winePrefix in winePrefixes)
            {
                var wineFileSystem = winePrefix.CreateOverlayFileSystem(realFileSystem);
                var wineRegistry = winePrefix.CreateRegistry(realFileSystem);

                if (options.GOG) RunGOGHandler(wineRegistry, wineFileSystem, logger);
                if (options.EGS) RunEGSHandler(wineRegistry, wineFileSystem, logger);
                if (options.Origin) RunOriginHandler(wineFileSystem, logger);
            }
        }
    }

    private static void RunGOGHandler(IRegistry registry, IFileSystem fileSystem, ILogger logger)
    {
        var handler = new GOGHandler(registry, fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunEGSHandler(IRegistry registry,
        IFileSystem fileSystem, ILogger logger)
    {
        var handler = new EGSHandler(registry, fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunOriginHandler(IFileSystem fileSystem, ILogger logger)
    {
        var handler = new OriginHandler(fileSystem);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunEADesktopHandler(
        IFileSystem fileSystem,
        IHardwareInfoProvider hardwareInfoProvider,
        ILogger logger)
    {
        var handler = new EADesktopHandler(fileSystem, hardwareInfoProvider);
        LogGamesAndErrors(handler.FindAllGames(), logger);
    }

    private static void RunSteamHandler(IFileSystem fileSystem, IRegistry? registry, ILogger logger)
    {
        var handler = new SteamHandler(fileSystem, registry);
        LogGamesAndErrors(handler.FindAllGames(), logger, game =>
        {
            if (!OperatingSystem.IsLinux()) return;
            var protonPrefix = game.GetProtonPrefix();
            if (!fileSystem.DirectoryExists(protonPrefix.ConfigurationDirectory)) return;
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
                logger.LogInformation($"Found wine prefix at {prefix.ConfigurationDirectory}");
                res.Add(prefix);
            }, error =>
            {
                logger.LogError(error.Value);
            });
        }

        return res;
    }

    private static void LogGamesAndErrors<TGame>(IEnumerable<Result<TGame>> results, ILogger logger, Action<TGame>? action = null)
        where TGame : class
    {
        foreach (var (game, error) in results)
        {
            if (game is not null)
            {
                logger.LogInformation("Found {}", game);
                action?.Invoke(game);
            }
            else
            {
                logger.LogError("{}", error);
            }
        }
    }
}
