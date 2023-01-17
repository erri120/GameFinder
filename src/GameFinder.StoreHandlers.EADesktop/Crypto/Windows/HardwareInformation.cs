using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GameFinder.StoreHandlers.EADesktop.Crypto.Windows;

internal static class HardwareInformation
{
    [SuppressMessage("Design", "MA0051:Method is too long")]
    public static string? GenerateHardwareString(IHardwareInfoProvider hardwareInfoProvider, out string? error)
    {
        var sb = new StringBuilder();

        var baseBoardManufacturer = hardwareInfoProvider.GetBaseBoardManufacturer(out error);
        if (baseBoardManufacturer is null) return null;

        var baseBoardSerialNumber = hardwareInfoProvider.GetBaseBoardSerialNumber(out error);
        if (baseBoardSerialNumber is null) return null;

        var biosManufacturer = hardwareInfoProvider.GetBIOSManufacturer(out error);
        if (biosManufacturer is null) return null;

        var biosSerialNumber = hardwareInfoProvider.GetBIOSSerialNumber(out error);
        if (biosSerialNumber is null) return null;

        var volumeSerialNumber = hardwareInfoProvider.GetVolumeSerialNumber(out error);
        if (volumeSerialNumber is null) return null;

        var videoControllerDeviceId = hardwareInfoProvider.GetVideoControllerDeviceId(out error);
        if (videoControllerDeviceId is null) return null;

        var processorManufacturer = hardwareInfoProvider.GetProcessorManufacturer(out error);
        if (processorManufacturer is null) return null;

        var processorId = hardwareInfoProvider.GetProcessorId(out error);
        if (processorId is null) return null;

        var processorName = hardwareInfoProvider.GetProcessorName(out error);
        if (processorName is null) return null;

        sb.Append(baseBoardManufacturer);
        sb.Append(';');
        sb.Append(baseBoardSerialNumber);
        sb.Append(';');
        sb.Append(biosManufacturer);
        sb.Append(';');
        sb.Append(biosSerialNumber);
        sb.Append(';');
        sb.Append(volumeSerialNumber);
        sb.Append(';');
        sb.Append(videoControllerDeviceId);
        sb.Append(';');
        sb.Append(processorManufacturer);
        sb.Append(';');
        sb.Append(processorId);
        sb.Append(';');
        sb.Append(processorName);
        sb.Append(';');

        return sb.ToString();
    }
}
