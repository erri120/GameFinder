using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.GOG
{
    [PublicAPI]
    public class GOGHandler : AStoreHandler<GOGGame>
    {
        /// <inheritdoc cref="AStoreHandler{TGame}.StoreType"/>
        public override StoreType StoreType => StoreType.GOG;

        private const string GOGRegKey = @"Software\GOG.com\Games";
        private const string GOG64RegKey = @"Software\WOW6432Node\GOG.com\Games";

        private readonly string _regKey;

        public GOGHandler()
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(GOGRegKey);
            if (regKey != null)
            {
                _regKey = GOGRegKey;
                return;
            }
            
            using var reg64Key = Registry.LocalMachine.OpenSubKey(GOG64RegKey);
            if (reg64Key == null)
                throw new NotImplementedException("GOG not found in registry!");
            
            _regKey = GOG64RegKey;
        }
        
        public override bool FindAllGames()
        {
            if (_regKey == null)
                throw new NotImplementedException("Registry Key is null!");

            using var gogKey = Registry.LocalMachine.OpenSubKey(_regKey);
            if (gogKey == null)
                throw new NotImplementedException($"Unable to open registry key {_regKey}");
            
            var keys = gogKey.GetSubKeyNames();
            foreach (var key in keys)
            {
                if (!int.TryParse(key, out var gameID))
                    throw new NotImplementedException($"Unable to parse {key} as int!");

                using var subKey = gogKey.OpenSubKey(key);
                if (subKey == null)
                    throw new NotImplementedException($"Unable to open sub-key {key} in {_regKey}");

                var sActualGameID = GetStringValueFromRegistry(subKey, "gameID");
                if (!int.TryParse(sActualGameID, out var actualGameID))
                    throw new NotImplementedException($"Unable to parse gameID {gameID} in {subKey}");

                if (gameID != actualGameID)
                    throw new NotImplementedException($"SubKey name does not match gameID value in {_regKey}: {gameID} != {actualGameID}");

                var sBuildID = GetStringValueFromRegistry(subKey, "BUILDID");
                if (!long.TryParse(sBuildID, out var buildID))
                    throw new NotImplementedException($"Unable to parse buildID {sBuildID} in {subKey}");
                
                var gameName = GetStringValueFromRegistry(subKey, "gameName");
                var exe = GetStringValueFromRegistry(subKey, "exe");
                var exeFile = GetStringValueFromRegistry(subKey, "exeFile");
                var path = GetStringValueFromRegistry(subKey, "path");

                if (!Directory.Exists(path))
                    throw new NotImplementedException($"Game Directory {path} from registry {key} does not exist!");

                var sInstallDate = GetStringValueFromRegistry(subKey, "INSTALLDATE");
                if (!DateTime.TryParse(sInstallDate, out var installationDate))
                    throw new NotImplementedException($"Unable to parse {sInstallDate} as DateTime in {key}");
                
                var installerLanguage = GetNullableStringValueFromRegistry(subKey, "installer_language");
                var langCode = GetNullableStringValueFromRegistry(subKey, "lang_code");
                var language = GetNullableStringValueFromRegistry(subKey, "language");
                var launchCommand = GetNullableStringValueFromRegistry(subKey, "launchCommand");
                var launchParam = GetNullableStringValueFromRegistry(subKey, "launchParam");
                
                var sProductID = GetStringValueFromRegistry(subKey, "productID");
                if (!int.TryParse(sProductID, out var productID))
                    throw new NotImplementedException($"Unable to parse productID {productID} in {subKey}");

                var saveGameFolder = GetNullableStringValueFromRegistry(subKey, "savegamefolder");
                var startMenu = GetNullableStringValueFromRegistry(subKey, "startMenu");
                var startMenuLink = GetNullableStringValueFromRegistry(subKey, "startMenuLink");
                var supportLink = GetNullableStringValueFromRegistry(subKey, "supportLink");
                var uninstallCommand = GetNullableStringValueFromRegistry(subKey, "uninstallCommand");
                var version = GetNullableStringValueFromRegistry(subKey, "ver");
                var workingDir = GetNullableStringValueFromRegistry(subKey, "workingDir");

                var game = new GOGGame
                {
                    Name = gameName,
                    Path = path,
                    
                    GameID = gameID,
                    ProductID = productID,
                    BuildID = buildID,
                    EXE = exe,
                    EXEFile = exeFile,
                    InstallationDate = installationDate,
                    InstallerLanguage = installerLanguage,
                    LangCode = langCode,
                    Language = language,
                    LaunchCommand = launchCommand,
                    LaunchParam = launchParam,
                    SaveGameFolder = saveGameFolder,
                    StartMenu = startMenu,
                    StartMenuLink = startMenuLink,
                    SupportLink = supportLink,
                    UninstallCommand = uninstallCommand,
                    Version = version,
                    WorkingDir = workingDir
                };
                
                Games.Add(game);
            }

            return true;
        }

        private static string GetStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            if (value == null)
                throw new NotImplementedException($"Key {key} does not have a {valueName} value!");

            var sValue = value.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(sValue))
                throw new NotImplementedException($"Value {valueName} in key {key} is null or empty!");

            return sValue;
        }
        
        private static string? GetNullableStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            var sValue = value?.ToString();
            return sValue;
        }
    }
}
