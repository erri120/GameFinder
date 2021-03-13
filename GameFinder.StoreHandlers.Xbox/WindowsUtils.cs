using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace GameFinder.StoreHandlers.Xbox
{
    internal static class WindowsUtils
    {
        internal static IEnumerable<Package> GetUWPPackages()
        {
            var manager = new PackageManager();
            var user = WindowsIdentity.GetCurrent().User;
            if (user == null)
                throw new NotImplementedException();

            var packages = manager.FindPackagesForUser(user.Value)
                .Where(x => !x.IsFramework && !x.IsResourcePackage && x.SignatureKind == PackageSignatureKind.Store)
                .Where(x => x.InstalledLocation != null);
            return packages;
        }
    }
}
