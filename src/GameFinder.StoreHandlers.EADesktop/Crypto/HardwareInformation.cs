using System.Text;

namespace GameFinder.StoreHandlers.EADesktop.Crypto;

internal static class HardwareInformation
{
    public static string GenerateHardwareString(IHardwareInfoProvider hardwareInfoProvider)
    {
        var sb = new StringBuilder();

        var baseBoardManufacturer = hardwareInfoProvider.GetBaseBoardManufacturer();
        var baseBoardSerialNumber = hardwareInfoProvider.GetBaseBoardSerialNumber();
        var biosManufacturer = hardwareInfoProvider.GetBIOSManufacturer();
        var biosSerialNumber = hardwareInfoProvider.GetBIOSSerialNumber();
        var volumeSerialNumber = hardwareInfoProvider.GetVolumeSerialNumber();
        var videoControllerDeviceId = hardwareInfoProvider.GetVideoControllerDeviceId();
        var processorManufacturer = hardwareInfoProvider.GetProcessorManufacturer();
        var processorId = hardwareInfoProvider.GetProcessorId();
        var processorName = hardwareInfoProvider.GetProcessorName();

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
