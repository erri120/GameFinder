using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using Microsoft.Win32;
using Xunit;

namespace GameFinder.Tests
{
    public static class Setup
    {
        private static bool IsCI => TestUtils.IsCI;

        public static string SetupSteam()
        {
            //if (!IsCI) return string.Empty;

            var currentDir = GetCurrentDir();
            var steamDir = Path.Combine(currentDir, "Steam");
            Directory.CreateDirectory(steamDir);

            var steamConfigDir = Path.Combine(steamDir, "config");
            Directory.CreateDirectory(steamConfigDir);
            var steamConfig = Path.Combine(steamConfigDir, "config.vdf");
            File.WriteAllText(steamConfig, "Hello World!", Encoding.UTF8);
            
            var steamappsDir = Path.Combine(steamDir, "steamapps");
            Directory.CreateDirectory(steamappsDir);

            var steamLibraries = Path.Combine(steamappsDir, "libraryfolders.vdf");
            File.WriteAllText(steamLibraries, "Hello World!", Encoding.UTF8);

            var manifestFile = GetFile("appmanifest_72850.acf");
            var manifestOutput = Path.Combine(steamappsDir, "appmanifest_72850.acf");
            
            File.Copy(manifestFile, manifestOutput, true);
            var gameDir = Path.Combine(steamappsDir, "common", "Skyrim");
            Directory.CreateDirectory(gameDir);
            
            Assert.True(File.Exists(steamConfig));
            Assert.True(File.Exists(manifestFile));
            Assert.True(Directory.Exists(gameDir));

            return steamDir;
        }

        public static void SetupGOG()
        {
            if (!IsCI) return;

            var currentDir = GetCurrentDir();
            var gogDir = Path.Combine(currentDir, "GOG");
            var gameDir = Path.Combine(gogDir, "Gwent");
            Directory.CreateDirectory(gameDir);
            var exePath = Path.Combine(gameDir, "Gwent.exe");
            File.WriteAllText(exePath, "Hello World!", Encoding.UTF8);
            var uninstallPath = Path.Combine(gameDir, "unins000.exe");
            File.WriteAllText(uninstallPath, "Hello World!", Encoding.UTF8);

            using var gamesKey = Registry.LocalMachine.CreateSubKey(@"Software\WOW6432Node\GOG.com\Games");
            using var gameKey = gamesKey.CreateSubKey("1971477531");
            gameKey.SetValue("BUILDID", "54099623651166556", RegistryValueKind.String);
            gameKey.SetValue("exe", exePath, RegistryValueKind.String);
            gameKey.SetValue("exeFile", "Gwent.exe", RegistryValueKind.String);
            gameKey.SetValue("gameID", "1971477531", RegistryValueKind.String);
            gameKey.SetValue("gameName", "Gwent", RegistryValueKind.String);
            gameKey.SetValue("INSTALLDATE", "2021-02-19 14:52:34", RegistryValueKind.String);
            gameKey.SetValue("installer_language", "english", RegistryValueKind.String);
            gameKey.SetValue("lang_code", "en-US", RegistryValueKind.String);
            gameKey.SetValue("language", "english", RegistryValueKind.String);
            gameKey.SetValue("launchCommand", exePath, RegistryValueKind.String);
            gameKey.SetValue("launchParam", "", RegistryValueKind.String);
            gameKey.SetValue("path", gameDir, RegistryValueKind.String);
            gameKey.SetValue("productID", "1971477531", RegistryValueKind.String);
            gameKey.SetValue("startMenu", "Gwent", RegistryValueKind.String);
            gameKey.SetValue("startMenuLink", @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Gwent [GOG.com]\Gwent", RegistryValueKind.String);
            gameKey.SetValue("supportLink", "", RegistryValueKind.String);
            gameKey.SetValue("uninstallCommand", uninstallPath, RegistryValueKind.String);
            gameKey.SetValue("ver", "8.2", RegistryValueKind.String);
            gameKey.SetValue("workingDir", gameDir, RegistryValueKind.String);
        }

        public static void CleanupGOG()
        {
            if (!IsCI) return;
            
            using var gamesKey = Registry.LocalMachine.OpenSubKey(@"Software\WOW6432Node\GOG.com\Games", true);
            gamesKey?.DeleteSubKeyTree("1971477531", true);
        }
        
        public static void SetupBethNet()
        {
            if (!IsCI) return;
            
            var currentDir = GetCurrentDir();
            var bethNetDir = Path.Combine(currentDir, "BethNet");
            var gameDir = Path.Combine(bethNetDir, "games", "Fallout Shelter");
            Directory.CreateDirectory(bethNetDir);
            Directory.CreateDirectory(gameDir);

            using var launcherRegKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Bethesda Softworks\Bethesda.net");
            launcherRegKey.SetValue("installLocation", bethNetDir, RegistryValueKind.String);
            
            using var regKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Fallout Shelter");
            regKey.SetValue("DisplayName", "Fallout Shelter", RegistryValueKind.String);
            regKey.SetValue("Path", gameDir, RegistryValueKind.String);
            regKey.SetValue("ProductID", (long)8, RegistryValueKind.QWord);
            regKey.SetValue("UninstallString", @"""c:\program files (x86)\bethesda.net launcher\bethesdanetupdater.exe"" bethesdanet://uninstall/8");
        }

        public static void CleanupBethNet()
        {
            if (!IsCI) return;
            //does not work atm due to UnauthorizedAccessException
            using var nodeKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Bethesda Softworks\", true);
            nodeKey?.DeleteSubKeyTree("Bethesda.net", true);

            using var uninstallKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", true);
            uninstallKey?.DeleteSubKeyTree("Fallout Shelter", true);
        }
        
        public static string SetupOrigin()
        {
            var originFolder = Path.Combine(GetCurrentDir(), "files", "origin");
            var localContentPath = Path.Combine(originFolder, "LocalContent");
            Directory.CreateDirectory(localContentPath);

            var file = Path.Combine(localContentPath, "Origin.OFR.50.0001131.mfst");
            var installPath = Path.Combine(originFolder, "Dragon Age Inquisition");

            var installerPath = Path.Combine(installPath, "__Installer");
            var installerManifest = Path.Combine(installerPath, "installerdata.xml");
            Assert.True(File.Exists(installerManifest));
            
            File.WriteAllText(file, $"?id=Origin.OFR.50.0001131&dipinstallpath={HttpUtility.UrlEncode(installPath)}", Encoding.UTF8);
            return localContentPath;
        }
        
        public static string SetupEpicGamesStore()
        {
            var currentDir = GetCurrentDir();
            var egsDir = Path.Combine(currentDir, "EGS");
            var manifestDir = Path.Combine(egsDir, "Manifests");
            var gameDir = Path.Combine(egsDir, "games", "UnrealTournament");

            Directory.CreateDirectory(egsDir);
            Directory.CreateDirectory(manifestDir);
            Directory.CreateDirectory(gameDir);
            
            var manifestFile = GetFile("8AAFB83044E76B812D3D8C9652E8C13C.item");
            var contents = File.ReadAllText(manifestFile, Encoding.UTF8);
            contents = contents.Replace("$InstallLocation$", gameDir.Replace(@"\", @"\\"));
            
            var manifestOutput = Path.Combine(manifestDir, "8AAFB83044E76B812D3D8C9652E8C13C.item");
            File.WriteAllText(manifestOutput, contents, Encoding.UTF8);

            return manifestDir;
        }

        public static string GetCurrentDir()
        {
            var currentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Assert.NotNull(currentDir);
            Assert.True(Directory.Exists(currentDir));
            return currentDir!;
        }
        
        public static string GetFile(string name)
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
