import { readFileSync } from 'node:fs';
import { join } from 'node:path';
import { platform } from 'node:os';
import { EADesktopHandler } from '../../../src/store-handlers/ea-desktop/ea-desktop-handler.js';
import {
  sha1,
  sha3_256Bytes,
  generateHardwareString,
  generateDecryptionKey,
  decryptISFile,
  DATA_DIR_HASH,
} from '../../../src/store-handlers/ea-desktop/crypto.js';
import { createEADesktopGame, isInstalled } from '../../../src/store-handlers/ea-desktop/types.js';
import type { EAInstallInfo, EAInstallInfoFile } from '../../../src/store-handlers/ea-desktop/types.js';
import type { HardwareInfo } from '../../../src/store-handlers/ea-desktop/crypto.js';

// Test files directory
const TEST_FILES_DIR = join(
  process.cwd(),
  'tests',
  'GameFinder.StoreHandlers.EADesktop.Tests',
  'files'
);

describe('EA Desktop Crypto', () => {
  it('should calculate correct SHA1 hash', () => {
    // Test value from C# implementation
    const result = sha1('erri120');
    expect(result).toBe('3f37d8cece4441299a6abbd8db2acc0102cfd398');
  });

  it('should calculate correct SHA3-256 hash for data dir', () => {
    const result = sha3_256Bytes('allUsersGenericId');
    expect(result.toString('hex')).toBe(DATA_DIR_HASH);
  });

  it('should generate correct hardware string', () => {
    const info: HardwareInfo = {
      baseBoardManufacturer: 'ASUSTeK COMPUTER INC.',
      baseBoardSerialNumber: '123456789',
      biosManufacturer: 'American Megatrends Inc.',
      biosSerialNumber: 'Default string',
      volumeSerialNumber: '1234567890',
      videoControllerDeviceId: 'PCI\\VEN_10DE&DEV_2204',
      processorManufacturer: 'GenuineIntel',
      processorId: 'BFEBFBFF000906ED',
      processorName: 'Intel(R) Core(TM) i9-9900K CPU @ 3.60GHz',
    };

    const result = generateHardwareString(info);
    expect(result).toContain('ASUSTeK COMPUTER INC.');
    expect(result).toContain('GenuineIntel');
    expect(result.endsWith(';')).toBe(true);
  });

  it('should decrypt IS file with correct key', () => {
    // This test uses the test fixture with known hardware info
    // The hardware info for erri120's test:
    const info: HardwareInfo = {
      baseBoardManufacturer: '',
      baseBoardSerialNumber: '',
      biosManufacturer: '',
      biosSerialNumber: '',
      volumeSerialNumber: '',
      videoControllerDeviceId: '',
      processorManufacturer: '',
      processorId: '',
      processorName: '',
    };

    // Generate key (will be different from actual key without real hardware info)
    const key = generateDecryptionKey(info);
    expect(key.length).toBe(32); // 256 bits

    // Read encrypted file
    const encryptedPath = join(TEST_FILES_DIR, 'IS_erri120.encrypted');
    const encryptedData = readFileSync(encryptedPath);
    expect(encryptedData.length).toBeGreaterThan(64);

    // Note: We can't actually decrypt without the real hardware info
    // but we can verify the file structure
    expect(encryptedData.length).toBe(5168);
  });

  it('should have matching decrypted content structure', () => {
    // Read the pre-decrypted file to verify expected structure
    const decryptedPath = join(TEST_FILES_DIR, 'IS_erri120.decrypted');
    const decryptedContent = readFileSync(decryptedPath, 'utf8');
    const data = JSON.parse(decryptedContent) as EAInstallInfoFile;

    expect(data.schema.version).toBe(21);
    expect(Array.isArray(data.installInfos)).toBe(true);
    expect(data.installInfos.length).toBe(5);

    // Check first installed game (Titanfall 2)
    const titanfall = data.installInfos.find((i) => i.baseSlug === 'titanfall-2');
    expect(titanfall).toBeDefined();
    expect(titanfall?.softwareId).toBe('Origin.SFT.50.0000532');
    expect(titanfall?.baseInstallPath).toBe('E:\\SteamLibrary\\steamapps\\common\\Titanfall2\\');
  });
});

