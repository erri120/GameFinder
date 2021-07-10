using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.EGS
{
    [PublicAPI]
    public class EGSHandler : AStoreHandler<EGSGame>
    {
        /// <inheritdoc/>
        public override StoreType StoreType => StoreType.EpicGamesStore;

        private const string RegKey = @"SOFTWARE\Epic Games\EOS";
        
        private readonly string? _metadataPath;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public EGSHandler() : this(NullLogger.Instance) { }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logger">Logger instance to use, will default to <see cref="NullLogger"/></param>
        public EGSHandler(ILogger? logger = null) : base(logger ?? NullLogger.Instance)
        {
            using var regKey = Registry.CurrentUser.OpenSubKey(RegKey);
            if (regKey == null)
            {
                Logger.LogError("Unable to open Registry Key {RegKey}", RegKey);
                return;
            }

            var modSdkMetadataDir = RegistryHelper.GetStringValueFromRegistry(regKey, "ModSdkMetadataDir", Logger);
            if (modSdkMetadataDir == null) return;
            
            if (!Directory.Exists(modSdkMetadataDir))
            {
                Logger.LogError("ModSdkMetadataDir from Registry does not exist at {Path}", modSdkMetadataDir);
                return;
            }

            _metadataPath = modSdkMetadataDir;
        }

        /// <summary>
        /// Constructor for providing the path of the metadata directory.
        /// </summary>
        /// <param name="metadataPath"></param>
        /// <param name="logger">Logger instance to use, will default to <see cref="NullLogger"/></param>
        /// <exception cref="ArgumentException">Provided path to the directory does not exist</exception>
        public EGSHandler(string metadataPath, ILogger? logger = null) : base(logger ?? NullLogger.Instance)
        {
            if (!Directory.Exists(metadataPath))
                throw new ArgumentException($"Metadata directory at {metadataPath} does not exist!", nameof(metadataPath));

            _metadataPath = metadataPath;
        }

        /// <inheritdoc />
        public override bool FindAllGames()
        {
            if (_metadataPath == null) return false;

            var itemFiles = Directory.EnumerateFiles(_metadataPath, "*.item", SearchOption.TopDirectoryOnly);
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
                    Logger.LogError("Unable to parse file {Path} as Json", itemFilePath);
                    continue;
                }

                if (manifestFile.FormatVersion != 0)
                {
                    Logger.LogError("FormatVersion in file {Path} is not supported: \"{Version}\"",
                        itemFilePath, manifestFile.FormatVersion);
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
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "EGSHandler";
        }
    }
}
