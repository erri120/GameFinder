using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Abstractions;
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
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
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
            EnableAnsiOutput = false,
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
        if (File.Exists("log.log")) File.Delete("log.log");

        if (options.GOG)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogError("GOG Galaxy is only supported on Windows!");
            }
            else
            {
                var handler = new GOGHandler();
                var results = handler.FindAllGames();
                LogGamesAndErrors(results, logger);
            }
        }

        if (options.EGS)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogError("Epic Games Store is only supported on Windows!");
            }
            else
            {
                var handler = new EGSHandler();
                var results = handler.FindAllGames();
                LogGamesAndErrors(results, logger);
            }
        }

        if (options.Steam)
        {
            var handler = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new SteamHandler(new WindowsRegistry())
                : new SteamHandler(registry: null);

            var results = handler.FindAllGames();
            LogGamesAndErrors(results, logger, game =>
            {
                var protonPrefix = game.GetProtonPrefix();
                if (!Directory.Exists(protonPrefix.ProtonDirectory)) return;
                logger.LogInformation("Proton prefix directory: {}", protonPrefix.ProtonDirectory);
            });
        }

        if (options.Origin)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogError("Origin is only supported on Windows!");
            }
            else
            {
                var handler = new OriginHandler();
                var results = handler.FindAllGames();
                LogGamesAndErrors(results, logger);
            }
        }

        if (options.EADesktop)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                logger.LogError("EA Desktop is only supported on Windows!");
            }
            else
            {
                var decryptionKey = Decryption.CreateDecryptionKey(new HardwareInfoProvider());
                var sDecryptionKey = Convert.ToHexString(decryptionKey).ToLower(CultureInfo.InvariantCulture);
                logger.LogDebug("EA Decryption Key: {}", sDecryptionKey);

                var handler = new EADesktopHandler();
                var results = handler.FindAllGames();
                LogGamesAndErrors(results, logger);
            }
        }

        if (options.Wine)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logger.LogError("Wine is only supported on Linux!");
            }
            else
            {
                var prefixManager = new DefaultWinePrefixManager(new FileSystem());
                LogWinePrefixes(prefixManager, logger);
            }
        }

        if (options.Bottles)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                logger.LogError("Bottles is only supported on Linux!");
            }
            else
            {
                var prefixManager = new BottlesWinePrefixManager(new FileSystem());
                LogWinePrefixes(prefixManager, logger);
            }
        }
    }

    private static void LogWinePrefixes<TWinePrefix>(
        IWinePrefixManager<TWinePrefix> prefixManager, ILogger logger)
    where TWinePrefix : AWinePrefix
    {
        foreach (var result in prefixManager.FindPrefixes())
        {
            result.Switch(prefix =>
            {
                logger.LogInformation($"Found wine prefix at {prefix.ConfigurationDirectory}");
            }, error =>
            {
                logger.LogError(error.Value);
            });
        }
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
