using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GameFinder.StoreHandlers.Steam
{
    [PublicAPI]
    public class SteamHandler : AStoreHandler<SteamGame>
    {
        public override StoreType StoreType => StoreType.Steam;

        private const string SteamRegKey = @"Software\Valve\Steam";

        private bool DidInit { get; set; }
        public string SteamPath { get; internal set; } = string.Empty;
        private string SteamConfig { get; set; } = string.Empty;
        public List<string> SteamUniverses { get; internal set; } = new();
        
        public override bool Init()
        {
            if (DidInit)
                throw new ArgumentException("Init was already called!");

            using var steamKey = Registry.CurrentUser.OpenSubKey(SteamRegKey);

            var steamPathKey = steamKey?.GetValue("SteamPath");
            if (steamPathKey == null)
                throw new SteamNotFoundException($"SteamPath Key does not exist in registry at {SteamRegKey}");

            var steamPath = steamPathKey.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(steamPath))
                throw new SteamNotFoundException($"SteamPath Key in registry at {SteamRegKey} is null or empty!");

            if (!Directory.Exists(steamPath))
                throw new DirectoryNotFoundException($"Unable to find Steam at path found in registry: {steamPath}");
            
            var steamConfig = Path.Combine(steamPath, "config", "config.vdf");
            if (!File.Exists(steamConfig))
                throw new FileNotFoundException($"Unable to find Steam Config at path found in registry: {SteamConfig}");

            SteamPath = steamPath;
            SteamConfig = steamConfig;

            DidInit = true;
            return true;
        }

        private bool FindAllUniverses()
        {
            if (!DidInit)
                throw new ArgumentException("SteamHandler is not yet initialized!");

            var lines = File.ReadAllLines(SteamConfig, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (!line.ContainsCaseInsensitive("BaseInstallFolder_")) continue;
                var vdfValue = GetVdfValue(line);
                var path = Path.Combine(vdfValue, "steamapps");

                if (!Directory.Exists(path))
                    throw new DirectoryNotFoundException($"Unable to find Universe at {path}");
                
                SteamUniverses.Add(path);
            }

            var defaultPath = Path.Combine(SteamPath, "steamapps");
            if (Directory.Exists(defaultPath))
                SteamUniverses.Add(defaultPath);

            return true;
        }
        
        public override bool FindAllGames()
        {
            if (!DidInit)
                throw new ArgumentException("SteamHandler is not yet initialized!");

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
                                throw new FormatException($"Unable to parse appid \"{sID}\" as int in file {acfFile}");
                            
                            game.ID = id;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"name\""))
                        {
                            game.Name = GetVdfValue(line);
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"installdir\""))
                        {
                            var path = Path.Combine(universe, "common", GetVdfValue(line));
                            if (!Directory.Exists(path))
                                throw new DirectoryNotFoundException($"Unable to find installdir found in {acfFile}: {path}");
                            game.Path = path;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"LastUpdated\""))
                        {
                            var sTimeStamp = GetVdfValue(line);
                            if (!long.TryParse(sTimeStamp, out var timeStamp))
                                throw new FormatException($"Unable to parse LastUpdated \"{sTimeStamp}\" unix timestamp as long in file {acfFile}");
                            var dateTime = timeStamp.ToDateTime();
                            game.LastUpdated = dateTime;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"SizeOnDisk\""))
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new FormatException($"Unable to parse SizeOnDisk \"{sBytes}\" as long in file {acfFile}");
                            game.SizeOnDisk = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesToDownload\""))
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new FormatException($"Unable to parse BytesToDownload \"{sBytes}\" as long in file {acfFile}");
                            game.BytesToDownload = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesDownloaded\""))
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new FormatException($"Unable to parse BytesDownloaded \"{sBytes}\" as long in file {acfFile}");
                            game.BytesDownloaded = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesToStage\""))
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new FormatException($"Unable to parse BytesToStage \"{sBytes}\" as long in file {acfFile}");
                            game.BytesToStage = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesStaged\""))
                        {
                            var sBytes = GetVdfValue(line);
                            if (!long.TryParse(sBytes, out var bytes))
                                throw new FormatException($"Unable to parse BytesStaged \"{sBytes}\" as long in file {acfFile}");
                            game.BytesStaged = bytes;
                            continue;
                        }
                    }
                    
                    Games.Add(game);
                }
            }

            return true;
        }

        private static string GetVdfValue(string line)
        {
            var split = line.Split("\"");
            if (split.Length != 5)
                throw new SteamVdfParsingException(line, "Unable to parse vdf line: can not split line correctly");
            return split[3];
        }
    }
}
