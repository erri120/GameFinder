using System;
using System.IO;
using CommandLine;
using GameFinder.StoreHandlers.BethNet;
using GameFinder.StoreHandlers.EGS;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Origin;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Xbox;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Extensions.Logging;
using NLog.Targets;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace GameFinder.Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            var consoleTarget = new ConsoleTarget("console");
            var fileTarget = new FileTarget("file")
            {
                FileName = "log.log"
            };

            config.AddRuleForAllLevels(consoleTarget);
            config.AddRuleForAllLevels(fileTarget);

            LogManager.Configuration = config;

            var logger = new NLogLoggerProvider().CreateLogger("");
            
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(x => Run(x, logger));
        }

        private static void RunHandler<THandler, TGame>(string handlerName, ILogger logger,
            Func<THandler> createHandler, Action<TGame> logGame) 
            where THandler : AStoreHandler<TGame> where TGame : AStoreGame
        {
            logger.LogInformation("Finding games for {HandlerName}", handlerName);
            try
            {
                var handler = createHandler();
                var res = handler.FindAllGames();
                if (!res)
                {
                    logger.LogError("Unable to find games for {HandlerName}", handlerName);
                    return;
                }

                logger.LogInformation("Found {Count} games for {HandlerName}", handler.Games.Count, handlerName);
                handler.Games.ForEach(logGame);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception trying to find games for {HandlerName}", handlerName);
            }
        }
        
        private static void Run(Options options, ILogger logger)
        {
            if (File.Exists("log.log"))
                File.Delete("log.log");
            
            if (options.BethNet)
            {
                RunHandler<BethNetHandler, BethNetGame>("BethNet", logger,
                    () => new BethNetHandler(logger),
                    x => logger.LogInformation("{Game}: {Path}", x, x.Path));
            }

            if (options.EGS)
            {
                RunHandler<EGSHandler, EGSGame>("EGS", logger,
                    () => new EGSHandler(logger),
                    x => logger.LogInformation("{Game}: {Path}", x, x.Path));
            }

            if (options.GOG)
            {
                RunHandler<GOGHandler, GOGGame>("GOG", logger,
                    () => new GOGHandler(logger),
                    x => logger.LogInformation("{Game}: {Path}", x, x.Path));
            }

            if (options.Steam)
            {
                RunHandler<SteamHandler, SteamGame>("Steam", logger,
                    () => new SteamHandler(logger),
                    x => logger.LogInformation("{Game}: {Path}", x, x.Path));
            }

            if (options.Xbox)
            {
                RunHandler<XboxHandler, XboxGame>("Xbox", logger,
                    () => new XboxHandler(logger),
                    x => logger.LogInformation("{Game}: {Path}", x, x.Path));
            }

            if (options.Origin)
            {
                RunHandler<OriginHandler, OriginGame>("Origin", logger,
                    () => new OriginHandler(true, true, logger),
                    x => logger.LogInformation("{Game}: {Path}", x, x.Path));
            }
        }
    }
}
