using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Win32;
using Xunit;

namespace GameFinder.Tests
{
    public static class Setup
    {
        private static readonly bool IsCI;
        
        static Setup()
        {
            var ciEnv = Environment.GetEnvironmentVariable("CI", EnvironmentVariableTarget.Process);
            if (ciEnv == null) return;
            if (!bool.TryParse(ciEnv, out IsCI))
                IsCI = false;
        }

        public static void SetupSteam()
        {
            if (!IsCI) return;

            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.NotNull(currentDir);
            var steamDir = Path.Combine(currentDir!, "Steam");
            Directory.CreateDirectory(steamDir);
            
            var steamKey = Registry.CurrentUser.CreateSubKey(@"Software\Valve\Steam");
            steamKey.SetValue("SteamPath", steamDir);
            steamKey.Dispose();

            var steamConfigDir = Path.Combine(steamDir, "config");
            Directory.CreateDirectory(steamConfigDir);
            var steamConfig = Path.Combine(steamConfigDir, "config.vdf");
            File.WriteAllText(steamConfig, "Hello World!", Encoding.UTF8);

            var steamappsDir = Path.Combine(steamDir, "steamapps");
            Directory.CreateDirectory(steamappsDir);

            var manifestFile = GetFile("appmanifest_72850.acf");
            var manifestOutput = Path.Combine(steamappsDir, "appmanifest_72850.acf");
            
            File.Copy(manifestFile, manifestOutput, true);
            var gameDir = Path.Combine(steamappsDir, "common", "Skyrim");
            Directory.CreateDirectory(gameDir);
            
            Assert.True(File.Exists(steamConfig));
            Assert.True(File.Exists(manifestFile));
            Assert.True(Directory.Exists(gameDir));
        }

        public static void SetupGOG()
        {
            if (!IsCI) return;
            throw new NotImplementedException();
        }

        public static void SetupBethNet()
        {
            if (!IsCI) return;
            throw new NotImplementedException();
        }
        
        public static void SetupOrigin()
        {
            if (!IsCI) return;
            throw new NotImplementedException();
        }
        
        public static void SetupEpicGamesStore()
        {
            if (!IsCI) return;
            throw new NotImplementedException();
        }

        private static string GetFile(string name)
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.NotNull(currentDir);

            var filesDir = Path.Combine(currentDir!, "files");
            Assert.True(Directory.Exists(filesDir));

            var file = Path.Combine(filesDir, name);
            Assert.True(File.Exists(file));

            return file;
        }
    }
}
