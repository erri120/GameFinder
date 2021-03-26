using System;
using System.IO;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.BethNet
{
    [PublicAPI]
    public class BethNetHandler : AStoreHandler<BethNetGame>
    {
        public override StoreType StoreType => StoreType.BethNet;

        private const string LauncherRegKey = @"SOFTWARE\WOW6432Node\Bethesda Softworks\Bethesda.net";
        private const string UninstallRegKey = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
        
        public readonly string LauncherPath;
        
        public BethNetHandler()
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(LauncherRegKey);
            if (regKey == null)
                throw new RegistryKeyNullException(LauncherRegKey);

            var installLocation = RegistryHelper.GetStringValueFromRegistry(regKey, "installLocation");
            if (!Directory.Exists(installLocation))
                throw new BethNetNotFoundException($"Unable to find Bethesda.net Launcher at path from registry: {installLocation}");
            
            LauncherPath = installLocation;
        }
        
        public override bool FindAllGames()
        {
            using var uninstallRegKey = Registry.LocalMachine.OpenSubKey(UninstallRegKey);
            if (uninstallRegKey == null)
                throw new RegistryKeyNullException(UninstallRegKey);

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

                    var productID = RegistryHelper.GetQWordValueFromRegistry(subKey, "ProductID");
                    var path = RegistryHelper.GetStringValueFromRegistry(subKey, "Path").RemoveQuotes();
                    var displayName = RegistryHelper.GetStringValueFromRegistry(subKey, "DisplayName");
                    
                    var game = new BethNetGame
                    {
                        Name = displayName,
                        Path = path,

                        ID = productID
                    };
                    
                    Games.Add(game);
                    continue;
                }
                //rethrow all custom exceptions but ignore everything else
                catch (RegistryException)
                {
                    throw;
                }
                catch (StoreHandlerException)
                {
                    throw;
                }
                catch (Exception)
                {
                    //ignore
                    continue;
                }
            }
            
            return true;
        }
    }
}
