using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace GameFinder.StoreHandlers.EADesktop.Crypto.Windows;

[SupportedOSPlatform("windows")]
[ExcludeFromCodeCoverage(Justification = "Uses DllImport.")]
internal static class Native
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-getvolumeinformationw
    /// </summary>
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
    public static extern bool GetVolumeInformationW(
        string rootPathName,                // [in, optional]  LPCWSTR lpRootPathName
        StringBuilder? volumeNameBuffer,    // [out, optional] LPWSTR  lpVolumeNameBuffer
        int volumeNameSize,                 // [in]            DWORD   nVolumeNameSize,
        out uint volumeSerialNumber,        // [out, optional] LPDWORD lpVolumeSerialNumber
        out uint maximumComponentLength,    // [out, optional] LPDWORD lpMaximumComponentLength
        out uint fileSystemFlags,           // [out, optional] LPDWORD lpFileSystemFlags
        StringBuilder? fileSystemNameBuffer,// [out, optional] LPWSTR  lpFileSystemNameBuffer
        int nFileSystemNameSize             // [in]            DWORD   nFileSystemNameSize
    );
}
