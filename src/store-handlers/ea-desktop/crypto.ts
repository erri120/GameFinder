/**
 * EA Desktop file decryption utilities
 *
 * EA Desktop stores game information in an encrypted file.
 * The encryption uses AES-256-CBC with a key derived from hardware information.
 */

import { createHash, createDecipheriv } from 'node:crypto';
import jsSha3 from 'js-sha3';
import { Result, ok, err } from 'neverthrow';

const { sha3_256 } = jsSha3;
import type { GameFinderError } from '../../common/index.js';

/**
 * Hardware information required to generate the decryption key
 */
export interface HardwareInfo {
  baseBoardManufacturer: string;
  baseBoardSerialNumber: string;
  biosManufacturer: string;
  biosSerialNumber: string;
  volumeSerialNumber: string;
  videoControllerDeviceId: string;
  processorManufacturer: string;
  processorId: string;
  processorName: string;
}

/**
 * The fixed IV used for AES decryption
 * Derived from SHA3-256("allUsersGenericIdIS")
 */
const AES_IV = Buffer.from([
  0x84, 0xef, 0xc4, 0xb8, 0x36, 0x11, 0x9c, 0x20, 0x41, 0x93, 0x98, 0xc3, 0xf3,
  0xf2, 0xbc, 0xef,
]);

/**
 * The hash of "allUsersGenericId" used in the data path
 */
export const DATA_DIR_HASH =
  '530c11479fe252fc5aabc24935b9776d4900eb3ba58fdc271e0d6229413ad40e';

/**
 * Calculate SHA1 hash of a string (ASCII encoding)
 */
export function sha1(input: string): string {
  return createHash('sha1').update(input, 'ascii').digest('hex');
}

/**
 * Calculate SHA3-256 hash of a string (ASCII encoding) and return as bytes
 */
export function sha3_256Bytes(input: string): Buffer {
  return Buffer.from(sha3_256.arrayBuffer(input));
}

/**
 * Generate the hardware string from hardware info
 */
export function generateHardwareString(info: HardwareInfo): string {
  return [
    info.baseBoardManufacturer,
    info.baseBoardSerialNumber,
    info.biosManufacturer,
    info.biosSerialNumber,
    info.volumeSerialNumber,
    info.videoControllerDeviceId,
    info.processorManufacturer,
    info.processorId,
    info.processorName,
  ].join(';') + ';';
}

/**
 * Generate the decryption key from hardware info
 */
export function generateDecryptionKey(info: HardwareInfo): Buffer {
  const hardwareString = generateHardwareString(info);
  const hardwareSha1 = sha1(hardwareString);
  const hashInput = 'allUsersGenericId' + 'IS' + hardwareSha1;
  return sha3_256Bytes(hashInput);
}

/**
 * Decrypt the EA Desktop IS file
 */
export function decryptISFile(
  encryptedData: Buffer,
  decryptionKey: Buffer
): Result<string, GameFinderError> {
  try {
    // Skip first 64 bytes (hash/header)
    const ciphertext = encryptedData.subarray(64);

    // Decrypt using AES-256-CBC
    const decipher = createDecipheriv('aes-256-cbc', decryptionKey, AES_IV);
    const decrypted = Buffer.concat([
      decipher.update(ciphertext),
      decipher.final(),
    ]);

    return ok(decrypted.toString('utf8'));
  } catch (error) {
    return err({
      code: 'EA_DECRYPTION_ERROR',
      message: 'Failed to decrypt EA Desktop IS file',
      cause: error instanceof Error ? error : new Error(String(error)),
    });
  }
}
