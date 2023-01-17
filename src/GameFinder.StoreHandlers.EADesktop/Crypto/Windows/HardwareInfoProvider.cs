using System.Globalization;
using System.Runtime.Versioning;
using JetBrains.Annotations;
using static GameFinder.StoreHandlers.EADesktop.Crypto.Windows.WMIHelper;

namespace GameFinder.StoreHandlers.EADesktop.Crypto.Windows;

/// <summary>
/// Implementation of <see cref="IHardwareInfoProvider"/> that uses WMI and GetVolumeInformationW.
/// </summary>
[PublicAPI]
[SupportedOSPlatform("windows")]
public sealed class HardwareInfoProvider : IHardwareInfoProvider
{
    /// <inheritdoc/>
    public string? GetVolumeSerialNumber(out string? error)
    {
        error = null;

        var ok = Native.GetVolumeInformationW(
            "C:\\",
            null!,
            0,
            out var volumeSerialNumber,
            out _,
            out _,
            null!,
            0);

        if (ok) return volumeSerialNumber.ToString("X", CultureInfo.InvariantCulture);
        error = $"{nameof(Native.GetVolumeInformationW)} returned false";
        return null;
    }

    /// <inheritdoc/>
    public string? GetBaseBoardManufacturer(out string? error)
    {
        return GetWMIProperty(Win32BaseBoardClass, ManufacturerPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetBaseBoardSerialNumber(out string? error)
    {
        return GetWMIProperty(Win32BaseBoardClass, SerialNumberPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetBIOSManufacturer(out string? error)
    {
        return GetWMIProperty(Win32BIOSClass, ManufacturerPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetBIOSSerialNumber(out string? error)
    {
        return GetWMIProperty(Win32BIOSClass, SerialNumberPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetVideoControllerDeviceId(out string? error)
    {
        return GetWMIProperty(Win32VideoControllerClass, PNPDeviceIDPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetProcessorManufacturer(out string? error)
    {
        return GetWMIProperty(Win32ProcessorClass, ManufacturerPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetProcessorId(out string? error)
    {
        return GetWMIProperty(Win32ProcessorClass, ProcessorIDPropertyName, out error);
    }

    /// <inheritdoc/>
    public string? GetProcessorName(out string? error)
    {
        return GetWMIProperty(Win32ProcessorClass, NamePropertyName, out error);
    }
}
