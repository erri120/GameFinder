using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using static GameFinder.ResultUtils;

namespace GameFinder.StoreHandlers.BethNet
{
    /// <summary>
    /// Store Handler for Bethesda.net Games. 
    /// </summary>
    [PublicAPI]
    public class BethNetHandler : AStoreHandler<BethNetGame>
    {
        /// <inheritdoc cref="AStoreHandler{TGame}.StoreType"/>
        public override StoreType StoreType => StoreType.BethNet;

        private const string Launcher32RegKey = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Bethesda.net";
        private const string Launcher64RegKey = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Bethesda.net";
        
        private const string Uninstall32RegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        private const string Uninstall64RegKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

        private const string UninstallString = "bethesdanet://uninstall";
        
        /// <summary>
        /// Path to the Launcher.
        /// </summary>
        public readonly string? LauncherPath;
        
        private readonly List<string> _initErrors = new List<string>();
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public BethNetHandler()
        {
            var regKey = Registry.LocalMachine.OpenSubKey(Launcher32RegKey);
            if (regKey == null)
            {
                regKey = Registry.LocalMachine.OpenSubKey(Launcher64RegKey);
                if (regKey == null)
                {
                    _initErrors.Add($"Unable to open Registry Keys {Launcher32RegKey} and {Launcher64RegKey}");
                    return;
                }
            }

            var regRes = RegistryHelper.GetStringValueFromRegistry(regKey, "installLocation");
            regKey.Dispose();
            
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
            var res = new Result<bool>();
            res.AppendErrors(_initErrors);

            var subKey32Names = new List<string>();
            var subKey64Names = new List<string>();
            
            using var uninstall32RegKey = Registry.LocalMachine.OpenSubKey(Uninstall32RegKey);
            if (uninstall32RegKey == null)
            {
                res.AddError($"Unable to open Registry Key {Uninstall32RegKey}");
            }
            else
            {
                subKey32Names.AddRange(uninstall32RegKey.GetSubKeyNames());
            }

            using var uninstall64RegKey = Registry.LocalMachine.OpenSubKey(Uninstall64RegKey);
            if (uninstall64RegKey == null)
            {
                res.AddError($"Unable to open Registry Key {Uninstall64RegKey}");
            }
            else
            {
                subKey64Names.AddRange(uninstall64RegKey.GetSubKeyNames());
            }

            if (!subKey32Names.Any() && !subKey64Names.Any())
            {
                res.AddError($"Did not find any Sub Keys in the Registry for {Uninstall32RegKey} and {Uninstall64RegKey}");
                return NotOk(res);
            }
            
            //idea from https://github.com/TouwaStar/Galaxy_Plugin_Bethesda/blob/master/betty/local.py

            void GetGames(RegistryKey? uninstallKey, IEnumerable<string> subKeyNames)
            {
                if (uninstallKey == null) return;
                
                foreach (var subKeyName in subKeyNames)
                {
                    try
                    {
                        using var subKey = uninstallKey.OpenSubKey(subKeyName);
                        if (subKey == null) continue;

                        var gameRes = GetGameFromRegistry(subKey);
                        if (gameRes.HasErrors)
                        {
                            res.AppendErrors(gameRes);
                        }

                        if (gameRes.Value == null) continue;
                    
                        Games.Add(gameRes.Value);
                    }
                    catch (Exception e)
                    {
                        res.AddError($"{e}");
                    }
                }
            }
            
            GetGames(uninstall32RegKey, subKey32Names);
            GetGames(uninstall64RegKey, subKey64Names);
            
            return Ok(res);
        }

        private static Result<BethNetGame?> GetGameFromRegistry(RegistryKey subKey)
        {
            var res = new Result<BethNetGame?>(null);

            try
            {
                var uninstallString = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "UninstallString");
                if (uninstallString == null) return res;

                if (!uninstallString.ContainsCaseInsensitive(UninstallString)) return res;

                var qWordRes = RegistryHelper.GetQWordValueFromRegistry(subKey, "ProductID");
                if (qWordRes.HasErrors)
                {
                    res.AppendErrors(qWordRes);
                    return res;
                }

                var productID = qWordRes.Value;

                var stringRes = RegistryHelper.GetStringValueFromRegistry(subKey, "Path");
                if (stringRes.HasErrors)
                {
                    res.AppendErrors(stringRes);
                    return res;
                }

                var path = stringRes.Value.RemoveQuotes();

                stringRes = RegistryHelper.GetStringValueFromRegistry(subKey, "DisplayName");
                if (stringRes.HasErrors)
                {
                    res.AppendErrors(stringRes);
                    return res;
                }

                var displayName = stringRes.Value;

                var game = new BethNetGame
                {
                    Name = displayName,
                    Path = path,

                    ID = productID
                };

                var t = new Result<BethNetGame?>(game);
                t.AppendErrors(res);
                return t;
            }
            catch (Exception e)
            {
                res.AddError($"{e}");
            }

            return res;
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "BethNetHandler";
        }
    }
}
