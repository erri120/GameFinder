using System;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace GameFinder.RegistryUtils
{
    internal static class RegistryHelper
    {
        private static object? GetObjectFromRegistry(RegistryKey key, string valueName, ILogger logger)
        {
            var value = key.GetValue(valueName);
            if (value == null)
                logger.LogWarning("RegistryKey {Key} does not have a value {ValueName}", key, valueName);
            return value;
        }
        
        internal static long? GetQWordValueFromRegistry(RegistryKey key, string valueName, ILogger logger)
        {
            var res = GetObjectFromRegistry(key, valueName, logger);
            return (long?)res;
        }
        
        internal static string? GetStringValueFromRegistry(RegistryKey key, string valueName, ILogger logger)
        {
            var res = GetObjectFromRegistry(key, valueName, logger);
            if (res == null) return null;
            
            var sValue = res.ToString() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(sValue)) return sValue;
            
            logger.LogError("Value {ValueName} in RegistryKey {Key} is null or white space", valueName, key);
            return null;

        }
        
        internal static string? GetNullableStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            var sValue = value?.ToString();
            return sValue;
        }
    }
}