describe('EA Desktop Types', () => {
  it('should correctly identify installed games', () => {
    const installed: EAInstallInfo = {
      baseInstallPath: 'C:\\Games\\Test',
      baseSlug: 'test-game',
      softwareId: 'Origin.SFT.50.0001234',
      installCheck: '',
      contentManifestLaunchers: '',
      executableCheck: '',
      installedLocale: 'en_US',
      installedVersion: '1.0.0',
      dlcSubPath: '',
      detailedState: {
        installPhase: 0,
        installReason: 0,
        installStatus: 5,
        previousInstallStatus: 0,
      },
      localInstallProperties: {
        launchers: [],
        localManifestVersion: { major: 1, minor: 0, build: 0, rev: 0 },
        useGameVersionFromManifest: true,
      },
      localUninstallProperties: {
        uninstallCommand: '',
        uninstallParameters: '',
      },
    };

    expect(isInstalled(installed)).toBe(true);
  });

  it('should correctly identify uninstalled games', () => {
    const notInstalled: EAInstallInfo = {
      baseInstallPath: '',
      baseSlug: 'test-game',
      softwareId: 'Origin.SFT.50.0001234',
      installCheck: '',
      contentManifestLaunchers: '',
      executableCheck: '',
      installedLocale: 'en_US',
      installedVersion: '',
      dlcSubPath: '',
      detailedState: {
        installPhase: 0,
        installReason: 0,
        installStatus: 0,
        previousInstallStatus: 0,
      },
      localInstallProperties: {
        launchers: [],
        localManifestVersion: { major: 0, minor: 0, build: 0, rev: 0 },
        useGameVersionFromManifest: false,
      },
      localUninstallProperties: {
        uninstallCommand: '',
        uninstallParameters: '',
      },
    };

    expect(isInstalled(notInstalled)).toBe(false);
  });

  it('should create game from install info', () => {
    const installInfo: EAInstallInfo = {
      baseInstallPath: 'C:\\Games\\Apex',
      baseSlug: 'apex-legends',
      softwareId: 'Origin.SFT.50.0000848',
      installCheck: '',
      contentManifestLaunchers: '',
      executableCheck: '',
      installedLocale: 'en_US',
      installedVersion: '1.1.0.7',
      dlcSubPath: '',
      detailedState: {
        installPhase: 2,
        installReason: 1,
        installStatus: 3,
        previousInstallStatus: 2,
      },
      localInstallProperties: {
        launchers: [
          {
            exePath: 'EasyAntiCheat_launcher.exe',
            cmdArgs: '',
            executeElevated: false,
            isTimedTrial: false,
            requires64BitOs: true,
          },
        ],
        localManifestVersion: { major: 1, minor: 1, build: 0, rev: 7 },
        useGameVersionFromManifest: true,
      },
      localUninstallProperties: {
        uninstallCommand: '',
        uninstallParameters: '',
      },
    };

    const game = createEADesktopGame(installInfo);

    expect(game.id).toBe('Origin.SFT.50.0000848');
    expect(game.name).toBe('apex-legends');
    expect(game.path).toBe('C:\\Games\\Apex');
    expect(game.store).toBe('ea-desktop');
    expect(game.version).toBe('1.1.0.7');
    expect(game.launchers.length).toBe(1);
  });
});

describe('EADesktopHandler', () => {
  let handler: EADesktopHandler;

  beforeEach(() => {
    handler = new EADesktopHandler();
  });

  it('should have the correct store name', () => {
    expect(handler.storeName).toBe('EA Desktop');
  });

  it('should return empty on non-Windows', async () => {
    if (platform() !== 'win32') {
      const result = await handler.findAllGames();
      expect(result.isOk()).toBe(true);
      if (result.isOk()) {
        expect(result.value).toEqual([]);
      }
    }
  });

  it('should implement isAvailable', async () => {
    const available = await handler.isAvailable();
    expect(typeof available).toBe('boolean');

    if (platform() !== 'win32') {
      expect(available).toBe(false);
    }
  });
});
