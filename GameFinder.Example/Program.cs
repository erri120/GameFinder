using System;
using GameFinder.StoreHandlers.Steam;

namespace GameFinder.Example
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var steamHandler = new SteamHandler();
            steamHandler.FindAllGames();
            foreach (var steamGame in steamHandler.Games)
            {
                Console.WriteLine($"{steamGame} is located at {steamGame.Path}");
            }
        }
    }
}
