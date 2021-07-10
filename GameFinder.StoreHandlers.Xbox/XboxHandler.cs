using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GameFinder.StoreHandlers.Xbox.DTO;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

#if NET5_0
using System.Net.Http.Json;
#endif

namespace GameFinder.StoreHandlers.Xbox
{
    [PublicAPI]
    public class XboxHandler : AStoreHandler<XboxGame>
    {
        private const string XboxAppId = "Microsoft.GamingApp_8wekyb3d8bbwe";

        /// <inheritdoc/>
        public override StoreType StoreType => StoreType.Xbox;
        
        private readonly string? _xuid;
        private readonly bool _didInit;
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public XboxHandler() : this(NullLogger.Instance) { }

        /// <summary>
        /// Initializes the Xbox StoreHandler. The optional <paramref name="xuid"/> parameter is used for to request
        /// the title history of a user. See the README for more information.
        /// </summary>
        /// <param name="xuid"></param>
        /// <param name="logger">Logger instance to use, will default to <see cref="NullLogger"/></param>
        public XboxHandler(ILogger? logger = null, string? xuid = null) : base(logger ?? NullLogger.Instance)
        {
            var os = Environment.OSVersion;
            if (os.Platform != PlatformID.Win32NT)
            {
                Logger.LogError("OS Platform ID is {PlatformID} but {Win32NT} is required", os.Platform, PlatformID.Win32NT);
                return;
            }
            
            if (os.Version.Major < 10)
            {
                Logger.LogError("OS Version has to be Windows 10 or greater!");
                return;
            }

            _xuid = xuid;
            _didInit = true;
        }

        /// <inheritdoc />
        public override bool FindAllGames()
        {
            if (!_didInit) return false;
            
            var packages = WindowsUtils.GetUWPPackages();
            List<TitleHistoryResponse.Title>? titles = null;
            
            if (_xuid != null)
            {
                titles = GetTitlesFromXbox().Result;
            }
            
            foreach (var package in packages)
            {
                if (titles != null)
                {
                    var isXbox = titles.Any(x =>
                        x.PackageFamilyName != null && x.PackageFamilyName.Equals(package.Id.FamilyName));
                    if (!isXbox)
                        continue;
                }

                var game = new XboxGame
                {
                    Name = package.Id.Name,
                    Path = package.InstalledLocation.Path,

                    Description = package.Description,
                    Publisher = package.Id.Publisher,
                    DisplayName = package.DisplayName,
                    FamilyName = package.Id.FamilyName,
                    FullName = package.Id.FullName,
                    InstalledDate = package.InstalledDate,
                    PublisherId = package.Id.PublisherId,
                    InstalledLocationNameFolderRelativeId = package.InstalledLocation.FolderRelativeId,
                    InstalledLocationAttributes = package.InstalledLocation.Attributes,
                    InstalledLocationName = package.InstalledLocation.Name,
                    InstalledLocationDateCreated = package.InstalledLocation.DateCreated,
                    InstalledLocationDisplayName = package.InstalledLocation.DisplayName,
                    InstalledLocationDisplayType = package.InstalledLocation.DisplayType,
                    PublisherDisplayName = package.PublisherDisplayName
                };
                
                Utils.DoNoThrow(() => game.Logo = package.Logo);

                Games.Add(game);
            }

            return true;
        }

        private async Task<List<TitleHistoryResponse.Title>?> GetTitlesFromXbox()
        {
            if (_xuid == null) return new List<TitleHistoryResponse.Title>();

            var url = $"https://titlehub.xboxlive.com/users/xuid({_xuid})/titles/titlehistory/decoration/details";
            using var client = new HttpClient();
            
            using var response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

#if NET5_0
            var content = await response.Content.ReadFromJsonAsync<TitleHistoryResponse>(Utils.DefaultSerializerOptions)
                .ConfigureAwait(false);
#else
            var text = await response.Content.ReadAsStringAsync()
                .ConfigureAwait(false);
            var content = Utils.FromJson<TitleHistoryResponse>(text);
#endif

            return content?.Titles;
        }
        
        /// <inheritdoc />
        public override string ToString()
        {
            return "XboxHandler";
        }
    }
}
