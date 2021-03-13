using System;
using System.Linq;
using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.Xbox
{
    [PublicAPI]
    public class XboxHandler : AStoreHandler<XboxGame>
    {
        private const string XboxAppId = "Microsoft.GamingApp_8wekyb3d8bbwe";

        public override StoreType StoreType => StoreType.Xbox;
        
        public XboxHandler()
        {
            var os = Environment.OSVersion;
            if (os.Platform != PlatformID.Win32NT)
                throw new XboxAppNotFoundException($"Xbox App not found! OS Platform ID is not {PlatformID.Win32NT}");
            if (os.Version.Major < 10)
                throw new XboxAppNotFoundException($"Xbox App not found! OS Version has to be Windows 10 or greater!");
        }
        
        public override bool FindAllGames()
        {
            var packages = WindowsUtils.GetUWPPackages();

            foreach (var package in packages)
            {
                Games.Add(new XboxGame
                {
                    Name = package.Id.Name,
                    Path = package.InstalledLocation.Path,
                    
                    Description = package.Description,
                    Logo = package.Logo,
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
                });
            }

            return true;
        }
    }
}
