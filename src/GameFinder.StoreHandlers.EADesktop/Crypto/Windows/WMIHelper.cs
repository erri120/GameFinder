using System;
using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Runtime.Versioning;

namespace GameFinder.StoreHandlers.EADesktop.Crypto.Windows;

[SupportedOSPlatform("windows")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class WMIHelper
{
    // private const string ComputerName = "localhost";
    // private const string Namespace = @"ROOT\CIMV2";
    // private const string QueryDialect = "WQL";

    public const string Win32BaseBoardClass = "Win32_BaseBoard";
    public const string Win32BIOSClass = "Win32_BIOS";
    public const string Win32VideoControllerClass = "Win32_VideoController";
    public const string Win32ProcessorClass = "Win32_Processor";

    public const string ManufacturerPropertyName = "Manufacturer";
    public const string SerialNumberPropertyName = "SerialNumber";
    public const string PNPDeviceIDPropertyName = "PNPDeviceId";
    public const string NamePropertyName = "Name";
    public const string ProcessorIDPropertyName = "ProcessorId";

    public static string? GetWMIProperty(string className, string propertyName, out string? error)
    {
        error = null;

        try
        {
            var query = $"SELECT {propertyName} FROM {className}";
            var selectQuery = new SelectQuery(query);
            var searcher = new ManagementObjectSearcher(selectQuery);

            using var results = searcher.Get();
            if (results.Count != 1)
            {
                error = $"Query did not return 1 element but {results.Count}:\n\"{query}\"";
                return null;
            }

            var arr = new ManagementBaseObject[1];
            results.CopyTo(arr, 0);

            var baseObject = arr[0];

            var propertyData = baseObject.Properties[propertyName];
            if (propertyData.Type == CimType.String) return (string)propertyData.Value;

            error = $"Property from query is not of type {nameof(CimType.String)} but {propertyData.Type}:\n\"{query}\"";
            return null;

        }
        catch (Exception e)
        {
            error = $"Exception while getting property {propertyName} from class {className}:\n{e}";
            return null;
        }
    }
}
