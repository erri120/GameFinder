using JetBrains.Annotations;

namespace GameFinder.StoreHandlers.EADesktop.Crypto;

/// <summary>
/// Represents a Hardware Info Provider.
/// </summary>
[PublicAPI]
public interface IHardwareInfoProvider
{
    /// <summary>
    /// Returns the Serial Number of the Volume that contains the Windows folder.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetVolumeSerialNumber();

    /// <summary>
    /// Returns the Manufacturer of the Motherboard.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetBaseBoardManufacturer();

    /// <summary>
    /// Returns the Serial Number of the Motherboard.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetBaseBoardSerialNumber();

    /// <summary>
    /// Returns the Manufacturer of the BIOS.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetBIOSManufacturer();

    /// <summary>
    /// Returns of Serial Number of the BIOS.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetBIOSSerialNumber();

    /// <summary>
    /// Returns the PNPDeviceId of the GPU.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetVideoControllerDeviceId();

    /// <summary>
    /// Returns the Manufacturer of the CPU.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetProcessorManufacturer();

    /// <summary>
    /// Returns the ProcessorId of the CPU.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetProcessorId();

    /// <summary>
    /// Returns the name of the CPU.
    /// </summary>
    /// <exception cref="HardwareInfoProviderException"></exception>
    /// <returns></returns>
    string GetProcessorName();
}
