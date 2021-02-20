using Microsoft.Win32;

namespace GameFinder.RegistryUtils
{
    internal static class RegistryHelper
    {
        internal static string GetStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            if (value == null)
                throw new RegistryValueNotExistException($"RegistryKey {key} does not have value {valueName}!", key);

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
