using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
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

        var consoleTarget = new ConsoleTarget("console");
        var fileTarget = new FileTarget("file")
        {
            FileName = "log.log",
        };

        config.AddRuleForAllLevels(consoleTarget);
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
            LogGamesAndErrors(results, logger);
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
    }

    private static void LogGamesAndErrors<TGame>(IEnumerable<Result<TGame>> results, ILogger logger)
        where TGame : class
    {
        foreach (var (game, error) in results)
        {
            if (game is not null)
            {
                logger.LogInformation("Found {}", game);
            }
            else
            {
                logger.LogError("{}", error);
            }
        }
    }
}
