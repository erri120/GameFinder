using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using GameFinder.RegistryUtils;
using JetBrains.Annotations;
using Microsoft.Win32;
using static GameFinder.ResultUtils;

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
        public readonly string? SteamPath;
        private string? SteamConfig { get; set; }
        private string? SteamLibraries { get; set; }

        /// <summary>
        /// List of all found Steam Universes
        /// </summary>
        public List<string> SteamUniverses { get; internal set; } = new List<string>();

        /// <summary>
        /// True if steam was found.
        /// </summary>
        public readonly bool FoundSteam;

        private readonly List<string> _initErrors = new List<string>();
        
        /// <summary>
        /// SteamHandler constructor without arguments will try to find the Steam path using the registry
        /// </summary>
        public SteamHandler()
        {
            using var steamKey = Registry.CurrentUser.OpenSubKey(SteamRegKey);
            if (steamKey == null)
            {
                _initErrors.Add($"Unable to open registry key {steamKey}");
                return;
            }

            var regRes = RegistryHelper.GetStringValueFromRegistry(steamKey, "SteamPath");
            if (regRes.HasErrors)
            {
                _initErrors.AddRange(regRes.Errors);
                return;
            }

            var steamPath = regRes.Value;
            if (!Directory.Exists(steamPath))
            {
                _initErrors.Add($"Directory from path from registry does not exist: {steamPath}");
                return;
            }

            var steamConfig = Path.Combine(steamPath, "config", "config.vdf");
            if (!File.Exists(steamConfig))
            {
                _initErrors.Add($"Unable to find config.vdf at {steamConfig}");
                return;
            }

            var steamLibraries = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(steamLibraries))
            {
                _initErrors.Add($"Unable to find libraryfolders.vdf at {steamLibraries}");
                return;
            }

            FoundSteam = true;
            SteamPath = steamPath;
            SteamConfig = steamConfig;
            SteamLibraries = steamLibraries;
        }

        /// <summary>
        /// SteamHandler constructor with <paramref name="steamPath"/> argument, will not use the registry to find
        /// the Steam path.
        /// </summary>
        /// <param name="steamPath">Path to the directory containing <c>Steam.exe</c></param>
        /// <exception cref="ArgumentException"><paramref name="steamPath"/> is not a directory or does not exist</exception>
        public SteamHandler(string steamPath)
        {
            if (!Directory.Exists(steamPath))
                throw new ArgumentException($"Directory does not exist: {steamPath}", nameof(steamPath));
            
            var steamConfig = Path.Combine(steamPath, "config", "config.vdf");
            if (!File.Exists(steamConfig))
            {
                _initErrors.Add($"Unable to find config.vdf at {steamConfig}");
                return;
            }

            var steamLibraries = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(steamLibraries))
            {
                _initErrors.Add($"Unable to find libraryfolders.vdf at {steamLibraries}");
                return;
            }

            FoundSteam = true;
            SteamPath = steamPath;
            SteamConfig = steamConfig;
            SteamLibraries = steamLibraries;
        }

        private Result<bool> FindAllUniverses()
        {
            if (!FoundSteam) return NotOk(_initErrors);
            if (SteamConfig == null) return NotOk(_initErrors);
            if (SteamLibraries == null) return NotOk(_initErrors);

            var res = new Result<bool>();
            var lines = File.ReadAllLines(SteamConfig, Encoding.UTF8);
            foreach (var line in lines)
            {
                if (!line.ContainsCaseInsensitive("BaseInstallFolder_")) continue;
                var vdfRes = GetVdfValue(line);
                if (vdfRes.HasErrors)
                {
                    res.AppendErrors(vdfRes);
                    continue;
                }
                
                var path = Path.Combine(vdfRes.Value, "steamapps");

                if (!Directory.Exists(path))
                {
                    res.AddError($"Unable to find Universe at {path}");
                    continue;
                }
                
                SteamUniverses.Add(path);
            }

            lines = File.ReadAllLines(SteamLibraries, Encoding.UTF8);
            
            // old libraryfolders.vdf format
            var rx = new Regex(@"\s+""\d+\""\s+""(?<path>.+)""");
            foreach (var line in lines)
            {
                var matches = rx.Matches(line);
                foreach (Match match in matches)
                {
                    var groups = match.Groups;
                    var path = Path.Combine(groups["path"].Value, "steamapps");
                    if (!Directory.Exists(path))
                    {
                        res.AddError($"Unable to find Universe at {path}");
                        continue;
                    }

                    if (!SteamUniverses.Contains(path))
                    {
                        SteamUniverses.Add(path);
                    }
                }
            }

            // new libraryfolders.vdf format (2021-06)
            var inBlock = false;
            string? currentPath = null;
            bool? isMounted = null;
            for (var i = 2; i < lines.Length - 1; i++)
            {
                var line = lines[i];
                if (line.Contains("\t{"))
                {
                    if (inBlock)
                    {
                        res.AddError("Found new block while already in a block! This format is not supported, please report this on GitHub.");
                        break;
                    }
                    
                    inBlock = true;
                    continue;
                }

                if (line.Contains("\t}"))
                {
                    if (!inBlock)
                    {
                        res.AddError("Found end block statement but we are not in a block! This format is not supported, please report this on GitHub.");
                        break;
                    }
                    
                    if (currentPath != null && isMounted.HasValue && isMounted.Value)
                    {
                        if(!SteamUniverses.Contains(currentPath))
                            SteamUniverses.Add(currentPath);
                    }

                    currentPath = null;
                    isMounted = null;
                    inBlock = false;
                    continue;
                }

                if (inBlock && line.ContainsCaseInsensitive("path") || line.ContainsCaseInsensitive("mounted"))
                {
                    var vdfValue = GetVdfValue(line);
                    if (vdfValue.HasErrors)
                    {
                        res.AppendErrors(vdfValue.Errors);
                        continue;
                    }

                    if (line.ContainsCaseInsensitive("path"))
                    {
                        currentPath = Path.Combine(vdfValue.Value, "steamapps");
                    }
                    else
                    {
                        var sMounted = vdfValue.Value;
                        if (!int.TryParse(sMounted, out var iMounted))
                        {
                            res.AddError($"Unable to parse mounted \"{sMounted}\" as int");
                            continue;
                        }

                        isMounted = iMounted == 1;
                    }
                    
                }
            }
            
            if (SteamUniverses.Count == 0)
            {
                _initErrors.Add("Found 0 Steam Universes!");
            }
            
            if (SteamPath == null) return Ok(res);
            
            var defaultPath = Path.Combine(SteamPath, "steamapps");
            if (Directory.Exists(defaultPath))
                SteamUniverses.Add(defaultPath);

            return Ok(res);
        }

        /// <inheritdoc />
        public override Result<bool> FindAllGames()
        {
            if (!FoundSteam) return NotOk(_initErrors);
            if (SteamConfig == null) return NotOk(_initErrors);
            if (SteamLibraries == null) return NotOk(_initErrors);
            
            var universeRes = FindAllUniverses();
            if (!universeRes.Value)
                return NotOk(universeRes);
            
            var res = new Result<bool>();
            if (universeRes.HasErrors)
                res.AppendErrors(universeRes);
            if (_initErrors.Any())
                res.AppendErrors(_initErrors);
            
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
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sID = vdfRes.Value;
                            if (!int.TryParse(sID, out var id))
                            {
                                res.AddError($"Unable to parse appid \"{sID}\" as int in file {acfFile}");
                                break;
                            }
                            
                            game.ID = id;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"name\"") && game.Name == string.Empty)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }

                            game.Name = vdfRes.Value;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"installdir\"") && game.Path == string.Empty)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var path = Path.Combine(universe, "common", vdfRes.Value);
                            game.Path = path;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"LastUpdated\"") && game.LastUpdated == DateTime.UnixEpoch)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sTimeStamp = vdfRes.Value;
                            if (!long.TryParse(sTimeStamp, out var timeStamp))
                            {
                                res.AddError($"Unable to parse LastUpdated \"{sTimeStamp}\" unix timestamp as long in file {acfFile}");
                                break;
                            }
                            var dateTime = timeStamp.ToDateTime();
                            game.LastUpdated = dateTime;
                            continue;
                        }

                        if (line.ContainsCaseInsensitive("\"SizeOnDisk\"") && game.SizeOnDisk == -1)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sBytes = vdfRes.Value;
                            if (!long.TryParse(sBytes, out var bytes))
                            {
                                res.AddError($"Unable to parse SizeOnDisk \"{sBytes}\" as long in file {acfFile}");
                                break;
                            }
                            game.SizeOnDisk = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesToDownload\"") && game.BytesToDownload == -1)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sBytes = vdfRes.Value;
                            if (!long.TryParse(sBytes, out var bytes))
                            {
                                res.AddError($"Unable to parse BytesToDownload \"{sBytes}\" as long in file {acfFile}");
                                break;
                            }
                            game.BytesToDownload = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesDownloaded\"") && game.BytesDownloaded == -1)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sBytes = vdfRes.Value;
                            if (!long.TryParse(sBytes, out var bytes))
                            {
                                res.AddError($"Unable to parse BytesDownloaded \"{sBytes}\" as long in file {acfFile}");
                                break;
                            }
                            game.BytesDownloaded = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesToStage\"") && game.BytesToStage == -1)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sBytes = vdfRes.Value;
                            if (!long.TryParse(sBytes, out var bytes))
                            {
                                res.AddError($"Unable to parse BytesToStage \"{sBytes}\" as long in file {acfFile}");
                                break;
                            }
                            game.BytesToStage = bytes;
                            continue;
                        }
                        
                        if (line.ContainsCaseInsensitive("\"BytesStaged\"") && game.BytesStaged == -1)
                        {
                            var vdfRes = GetVdfValue(line);
                            if (vdfRes.HasErrors)
                            {
                                res.AppendErrors(vdfRes);
                                break;
                            }
                            
                            var sBytes = vdfRes.Value;
                            if (!long.TryParse(sBytes, out var bytes))
                            {
                                res.AddError($"Unable to parse BytesStaged \"{sBytes}\" as long in file {acfFile}");
                                break;
                            }
                            game.BytesStaged = bytes;
                            continue;
                        }
                    }
                    
                    Games.Add(game);
                }
            }

            return Ok(res);
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

        private static Result<string> GetVdfValue(string line)
        {
            var split = line.Split("\"");
            return split.Length != 5 
                ? Err<string>($"Unable to parse line in vdf file: can not split line correctly\n{line}") 
                : Ok(split[3]);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "SteamHandler";
        }
    }
}
