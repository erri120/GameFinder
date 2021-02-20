using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.Steam
{
    /// <summary>
    /// Store Handler for Steam Games
    /// </summary>
    [PublicAPI]
    public class SteamHandler : AStoreHandler<SteamGame>
    {
        /// <inheritdoc cref="AStoreHandler{TGame}.StoreType"/>
        public override StoreType StoreType => StoreType.Steam;

        private const string SteamRegKey = @"Software\Valve\Steam";

        /// <summary>
        /// Path to the Steam Installation Directory
        /// </summary>
        public readonly string SteamPath;
        private string SteamConfig { get; set; }
        
        /// <summary>
        /// List of all found Steam Games
        /// </summary>
        public List<string> SteamUniverses { get; internal set; } = new();

        /// <summary>
        /// SteamHandler constructor without arguments will try to find the Steam path using the registry
        /// </summary>
        /// <exception cref="SteamNotFoundException">Steam was not found</exception>
        public SteamHandler()
        {
            using var steamKey = Registry.CurrentUser.OpenSubKey(SteamRegKey);

            var steamPathKey = steamKey?.GetValue("SteamPath");
            if (steamPathKey == null)
                throw new SteamNotFoundException($"SteamPath Key does not exist in registry at {steamKey}");

            var steamPath = steamPathKey.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(steamPath))
                throw new SteamNotFoundException($"SteamPath Key in registry at {steamKey} is null or empty!");

            if (!Directory.Exists(steamPath))
                throw new SteamNotFoundException($"Unable to find Steam at path found in registry: {steamPath}");
            
            var steamConfig = Path.Combine(steamPath, "config", "config.vdf");
            if (!File.Exists(steamConfig))
                throw new SteamNotFoundException($"Unable to find Steam Config: {SteamConfig}");

            SteamPath = steamPath;
            SteamConfig = steamConfig;
        }

        /// <summary>
        /// SteamHandler constructor with <paramref name="steamPath"/> argument, will not use the registry to find
        /// the Steam path.
        /// </summary>
        /// <param name="steamPath">Path to the directory containing <c>Steam.exe</c></param>
        /// <exception cref="ArgumentException"><paramref name="steamPath"/> is not a directory or does not exist</exception>
        /// <exception cref="SteamNotFoundException">Unable to find Steam Config in <paramref name="steamPath"/></exception>
        public SteamHandler(string steamPath)
        {
            if (!Directory.Exists(steamPath))
                throw new ArgumentException($"Directory does not exist: {steamPath}", nameof(steamPath));

            var steamConfig = Path.Combine(steamPath, "config", "config.vdf");
            if (!File.Exists(steamConfig))
                throw new SteamNotFoundException($"Unable to find Steam Config: {SteamConfig}");
            
            SteamPath = steamPath;
            SteamConfig = steamConfig;
        }

        private bool FindAllUniverses()
        {
            var lines = File.ReadAllLines(SteamConfig, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (!line.ContainsCaseInsensitive("BaseInstallFolder_")) continue;
                var vdfValue = GetVdfValue(line);
                var path = Path.Combine(vdfValue, "steamapps");

                if (!Directory.Exists(path))
                    throw new SteamUniverseNotFoundException($"Unable to find Universe at {path}");
                
                SteamUniverses.Add(path);
            }

            var defaultPath = Path.Combine(SteamPath, "steamapps");
            if (Directory.Exists(defaultPath))
                SteamUniverses.Add(defaultPath);

            return true;
        }
        
        public override bool FindAllGames()
        {
            if (!FindAllUniverses())
                return false;

            foreach (var universe in SteamUniverses)
            {
                var acfFiles = Directory.EnumerateFiles(universe, "*.acf", SearchOption.TopDirectoryOnly);
                foreach (var acfFile in acfFiles)
                {
                    var lines = File.ReadAllLines(acfFile, Encoding.UTF8);
                    var game = new SteamGame();

                    foreach (var line in lines)
                    {
                        if (line.ContainsCaseInsensitive("\"appid\""))
                        {
                            var sID = GetVdfValue(line);
                            if (!int.TryParse(sID, out var id))
                                throw new SteamVdfParsingException(line, $"Unable to parse appid \"{sID}\" as int in file {acfFile}");
                            
                            game.ID = id;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"name\"") && game.Name == string.Empty)
                        {
                            game.Name = GetVdfValue(line);
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"installdir\"") && game.Path == string.Empty)
                        {
                            var path = Path.Combine(universe, "common", GetVdfValue(line));
                            if (!Directory.Exists(path))
                                throw new SteamGameNotFoundException($"Unable to find installdir found in {acfFile}: {path}");
                            game.Path = path;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"LastUpdated\"") && game.LastUpdated == DateTime.UnixEpoch)
                        {
                            var sTimeStamp = GetVdfValue(line);
                            if (!long.TryParse(sTimeStamp, out var timeStamp))
                                throw new SteamVdfParsingException(line, $"Unable to parse LastUpdated \"{sTimeStamp}\" unix timestamp as long in file {acfFile}");
                            var dateTime = timeStamp.ToDateTime();
                            game.LastUpdated = dateTime;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"SizeOnDisk\"") && game.SizeOnDisk == -1)
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new SteamVdfParsingException(line, $"Unable to parse SizeOnDisk \"{sBytes}\" as long in file {acfFile}");
                            game.SizeOnDisk = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesToDownload\"") && game.BytesToDownload == -1)
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new SteamVdfParsingException(line, $"Unable to parse BytesToDownload \"{sBytes}\" as long in file {acfFile}");
                            game.BytesToDownload = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesDownloaded\"") && game.BytesDownloaded == -1)
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new SteamVdfParsingException(line, $"Unable to parse BytesDownloaded \"{sBytes}\" as long in file {acfFile}");
                            game.BytesDownloaded = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesToStage\"") && game.BytesToStage == -1)
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new SteamVdfParsingException(line, $"Unable to parse BytesToStage \"{sBytes}\" as long in file {acfFile}");
                            game.BytesToStage = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesStaged\"") && game.BytesStaged == -1)
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new SteamVdfParsingException(line, $"Unable to parse BytesStaged \"{sBytes}\" as long in file {acfFile}");
                            game.BytesStaged = bytes;
                            continue;
                        }
                    }
                    
                    Games.Add(game);
                }
            }

            return true;
        }

        /// <summary>
        /// Get Game by Steam ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SteamGame GetByID(int id)
        {
            return Games.First(x => x.ID == id);
        }

        /// <summary>
        /// Try get Game by Steam ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool TryGetByID(int id, [MaybeNullWhen(false)] out SteamGame? game)
        {
            game = Games.FirstOrDefault(x => x.ID == id);
            return game != null;
        }

        private static string GetVdfValue(string line)
        {
            var split = line.Split("\"");
            if (split.Length != 5)
                throw new SteamVdfParsingException(line, "Unable to parse lin in vdf file: can not split line correctly");
            return split[3];
        }
    }
}
