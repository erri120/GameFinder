using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using static GameFinder.ResultUtils;

namespace GameFinder.StoreHandlers.EGS
{
    [PublicAPI]
    public class EGSHandler : AStoreHandler<EGSGame>
    {
        public override StoreType StoreType => StoreType.EpicGamesStore;

        private const string RegKey = @"SOFTWARE\Epic Games\EOS";
        
        public readonly string? MetadataPath;
        
        private readonly List<string> _initErrors = new List<string>();

        public EGSHandler()
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(RegKey);
            if (regKey == null)
            {
                _initErrors.Add($"Unable to open Registry Key {RegKey}");
                return;
            }

            var regRes = RegistryHelper.GetStringValueFromRegistry(regKey, "ModSdkMetadataDir");
            if (regRes.HasErrors)
            {
                _initErrors.AddRange(regRes.Errors);
                return;
            }
            
            var modSdkMetadataDir = regRes.Value;
            if (!Directory.Exists(modSdkMetadataDir))
            {
                _initErrors.Add($"ModSdkMetadataDir from registry does not exist! {modSdkMetadataDir}");
                return;
            }

            MetadataPath = modSdkMetadataDir;
        }

        public EGSHandler(string metadataPath)
        {
            if (!Directory.Exists(metadataPath))
                throw new ArgumentException($"Metadata directory at {metadataPath} does not exist!", nameof(metadataPath));

            MetadataPath = metadataPath;
        }

        /// <inheritdoc />
        public override Result<bool> FindAllGames()
        {
            if (MetadataPath == null) return NotOk(_initErrors);

            var res = new Result<bool>();
            if (_initErrors.Any())
                res.AppendErrors(_initErrors);
            
            var itemFiles = Directory.EnumerateFiles(MetadataPath, "*.item", SearchOption.TopDirectoryOnly);
            foreach (var itemFilePath in itemFiles)
            {
                //var id = Path.GetFileNameWithoutExtension(itemFilePath);
                EGSManifestFile? manifestFile;
                
                try
                {
                    manifestFile = Utils.FromJson<EGSManifestFile>(itemFilePath);
                }
                catch (Exception)
                {
                    continue;
                }
                
                if (manifestFile == null)
                {
                    res.AddError($"Unable to parse {itemFilePath} as Json!");
                    continue;
                }

                if (manifestFile.FormatVersion != 0)
                {
                    res.AddError($"FormatVersion in file `{itemFilePath}` is not supported: `{manifestFile.FormatVersion}`");
                    continue;
                }
                
                var game = new EGSGame
                {
                    Name = manifestFile.DisplayName ?? manifestFile.FullAppName ?? manifestFile.AppName ?? throw new NotImplementedException(),
                    Path = manifestFile.InstallLocation!
                };
                CopyProperties(game, manifestFile);
                
                Games.Add(game);
            }
            
            return Ok(res);
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
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "EGSHandler";
        }
    }
}
