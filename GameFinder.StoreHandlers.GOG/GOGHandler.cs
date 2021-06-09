using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using static GameFinder.ResultUtils;

namespace GameFinder.StoreHandlers.GOG
{
    [PublicAPI]
    public class GOGHandler : AStoreHandler<GOGGame>
    {
        /// <inheritdoc cref="AStoreHandler{TGame}.StoreType"/>
        public override StoreType StoreType => StoreType.GOG;

        private const string GOGRegKey = @"Software\GOG.com\Games";
        private const string GOG64RegKey = @"Software\WOW6432Node\GOG.com\Games";

        private readonly string? _regKey;

        private readonly List<string> _initErrors = new List<string>();
        
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
            {
                _initErrors.Add("GOG not found in registry!");
                return;
            }
            
            _regKey = GOG64RegKey;
        }

        /// <inheritdoc />
        public override Result<bool> FindAllGames()
        {
            if (_regKey == null)
                return NotOk(_initErrors);
            
            using var gogKey = Registry.LocalMachine.OpenSubKey(_regKey);
            if (gogKey == null)
            {
                return NotOk($"Unable to open Registry Key {_regKey}");
            }

            var res = new Result<bool>();
            if (_initErrors.Any())
                res.AppendErrors(_initErrors);
            
            var keys = gogKey.GetSubKeyNames();
            foreach (var key in keys)
            {
                if (!int.TryParse(key, out var gameID))
                {
                    res.AddError($"Unable to parse {key} as int!");
                    continue;
                }

                using var subKey = gogKey.OpenSubKey(key);
                if (subKey == null)
                {
                    res.AddError($"Unable to open sub-key {key} in {_regKey}");
                    continue;
                }
                
                var regRes = RegistryHelper.GetStringValueFromRegistry(subKey, "gameID");
                if (regRes.HasErrors)
                {
                    res.AppendErrors(regRes);
                    continue;
                }
                
                var sActualGameID = regRes.Value;
                if (!int.TryParse(sActualGameID, out var actualGameID))
                {
                    res.AddError($"Unable to parse {sActualGameID} as int in {subKey}");
                    continue;
                }

                if (gameID != actualGameID)
                {
                    res.AddError($"SubKey name does not match gameID value in {_regKey}: {gameID} != {actualGameID}");
                    continue;
                }

                regRes = RegistryHelper.GetStringValueFromRegistry(subKey, "BUILDID");
                if (regRes.HasErrors)
                {
                    res.AppendErrors(regRes);
                    continue;
                }

                var sBuildID = regRes.Value;
                if (!long.TryParse(sBuildID, out var buildID))
                {
                    res.AddError($"Unable to parse buildID {sBuildID} in {subKey}");
                    continue;
                }
                
                regRes = RegistryHelper.GetStringValueFromRegistry(subKey, "gameName");
                if (regRes.HasErrors)
                {
                    res.AppendErrors(regRes);
                    continue;
                }
                
                var gameName = regRes.Value;
                
                regRes = RegistryHelper.GetStringValueFromRegistry(subKey, "path");
                if (regRes.HasErrors)
                {
                    res.AppendErrors(regRes);
                    continue;
                }
                
                var path = regRes.Value;

                regRes = RegistryHelper.GetStringValueFromRegistry(subKey, "INSTALLDATE");
                if (regRes.HasErrors)
                {
                    res.AppendErrors(regRes);
                    continue;
                }
                
                var sInstallDate = regRes.Value;
                if (!DateTime.TryParse(sInstallDate, out var installationDate))
                {
                    res.AddError($"Unable to parse {sInstallDate} as DateTime in {key}");
                    continue;
                }
                
                var exe = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "exe");
                var exeFile = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "exeFile");
                var installerLanguage = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "installer_language");
                var langCode = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "lang_code");
                var language = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "language");
                var launchCommand = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "launchCommand");
                var launchParam = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "launchParam");
                
                regRes = RegistryHelper.GetStringValueFromRegistry(subKey, "productID");
                if (regRes.HasErrors)
                {
                    res.AppendErrors(regRes);
                    continue;
                }
                
                var sProductID = regRes.Value;
                if (!int.TryParse(sProductID, out var productID))
                {
                    res.AddError($"Unable to parse productID {productID} as int in {subKey}");
                    continue;
                }

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

            return Ok(res);
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "GOGHandler";
        }
    }
}
