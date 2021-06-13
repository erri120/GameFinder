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

            var configRes = ParseSteamConfig(SteamConfig);
            var libraryFoldersRes = ParseLibraryFolders(SteamLibraries);

            if (configRes.HasErrors)
                res.AppendErrors(configRes.Errors);
            if (libraryFoldersRes.HasErrors)
                res.AppendErrors(libraryFoldersRes.Errors);

            var allPaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            allPaths.UnionWith(configRes.Value);
            allPaths.UnionWith(libraryFoldersRes.Value);

            foreach (var path in allPaths)
            {
                var universePath = Path.Combine(path, "steamapps");
                if (!Directory.Exists(universePath))
                {
                    res.AddError($"Universe does not exist at {universePath}");
                    continue;
                }
                
                SteamUniverses.Add(universePath);
            }
            
            if (SteamPath == null)
            {
                if (SteamUniverses.Count == 0)
                    res.AddError("Found 0 Steam Universes!");
                
                return Ok(res);
            }
            
            var defaultPath = Path.Combine(SteamPath, "steamapps");
            if (Directory.Exists(defaultPath))
                SteamUniverses.Add(defaultPath);

            if (SteamUniverses.Count == 0)
                res.AddError("Found 0 Steam Universes!");
            
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
                    var parseRes = ParseAcfFile(acfFile);
                    if (parseRes.HasErrors)
                    {
                        res.AppendErrors(parseRes);
                    }

                    if (!parseRes.HasValue) continue;

                    var game = parseRes.Value;
                    game.Path = Path.Combine(universe, "common", game.Path);
                    
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
        
        internal static Result<List<string>> ParseSteamConfig(string file)
        {
            var res = new Result<List<string>>(new List<string>());

            if (!File.Exists(file))
            {
                res.AddError($"The config file at {file} does not exist!");
                return res;
            }

            var text = File.ReadAllText(file, Encoding.UTF8);
            
            var regex = new Regex(@"\""BaseInstallFolder_\d*\""\s*\""(?<path>[^\""]*)\""", RegexOptions.Compiled);
            var matches = regex.Matches(text);

            if (matches.Count == 0)
            {
                res.AddError($"Found no matches in file {file}");
                return res;
            }
            
            GetAllMatches("path", matches, path => res.Value.Add(MakeValidPath(path)));
            return res;
        }

        internal static Result<List<string>> ParseLibraryFolders(string file)
        {
            var res = new Result<List<string>>(new List<string>());

            if (!File.Exists(file))
            {
                res.AddError($"The config file at {file} does not exist!");
                return res;
            }

            var lines = File.ReadLines(file, Encoding.UTF8);
            var firstLine = lines.First();

            // old format (before 1623193086 2021-06-08)
            if (firstLine.Contains("LibraryFolders", StringComparison.Ordinal))
            {
                var text = File.ReadAllText(file, Encoding.UTF8);
                
                var regex = new Regex(@"\s+""\d+\""\s+""(?<path>.+)""", RegexOptions.Compiled);
                var matches = regex.Matches(text);

                if (matches.Count == 0)
                {
                    res.AddError($"Found no matches in file {file}");
                    return res;
                }
            
                GetAllMatches("path", matches, path => res.Value.Add(MakeValidPath(path)));
                return res;
            }

            // new format (after 1623193086 2021-06-08)
            if (firstLine.Contains("libraryfolders", StringComparison.CurrentCulture))
            {
                var text = File.ReadAllText(file, Encoding.UTF8);

                var pathRegex = new Regex(@"\""path\""\s*\""(?<path>[^\""]*)\""");
                var mountedRegex = new Regex(@"\""mounted\""\s*\""(?<mounted>[^\""]*)\""");

                var pathMatches = pathRegex.Matches(text);
                var mountedMatches = mountedRegex.Matches(text);

                if (pathMatches.Count == 0)
                {
                    res.AddError($"Found no path matches in file {file}");
                    return res;
                }

                if (mountedMatches.Count == 0)
                {
                    res.AddError($"Found no mounted matches in file {file}");
                    return res;
                }

                if (pathMatches.Count != mountedMatches.Count)
                {
                    res.AddError($"Number of path matches does not equal number of mounted matches in file {file}");
                    return res;
                }

                var pathValues = new List<string>();
                var mountedValues = new List<string>();
                
                GetAllMatches("path", pathMatches, path => pathValues.Add(path));
                GetAllMatches("mounted", mountedMatches, mounted => mountedValues.Add(mounted));

                if (pathValues.Count != mountedValues.Count)
                {
                    res.AddError($"Number of path values does not equal number of mounted values in file {file}");
                    return res;
                }
                
                for (var i = 0; i < pathValues.Count; i++)
                {
                    var path = pathValues[i];
                    var sMounted = mountedValues[i];
                    if (sMounted != "1") continue;
                    
                    res.Value.Add(MakeValidPath(path));
                }
                
                return res;
            }

            res.AddError($"Unknown libraryfolders.vdf format, file starts with {firstLine} at {file}");
            return res;
        }

        private static readonly IReadOnlyList<string> AcfKeys = new[]
        {
            "appid",
            "name",
            "installdir",
            "LastUpdated",
            "SizeOnDisk",
            "BytesToDownload",
            "BytesDownloaded",
            "BytesToStage",
            "BytesStaged"
        };
        
        internal static Result<SteamGame> ParseAcfFile(string file)
        {
            var res = new Result<SteamGame>(null!);

            if (!File.Exists(file))
            {
                res.AddError($"Manifest {file} does not exist!");
                return res;
            }

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            
            var lines = File.ReadLines(file, Encoding.UTF8);
            foreach (var line in lines)
            {
                var first = AcfKeys.FirstOrDefault(x => line.ContainsCaseInsensitive($"\"{x}\""));
                if (first == null) continue;
                if (dict.ContainsKey(first)) continue;

                var vdfRes = GetVdfValue(line);
                if (vdfRes.HasErrors)
                {
                    res.AppendErrors(vdfRes);
                    continue;
                }

                var value = vdfRes.Value;
                dict.Add(first, value);
            }

            if (dict.Count != AcfKeys.Count)
            {
                foreach (var acfKey in AcfKeys)
                {
                    if (dict.TryGetValue(acfKey, out _))
                        continue;
                    res.AddError($"Manifest {file} does not contain a value with key {acfKey}");
                }
                
                return res;
            }
            
            var game = new SteamGame();

            var sAppId = dict["appid"];
            var name = dict["name"];
            var installDir = dict["installdir"];
            var sLastUpdated = dict["LastUpdated"];
            var sSizeOnDisk = dict["SizeOnDisk"];
            var sBytesToDownload = dict["BytesToDownload"];
            var sBytesDownloaded = dict["BytesDownloaded"];
            var sBytesToStage = dict["BytesToStage"];
            var sBytesStaged = dict["BytesStaged"];

            if (!int.TryParse(sAppId, out var appId))
            {
                res.AddError($"Unable to parse appid {sAppId} as int in Manifest {file}");
                return res;
            }

            game.ID = appId;

            bool ParseAndSet(string value, string key, Action<long> action)
            {
                if (!long.TryParse(value, out var lValue))
                {
                    res.AddError($"Unable to parse {key} as long in Manifest {file}");
                    return false;
                }

                action(lValue);
                return true;
            }

            if (!ParseAndSet(sLastUpdated, "LastUpdated", timeStamp =>
            {
                var dateTime = timeStamp.ToDateTime();
                game.LastUpdated = dateTime;
            }))
            {
                return res;
            }

            if (!ParseAndSet(sSizeOnDisk, "SizeOnDisk", sizeOnDisk => game.SizeOnDisk = sizeOnDisk))
                return res;

            if (!ParseAndSet(sBytesToDownload, "BytesToDownload",
                bytesToDownload => game.BytesToDownload = bytesToDownload))
                return res;

            if (!ParseAndSet(sBytesDownloaded, "BytesDownloaded",
                bytesDownloaded => game.BytesDownloaded = bytesDownloaded))
                return res;

            if (!ParseAndSet(sBytesToStage, "BytesToStage", bytesToStage => game.BytesToStage = bytesToStage))
                return res;

            if (!ParseAndSet(sBytesStaged, "BytesStaged", bytesStaged => game.BytesStaged = bytesStaged))
                return res;

            game.Name = name;
            game.Path = installDir;

            var vRes = new Result<SteamGame>(game);
            vRes.AppendErrors(res);
            return vRes;
        }
        
        private static void GetAllMatches(string group, MatchCollection matches, Action<string> action)
        {
            foreach (Match match in matches)
            {
                var groups = match.Groups;
#if NET5_0
                if (!groups.TryGetValue(group, out var currentGroup)) continue;
                if (!currentGroup.Success) continue;
#elif NETSTANDARD2_1
                var currentGroup = groups.FirstOrDefault(x => x.Success && x.Name.Equals(group, StringComparison.OrdinalIgnoreCase));
                if (currentGroup == null) continue;
#endif

                action(currentGroup.Value);
            }
        }
        
        private static string MakeValidPath(string input)
        {
            var sb = new StringBuilder(input);
            sb.Replace("\\\\", "\\");
            return sb.ToString();
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
