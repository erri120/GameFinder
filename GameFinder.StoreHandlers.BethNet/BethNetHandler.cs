using System;
using System.Collections.Generic;
using System.IO;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using static GameFinder.ResultUtils;

namespace GameFinder.StoreHandlers.BethNet
{
    [PublicAPI]
    public class BethNetHandler : AStoreHandler<BethNetGame>
    {
        public override StoreType StoreType => StoreType.BethNet;

        private const string LauncherRegKey = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Bethesda.net";
        private const string UninstallRegKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
        
        public readonly string? LauncherPath;
        
        private readonly List<string> _initErrors = new List<string>();
        
        public BethNetHandler()
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(LauncherRegKey);
            if (regKey == null)
            {
                _initErrors.Add($"Unable to open Registry Key {LauncherRegKey}");
                return;
            }

            var regRes = RegistryHelper.GetStringValueFromRegistry(regKey, "installLocation");
            if (regRes.HasErrors)
            {
                _initErrors.AddRange(regRes.Errors);
                return;
            }
            
            var installLocation = regRes.Value;
            if (!Directory.Exists(installLocation))
            {
                _initErrors.Add($"Unable to find Bethesda.net Launcher at path from registry: {installLocation}");
                return;
            }
            
            LauncherPath = installLocation;
        }

        /// <inheritdoc />
        public override Result<bool> FindAllGames()
        {
            if (_initErrors.Count != 0)
                return NotOk(_initErrors);
            
            using var uninstallRegKey = Registry.LocalMachine.OpenSubKey(UninstallRegKey);
            if (uninstallRegKey == null)
            {
                return NotOk($"Unable to open Registry Key {UninstallRegKey}");
            }

            var res = new Result<bool>();
            
            //idea from https://github.com/TouwaStar/Galaxy_Plugin_Bethesda/blob/master/betty/local.py
            var subKeyNames = uninstallRegKey.GetSubKeyNames();
            foreach (var subKeyName in subKeyNames)
            {
                try
                {
                    using var subKey = uninstallRegKey.OpenSubKey(subKeyName);
                    if (subKey == null) continue;

                    var uninstallString = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "UninstallString");
                    if (uninstallString == null) continue;

                    if (!uninstallString.ContainsCaseInsensitive("bethesdanet://uninstall/")) continue;

                    var qWordRes = RegistryHelper.GetQWordValueFromRegistry(subKey, "ProductID");
                    if (qWordRes.HasErrors)
                    {
                        res.AppendErrors(qWordRes);
                        continue;
                    }

                    var productID = qWordRes.Value;

                    var stringRes = RegistryHelper.GetStringValueFromRegistry(subKey, "Path");
                    if (stringRes.HasErrors)
                    {
                        res.AppendErrors(stringRes);
                        continue;
                    }
                    
                    var path = stringRes.Value.RemoveQuotes();
                    
                    stringRes = RegistryHelper.GetStringValueFromRegistry(subKey, "DisplayName");
                    if (stringRes.HasErrors)
                    {
                        res.AppendErrors(stringRes);
                        continue;
                    }

                    var displayName = stringRes.Value;
                    
                    var game = new BethNetGame
                    {
                        Name = displayName,
                        Path = path,

                        ID = productID
                    };
                    
                    Games.Add(game);
                    continue;
                }
                catch (Exception)
                {
                    //ignore
                    continue;
                }
            }
            
            return Ok(res);
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "BethNetHandler";
        }
    }
}
