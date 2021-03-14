using System;
using System.IO;
using System.Linq;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.EGS
{
    [PublicAPI]
    public class EGSHandler : AStoreHandler<EGSGame>
    {
        public override StoreType StoreType => StoreType.EpicGamesStore;

        private const string RegKey = @"SOFTWARE\Epic Games\EOS";
        
        public readonly string MetadataPath;
        
        public EGSHandler()
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(RegKey);
            if (regKey == null)
                throw new RegistryKeyNullException(RegKey);

            var modSdkMetadataDir = RegistryHelper.GetStringValueFromRegistry(regKey, "ModSdkMetadataDir");
            if (!Directory.Exists(modSdkMetadataDir))
                throw new EGSException($"ModSdkMetadataDir from registry does not exist! {modSdkMetadataDir}");

            MetadataPath = modSdkMetadataDir;
        }

        public EGSHandler(string metadataPath)
        {
            if (!Directory.Exists(metadataPath))
                throw new ArgumentException($"Metadata directory at {metadataPath} does not exist!", nameof(metadataPath));

            MetadataPath = metadataPath;
        }
        
        public override bool FindAllGames()
        {
            var itemFiles = Directory.EnumerateFiles(MetadataPath, "*.item", SearchOption.TopDirectoryOnly);
            foreach (var itemFilePath in itemFiles)
            {
                //var id = Path.GetFileNameWithoutExtension(itemFilePath);
                var manifestFile = Utils.FromJson<EGSManifestFile>(itemFilePath);
                if (manifestFile == null)
                    throw new EGSException($"Unable to parse {itemFilePath} as Json!");

                if (manifestFile.FormatVersion != 0)
                    throw new EGSException("Not Supported", new NotSupportedException($"FormatVersion in file {itemFilePath} is not supported! {manifestFile.FormatVersion}"));
                
                var game = new EGSGame
                {
                    Name = manifestFile.DisplayName ?? manifestFile.FullAppName ?? manifestFile.AppName ?? throw new NotImplementedException(),
                    Path = manifestFile.InstallLocation!
                };
                CopyProperties(game, manifestFile);

                if (!Directory.Exists(game.InstallLocation))
                    throw new EGSException($"InstallLocation from {itemFilePath} does not exist! {game.InstallLocation}");
                
                Games.Add(game);
            }
            
            return true;
        }

        private static void CopyProperties(EGSGame game, EGSManifestFile manifestFile)
        {
            var manifestProperties = manifestFile.GetType().GetProperties();
            var gameProperties = game.GetType().GetProperties();

            foreach (var manifestProperty in manifestProperties)
            {
                if (manifestProperty == null) continue;
                var gameProperty = gameProperties.FirstOrDefault(x => x.Name.Equals(manifestProperty.Name));
                if (gameProperty == null) continue;
                
                gameProperty.SetValue(game, manifestProperty.GetValue(manifestFile));
            }
        }
    }
}
