using Microsoft.Win32;

namespace GameFinder.RegistryUtils
{
    internal static class RegistryHelper
    {
        private static object GetObjectFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            if (value == null)
                throw new RegistryValueNotExistException($"RegistryKey {key} does not have value {valueName}!", key);

            return value;
        }
        
        internal static long GetQWordValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = GetObjectFromRegistry(key, valueName);
            return (long) value;
        }
        
        internal static string GetStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = GetObjectFromRegistry(key, valueName);
            var sValue = value.ToString() ?? string.Empty;
            if (string.IsNullOrEmpty(sValue))
                throw new RegistryValueNullException($"Value {valueName} in RegistryKey {key} is null or empty!", key);

            return sValue;
        }
        
        internal static string? GetNullableStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            var sValue = value?.ToString();
            return sValue;
        }
    }
}
