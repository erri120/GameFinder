using System;
using System.IO;
using GameFinder.RegistryUtils;
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
                throw new GOGNotFoundException("GOG not found in registry!");
            
            _regKey = GOG64RegKey;
        }

        /// <inheritdoc />
        public override bool FindAllGames()
        {
            using var gogKey = Registry.LocalMachine.OpenSubKey(_regKey);
            if (gogKey == null)
                throw new RegistryKeyNullException(_regKey);
            
            var keys = gogKey.GetSubKeyNames();
            foreach (var key in keys)
            {
                if (!int.TryParse(key, out var gameID))
                    throw new FormatException($"Unable to parse {key} as int!");

                using var subKey = gogKey.OpenSubKey(key);
                if (subKey == null)
                    throw new RegistryKeyNullException($"Unable to open sub-key {key} in {_regKey}");
                
                var sActualGameID = RegistryHelper.GetStringValueFromRegistry(subKey, "gameID");
                if (!int.TryParse(sActualGameID, out var actualGameID))
                    throw new FormatException($"Unable to parse gameID {gameID} in {subKey}");

                if (gameID != actualGameID)
                    throw new GOGException($"SubKey name does not match gameID value in {_regKey}: {gameID} != {actualGameID}");

                var sBuildID = RegistryHelper.GetStringValueFromRegistry(subKey, "BUILDID");
                if (!long.TryParse(sBuildID, out var buildID))
                    throw new FormatException($"Unable to parse buildID {sBuildID} in {subKey}");
                
                var gameName = RegistryHelper.GetStringValueFromRegistry(subKey, "gameName");
                var path = RegistryHelper.GetStringValueFromRegistry(subKey, "path");

                var sInstallDate = RegistryHelper.GetStringValueFromRegistry(subKey, "INSTALLDATE");
                if (!DateTime.TryParse(sInstallDate, out var installationDate))
                    throw new FormatException($"Unable to parse {sInstallDate} as DateTime in {key}");
                
                var exe = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "exe");
                var exeFile = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "exeFile");
                var installerLanguage = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "installer_language");
                var langCode = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "lang_code");
                var language = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "language");
                var launchCommand = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "launchCommand");
                var launchParam = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "launchParam");
                
                var sProductID = RegistryHelper.GetStringValueFromRegistry(subKey, "productID");
                if (!int.TryParse(sProductID, out var productID))
                    throw new FormatException($"Unable to parse productID {productID} as int in {subKey}");

                var saveGameFolder = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "savegamefolder");
                var startMenu = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "startMenu");
                var startMenuLink = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "startMenuLink");
                var supportLink = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "supportLink");
                var uninstallCommand = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "uninstallCommand");
                var version = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "ver");
                var workingDir = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "workingDir");

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
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "GOGHandler";
        }
    }
}
