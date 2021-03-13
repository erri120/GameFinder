using System;
using CommandLine;
using GameFinder.StoreHandlers.BethNet;
using GameFinder.StoreHandlers.EGS;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.Xbox;

namespace GameFinder.Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run);
        }

        private static void Log(string s)
        {
            Console.WriteLine(s);
        }

        private static void RunHandler<THandler, TGame>(string handlerName, Action<TGame> logGame) where THandler : AStoreHandler<TGame>, new() where TGame : AStoreGame
        {
            Log($"Finding {handlerName} games...");
            try
            {
                var handler = new THandler();
                if (!handler.FindAllGames())
                {
                    Log($"Unable to find {handlerName} games");
                    return;
                }
                Log($"Found {handler.Games.Count} {handlerName} games");
                handler.Games.ForEach(logGame);
            }
            catch (Exception e)
            {
                Log($"Exception trying to find {handlerName} games:\n{e}");
            }
        }
        
        private static void Run(Options options)
        {
            if (options.BethNet)
            {
                RunHandler<BethNetHandler, BethNetGame>("BethNet", 
                    x => Log($"{x}: {x.Path}"));
            }

            if (options.EGS)
            {
                RunHandler<EGSHandler, EGSGame>("EGS",
                    x => Log($"{x}: {x.Path}"));
            }

            if (options.GOG)
            {
                RunHandler<GOGHandler, GOGGame>("GOG",
                    x => Log($"{x}: {x.Path}"));
            }

            if (options.Steam)
            {
                RunHandler<SteamHandler, SteamGame>("Steam",
                    x => Log($"{x}: {x.Path}"));
            }

            if (options.Xbox)
            {
                RunHandler<XboxHandler, XboxGame>("Xbox",
                    x => Log($"{x}: {x.Path}"));
            }
        }
    }
}
