using System;
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
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetVolumeSerialNumber(out string? error);

    /// <summary>
    /// Returns the Manufacturer of the Motherboard.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetBaseBoardManufacturer(out string? error);

    /// <summary>
    /// Returns the Serial Number of the Motherboard.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetBaseBoardSerialNumber(out string? error);

    /// <summary>
    /// Returns the Manufacturer of the BIOS.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetBIOSManufacturer(out string? error);

    /// <summary>
    /// Returns of Serial Number of the BIOS.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetBIOSSerialNumber(out string? error);

    /// <summary>
    /// Returns the PNPDeviceId of the GPU.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetVideoControllerDeviceId(out string? error);

    /// <summary>
    /// Returns the Manufacturer of the CPU.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetProcessorManufacturer(out string? error);

    /// <summary>
    /// Returns the ProcessorId of the CPU.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetProcessorId(out string? error);

    /// <summary>
    /// Returns the name of the CPU.
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    string? GetProcessorName(out string? error);
}
