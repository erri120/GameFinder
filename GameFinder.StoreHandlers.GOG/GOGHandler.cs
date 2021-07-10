using System;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.GOG
{
    [PublicAPI]
    public class GOGHandler : AStoreHandler<GOGGame>
    {
        /// <inheritdoc/>
        public override StoreType StoreType => StoreType.GOG;

        private const string GOGRegKey = @"Software\GOG.com\Games";
        private const string GOG64RegKey = @"Software\WOW6432Node\GOG.com\Games";

        private readonly string? _regKey;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public GOGHandler() : this(NullLogger.Instance) { }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">Logger instance to use, will default to <see cref="NullLogger"/></param>
        public GOGHandler(ILogger? logger = null) : base(logger ?? NullLogger.Instance)
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(GOGRegKey);
            if (regKey != null)
            {
                _regKey = GOGRegKey;
                return;
            }
            
            using var reg64Key = Registry.LocalMachine.OpenSubKey(GOG64RegKey);
            if (reg64Key == null)
            {
                Logger.LogError("GOG was not found in the registry");
                return;
            }
            
            _regKey = GOG64RegKey;
        }

        /// <inheritdoc />
        public override bool FindAllGames()
        {
            if (_regKey == null) return false;
            
            using var gogKey = Registry.LocalMachine.OpenSubKey(_regKey);
            if (gogKey == null)
            {
                Logger.LogError("Unable to open Registry Key {@Key}", _regKey);
                return false;
            }

            var keys = gogKey.GetSubKeyNames();
            foreach (var key in keys)
            {
                if (!int.TryParse(key, out var gameId))
                {
                    Logger.LogError("Unable to parse subkey name \"{Value}\" of {@RegistryKey} as int", key, gogKey);
                    continue;
                }

                using var subKey = gogKey.OpenSubKey(key);
                if (subKey == null)
                {
                    Logger.LogError("Unable to open subkey \"{Name}\" of {@RegistryKey}", key, gogKey);
                    continue;
                }
                
                var sActualGameId = RegistryHelper.GetStringValueFromRegistry(subKey, "gameID", Logger);
                if (sActualGameId == null) continue;
                
                if (!int.TryParse(sActualGameId, out var actualGameId))
                {
                    Logger.LogError("Unable to parse \"{Value}\" (\"{Name}\") of Registry Key {@RegistryKey} as {Type}",
                        sActualGameId, "gameID", subKey, "int");
                    continue;
                }

                if (gameId != actualGameId)
                {
                    Logger.LogError("Name of subkey does not match gameID value in {@RegistryKey}: {GameId} != {ActualGameId}",
                        subKey, gameId, actualGameId);
                    continue;
                }

                var sBuildId = RegistryHelper.GetStringValueFromRegistry(subKey, "BUILDID", Logger);
                if (sBuildId == null) continue;
                
                if (!long.TryParse(sBuildId, out var buildId))
                {
                    Logger.LogError("Unable to parse \"{Value}\" (\"{Name}\") of Registry Key {@RegistryKey} as {Type}",
                        sBuildId, "BUILDID", subKey, "int");
                    continue;
                }
                
                var gameName = RegistryHelper.GetStringValueFromRegistry(subKey, "gameName", Logger);
                if (gameName == null) continue;
                
                var path = RegistryHelper.GetStringValueFromRegistry(subKey, "path", Logger);
                if (path == null) continue;

                var sInstallDate = RegistryHelper.GetStringValueFromRegistry(subKey, "INSTALLDATE", Logger);
                if (sInstallDate == null) continue;
                
                if (!DateTime.TryParse(sInstallDate, out var installationDate))
                {
                    Logger.LogError("Unable to parse \"{Value}\" (\"{Name}\") of Registry Key {@RegistryKey} as {Type}",
                        sInstallDate, "INSTALLDATE", subKey, "DateTime");
                    continue;
                }
                
                var sProductId = RegistryHelper.GetStringValueFromRegistry(subKey, "productID", Logger);
                if (sProductId == null) continue;
                
                if (!int.TryParse(sProductId, out var productId))
                {
                    Logger.LogError("Unable to parse \"{Value}\" (\"{Name}\") of Registry Key {@RegistryKey} as {Type}",
                        sProductId, "productID", subKey, "int");
                    continue;
                }
                
                var exe = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "exe");
                var exeFile = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "exeFile");
                var installerLanguage = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "installer_language");
                var langCode = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "lang_code");
                var language = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "language");
                var launchCommand = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "launchCommand");
                var launchParam = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "launchParam");
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
                    
                    GameID = gameId,
                    ProductID = productId,
                    BuildID = buildId,
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
