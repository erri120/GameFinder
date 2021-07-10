using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameFinder.RegistryUtils;
using GameFinder.StoreHandlers.Origin.DTO;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.Origin
{
    [PublicAPI]
    public class OriginHandler : AStoreHandler<OriginGame>
    {
        public override StoreType StoreType => StoreType.Origin;

        private readonly bool _useLocalFiles;
        private readonly bool _useApi;

        private readonly string? _localContentPath;
        
        public OriginHandler(bool useLocalFiles, bool useApi, ILogger? logger)
            : base(logger ?? NullLogger.Instance)
        {
            _useLocalFiles = useLocalFiles;
            _useApi = useApi;

            if (!_useLocalFiles && !_useApi)
                throw new ArgumentException("Usage of Local Files and API are both disabled!");

            _localContentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "Origin", "LocalContent");
        }

        public OriginHandler(string localContentPath, bool useLocalFiles, bool useApi, ILogger? logger)
            : this(useLocalFiles, useApi, logger)
        {
            _localContentPath = localContentPath;
        }
        
        public override bool FindAllGames()
        {
            if (_localContentPath == null || !Directory.Exists(_localContentPath))
            {
                Logger.LogError("LocalContent directory at {Path} does not exist", _localContentPath);
                return false;
            }

            var files = Directory
                .EnumerateFiles(_localContentPath, "*.mfst", SearchOption.AllDirectories)
                .Distinct()
                .ToList();

            if (_useLocalFiles)
            {
                List<ValueTuple<string, string>> tuples = files
                    .Select(x =>
                    {
                        var queryText = File.ReadAllText(x);
                        var query = HttpUtility.ParseQueryString(queryText, Encoding.UTF8);

                        if (!query.AllKeys.Contains("id")) return (string.Empty, string.Empty);
                        if (!query.AllKeys.Contains("dipinstallpath")) return (string.Empty, string.Empty);
                        
                        return (query["id"] ?? string.Empty, query["dipinstallpath"] ?? string.Empty);
                    })
                    .Where(x => !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2))
                    .ToList();

                if (tuples.Count == 0)
                    Logger.LogInformation("Found no Ids and Paths in the MFST files");
                
                foreach (var (id, installPath) in tuples)
                {
                    if (!Directory.Exists(installPath))
                    {
                        Logger.LogError("InstallPath for {Id} does not exist at {Path}", id, installPath);
                        continue;
                    }

                    var installerDataFile = Path.Combine(installPath, "__Installer", "installerdata.xml");
                    if (!File.Exists(installerDataFile))
                    {
                        Logger.LogError("Installer Manifest for {Id} does not exist at {Path}", id, installerDataFile);
                        continue;
                    }

                    var game = GetGameFromManifest(installerDataFile, Logger);
                    if (game == null) continue;

                    game.Id = id;
                    game.Path = installPath;

                    if (string.IsNullOrEmpty(game.Name))
                    {
                        var di = new DirectoryInfo(installPath);
                        game.Name = di.Name;
                    }
                    
                    Games.Add(game);
                }
            }

            if (_useApi)
            {
                var ids = files
                    .Select(x =>
                    {
                        var queryText = File.ReadAllText(x);
                        var query = HttpUtility.ParseQueryString(queryText, Encoding.UTF8);

                        return !query.AllKeys.Contains("id") ? null : query["id"];
                    })
                    .Where(x => x != null)
                    .Select(x => x!)
                    .Select(x =>
                    {
                        var index = x.IndexOf("@", StringComparison.OrdinalIgnoreCase);
                        return index == -1 ? x : x[..index];
                    })
                    .Distinct()
                    .ToList();

                if (!ids.Any())
                    Logger.LogError("Found no Ids for use with the Api");
                
                foreach (var id in ids)
                {
                    var game = GetGameFromAPI(id, Logger);
                    if (game == null)
                    {
                        Logger.LogError("Unable to get API Response for Game {Id}", id);
                        continue;
                    }

                    game.Id = id;

                    var existingGame = Games.FirstOrDefault(x => x.Id == game.Id);
                    if (existingGame != null)
                    {
                        CopyValues(existingGame, game);
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(game.Path))
                            Logger.LogWarning("Game {Id} was found using the Api but has no Path", game.Id);
                        
                        Games.Add(game);
                    }
                }
            }

            return true;
        }

        private static void CopyValues(OriginGame target, OriginGame source)
        {
            Type t = typeof(OriginGame);
            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                if (prop.Name == nameof(OriginGame.Path))
                {
                    // override path if it's null or white space
                    var path = (string?)prop.GetValue(target, null);
                    if (!string.IsNullOrWhiteSpace(path)) continue;
                }
                
                var value = prop.GetValue(source, null);
                if (value != null)
                    prop.SetValue(target, value, null);
            }
        }
        
        internal static OriginGame? GetGameFromAPI(string id, ILogger logger)
        {
            var game = new OriginGame();
            var apiResponse = GetApiResponse(id, logger).Result;
            if (apiResponse == null) return null;

            game.Name = apiResponse.ItemName ?? string.Empty;
            
            try
            {
                var installDir = GetInstallDir(id, apiResponse, logger);
                if (installDir != null)
                    game.Path = installDir;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while trying to find Installation Path of Game {Id}", id);
            }

            return game;
        }

        private static string? GetInstallDir(string id, ApiResponse apiResponse, ILogger logger)
        {
            var software = apiResponse.Publishing?.SoftwareList?.Software?.FirstOrDefault(x => x.SoftwarePlatform == "PCWIN");
            if (software == null)
            {
                logger.LogWarning("Found no PCWIN supported Software for Game {Id}", id);
                return null;
            }
            
            var path = software.FulfillmentAttributes?.InstallCheckOverride ?? software.FulfillmentAttributes?.ExecutePathOverride;
            if (path == null) return null;

            var pathSpan = path.AsSpan();
            
            //[HKEY_LOCAL_MACHINE\\SOFTWARE\\BioWare\\Dragon Age Inquisition\\Install Dir]\\DragonAgeInquisition.exe
            var startIndex = pathSpan.IndexOf("[", StringComparison.OrdinalIgnoreCase);
            var endIndex = pathSpan.IndexOf("]", StringComparison.OrdinalIgnoreCase);

            if (startIndex == -1 || endIndex == -1)
            {
                logger.LogError("Unable to correctly slice string \"{Path}\" for Game {Id}", path, id);
                return null;
            }
                
            //HKEY_LOCAL_MACHINE\\SOFTWARE\\BioWare\\Dragon Age Inquisition\\Install Dir
            var span = pathSpan.Slice(startIndex + 1, endIndex - 1);

            var splitCharIndex = span.IndexOf("\\", StringComparison.OrdinalIgnoreCase);
            var registryHiveType = span[..splitCharIndex].ToString();

            using var rootKey = registryHiveType switch
            {
                "HKEY_LOCAL_MACHINE" => RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64),
                "HKEY_CURRENT_USER" => RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64),
                _ => null
            };

            if (rootKey == null)
            {
                logger.LogError("Unknown Registry Hive: {Hive}", registryHiveType);
                return null;
            }

            var lastIndex = span.LastIndexOf('\\');
            var subKeyString = span.Slice(splitCharIndex + 1, lastIndex - splitCharIndex - 1).ToString();
            
            using var subKey = rootKey.OpenSubKey(subKeyString, RegistryRights.ReadKey);
            if (subKey == null)
            {
                logger.LogError("Unable to open subkey {Key} for Game {Id}", subKeyString, id);
                return null;
            }
            
            var valueName = span.Slice(lastIndex + 1, span.Length - lastIndex - 1).ToString();
            var value = RegistryHelper.GetStringValueFromRegistry(subKey, valueName, logger);
            return value;
        }
        
        internal static OriginGame? GetGameFromManifest(string file, ILogger logger)
        {
            var game = new OriginGame();
            var manifest = GetManifest(file, logger);
            if (manifest == null) return null;

            game.GameTitles = manifest.GameTitles?.GameTitle?.Select(x => new LocalizedString(x.Locale, x.Text)).ToList();
            game.Name = game.GameTitles?.FirstOrDefault(x => x.Locale == "en_US").Value ?? string.Empty;
            
            return game;
        }

        internal static async Task<ApiResponse?> GetApiResponse(string id, ILogger logger)
        {
            //https://api{N}.origin.com/ecommerce2/public/{ID}/en_US
            var random = new Random();
            var endpoint = random.Next(1, 5);

            var url = $"https://api{endpoint}.origin.com/ecommerce2/public/{id}/en_US";
            try
            {
                var res = await Utils.FromJsonAsync<ApiResponse>(url);
                if (res != null) return res;
                logger.LogError("Unable to fetch or deserialize response from Api for Game {Id} at {Url}", id, url);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while fetching or deserializing response from Api for Game {Id} at {Url}", id, url);
                return null;
            }
            
            return null;
        }
        
        internal static DiPManifest? GetManifest(string file, ILogger logger)
        {
            using var xmlReader = XmlReader.Create(file, new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
                ValidationType = ValidationType.None,
                CheckCharacters = false,
                ValidationFlags = XmlSchemaValidationFlags.None,
                DtdProcessing = DtdProcessing.Ignore,
                LineNumberOffset = 1
            });

            try
            {
                var xmlSerializer = new XmlSerializer(typeof(DiPManifest));
                var deserializedObject = xmlSerializer.Deserialize(xmlReader);

                if (deserializedObject is DiPManifest manifest) return manifest;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Exception while deserializing Manifest at {Path}", file);
                return null;
            }
            
            logger.LogError("Unable to deserialize Installer Data at {Path}", file);
            return null;
        }
    }
}
