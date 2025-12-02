/**
 * Hardware information collection for EA Desktop decryption
 * Uses WMI queries on Windows to gather hardware identifiers
 */

import { exec } from 'node:child_process';
import { promisify } from 'node:util';
import { platform } from 'node:os';
import { Result, ok, err } from 'neverthrow';
import type { GameFinderError } from '../../common/index.js';
import type { HardwareInfo } from './crypto.js';

const execAsync = promisify(exec);

/**
 * Run a WMI query and extract a property value
 */
async function wmiQuery(
  wmiClass: string,
  property: string
): Promise<string> {
  try {
    const { stdout } = await execAsync(
      `wmic ${wmiClass} get ${property} /value`,
      { encoding: 'utf8' }
    );

    // Parse output like "Property=Value\r\n"
    const match = new RegExp(`${property}=(.*)`, 'i').exec(stdout);
    return match?.[1]?.trim() ?? '';
  } catch {
    return '';
  }
}

/**
 * Get the C: drive volume serial number
 */
async function getVolumeSerialNumber(): Promise<string> {
  try {
    const { stdout } = await execAsync('vol C:', { encoding: 'utf8' });
    // Output like: " Volume Serial Number is XXXX-XXXX"
    const match = /Serial Number is ([A-F0-9]{4}-[A-F0-9]{4})/i.exec(stdout);
    if (match?.[1]) {
      // Convert "XXXX-XXXX" to decimal
      const hex = match[1].replace('-', '');
      return parseInt(hex, 16).toString();
    }
  } catch {
    // Fallback
  }
  return '';
}

/**
 * Collect hardware information from the system
 */
export async function collectHardwareInfo(): Promise<
  Result<HardwareInfo, GameFinderError>
> {
  if (platform() !== 'win32') {
    return err({
      code: 'EA_WINDOWS_ONLY',
      message: 'EA Desktop hardware info collection is only available on Windows',
    });
  }

  try {
    // Run all WMI queries in parallel
    const [
      baseBoardManufacturer,
      baseBoardSerialNumber,
      biosManufacturer,
      biosSerialNumber,
      volumeSerialNumber,
      videoControllerDeviceId,
      processorManufacturer,
      processorId,
      processorName,
    ] = await Promise.all([
      wmiQuery('baseboard', 'Manufacturer'),
      wmiQuery('baseboard', 'SerialNumber'),
      wmiQuery('bios', 'Manufacturer'),
      wmiQuery('bios', 'SerialNumber'),
      getVolumeSerialNumber(),
      wmiQuery('path win32_VideoController', 'PNPDeviceID'),
      wmiQuery('cpu', 'Manufacturer'),
      wmiQuery('cpu', 'ProcessorId'),
      wmiQuery('cpu', 'Name'),
    ]);

    return ok({
      baseBoardManufacturer,
      baseBoardSerialNumber,
      biosManufacturer,
      biosSerialNumber,
      volumeSerialNumber,
      videoControllerDeviceId,
      processorManufacturer,
      processorId,
      processorName,
    });
  } catch (error) {
    return err({
      code: 'EA_HARDWARE_INFO_ERROR',
      message: 'Failed to collect hardware information',
      cause: error instanceof Error ? error : new Error(String(error)),
    });
  }
}
