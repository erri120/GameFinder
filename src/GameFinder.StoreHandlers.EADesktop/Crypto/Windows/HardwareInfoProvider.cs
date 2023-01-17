using System.Diagnostics.CodeAnalysis;
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
[ExcludeFromCodeCoverage(Justification = "Only available on Windows.")]
public sealed class HardwareInfoProvider : IHardwareInfoProvider
{
    /// <inheritdoc/>
    public string GetVolumeSerialNumber()
    {
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
        throw new HardwareInfoProviderException($"{nameof(Native.GetVolumeInformationW)} returned false", null);
    }

    /// <inheritdoc/>
    public string GetBaseBoardManufacturer()
    {
        return GetWMIProperty(Win32BaseBoardClass, ManufacturerPropertyName);
    }

    /// <inheritdoc/>
    public string GetBaseBoardSerialNumber()
    {
        return GetWMIProperty(Win32BaseBoardClass, SerialNumberPropertyName);
    }

    /// <inheritdoc/>
    public string GetBIOSManufacturer()
    {
        return GetWMIProperty(Win32BIOSClass, ManufacturerPropertyName);
    }

    /// <inheritdoc/>
    public string GetBIOSSerialNumber()
    {
        return GetWMIProperty(Win32BIOSClass, SerialNumberPropertyName);
    }

    /// <inheritdoc/>
    public string GetVideoControllerDeviceId()
    {
        return GetWMIProperty(Win32VideoControllerClass, PNPDeviceIDPropertyName);
    }

    /// <inheritdoc/>
    public string GetProcessorManufacturer()
    {
        return GetWMIProperty(Win32ProcessorClass, ManufacturerPropertyName);
    }

    /// <inheritdoc/>
    public string GetProcessorId()
    {
        return GetWMIProperty(Win32ProcessorClass, ProcessorIDPropertyName);
    }

    /// <inheritdoc/>
    public string GetProcessorName()
    {
        return GetWMIProperty(Win32ProcessorClass, NamePropertyName);
    }
}
