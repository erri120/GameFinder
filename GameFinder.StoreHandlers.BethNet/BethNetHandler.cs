using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.BethNet
{
    /// <summary>
    /// Store Handler for Bethesda.net Games. 
    /// </summary>
    [PublicAPI]
    public class BethNetHandler : AStoreHandler<BethNetGame>
    {
        /// <inheritdoc />
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BethNetHandler() : this(NullLogger.Instance) { }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">Logger instance to use, will default to <see cref="NullLogger"/></param>
        public BethNetHandler(ILogger? logger = null) : base(logger ?? NullLogger.Instance)
        {
            var regKey = Registry.LocalMachine.OpenSubKey(Launcher32RegKey);
            if (regKey == null)
            {
                regKey = Registry.LocalMachine.OpenSubKey(Launcher64RegKey);
                if (regKey == null)
                {
                    Logger.LogError("Unable to open Registry Keys {32RegKey} and {64RegKey}",
                        Launcher32RegKey, Launcher64RegKey);
                    return;
                }
            }

            var installLocation = RegistryHelper.GetStringValueFromRegistry(regKey, "installLocation", Logger);
            regKey.Dispose();

            if (installLocation == null) return;
            if (!Directory.Exists(installLocation))
            {
                Logger.LogError("Unable to find Bethesda.net Launcher at path from Registry: {Path}", installLocation);
                return;
            }
            
            LauncherPath = installLocation;
        }

        /// <inheritdoc />
        public override bool FindAllGames()
        {
            if (LauncherPath == null) return false;
            
            var subKey32Names = new List<string>();
            var subKey64Names = new List<string>();
            
            using var uninstall32RegKey = Registry.LocalMachine.OpenSubKey(Uninstall32RegKey);
            if (uninstall32RegKey == null)
            {
                Logger.LogWarning("Unable to open Registry Key {RegKey}", Uninstall32RegKey);
            }
            else
            {
                subKey32Names.AddRange(uninstall32RegKey.GetSubKeyNames());
            }

            using var uninstall64RegKey = Registry.LocalMachine.OpenSubKey(Uninstall64RegKey);
            if (uninstall64RegKey == null)
            {
                Logger.LogWarning("Unable to open Registry Key {RegKey}", Uninstall64RegKey);
            }
            else
            {
                subKey64Names.AddRange(uninstall64RegKey.GetSubKeyNames());
            }

            if (!subKey32Names.Any() && !subKey64Names.Any())
            {
                Logger.LogError("Did not find any subkeys in Registry for {32RegKey} and {64RegKey}",
                    Uninstall32RegKey, Uninstall64RegKey);
                return false;
            }
            
            void GetGames(RegistryKey? uninstallKey, IEnumerable<string> subKeyNames)
            {
                if (uninstallKey == null) return;
                
                foreach (var subKeyName in subKeyNames)
                {
                    try
                    {
                        using var subKey = uninstallKey.OpenSubKey(subKeyName);
                        if (subKey == null) continue;

                        var game = GetGameFromRegistry(subKey, Logger);
                        if (game == null) continue;
                    
                        Games.Add(game);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e, "Exception while looking for a Bethesda.net Launcher Game at {RegistryKey}", subKeyName);
                    }
                }
            }
            
            GetGames(uninstall32RegKey, subKey32Names);
            GetGames(uninstall64RegKey, subKey64Names);

            return true;
        }

        private static BethNetGame? GetGameFromRegistry(RegistryKey subKey, ILogger logger)
        {
            try
            {
                var uninstallString = RegistryHelper.GetNullableStringValueFromRegistry(subKey, "UninstallString");
                if (uninstallString == null) return null;

                if (!uninstallString.ContainsCaseInsensitive(UninstallString)) return null;

                var qWordRes = RegistryHelper.GetQWordValueFromRegistry(subKey, "ProductID", logger);
                if (qWordRes == null) return null;

                var productID = qWordRes.Value;

                var path = RegistryHelper.GetStringValueFromRegistry(subKey, "Path", logger);
                if (path == null) return null;

                path = path.RemoveQuotes();
                
                var displayName = RegistryHelper.GetStringValueFromRegistry(subKey, "DisplayName", logger);
                if (displayName == null) return null;
                
                var game = new BethNetGame
                {
                    Name = displayName,
                    Path = path,
                    ID = productID
                };

                return game;
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Exception while looking for a Bethesda.net Launcher Game at {@RegistryKey}", subKey);
            }

            return null;
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "BethNetHandler";
        }
    }
}
