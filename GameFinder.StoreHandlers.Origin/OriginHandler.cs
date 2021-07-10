using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using GameFinder.StoreHandlers.Origin.DTO;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
                    .ToList();

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
                if (prop.Name == nameof(OriginGame.Path)) continue;
                
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
            
            return game;
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
            var res = await Utils.FromJsonAsync<ApiResponse>(url);
            
            if (res != null) return res;
            logger.LogError("Unable to fetch or deserialize response from Api for Game {Id} at {Url}", id, url);
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

            var xmlSerializer = new XmlSerializer(typeof(DiPManifest));
            var deserializedObject = xmlSerializer.Deserialize(xmlReader);

            if (deserializedObject is DiPManifest manifest) return manifest;
            logger.LogError("Unable to deserialize Installer Data at {Path}", file);
            return null;
        }
    }
}
