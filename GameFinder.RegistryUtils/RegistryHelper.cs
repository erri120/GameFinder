using Microsoft.Win32;
using static GameFinder.ResultUtils;

namespace GameFinder.RegistryUtils
{
    internal static class RegistryHelper
    {
        private static Result<object> GetObjectFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            return value == null 
                ? Err<object>($"RegistryKey {key} does not have value {valueName}!") 
                : Ok(value);
        }
        
        internal static Result<long> GetQWordValueFromRegistry(RegistryKey key, string valueName)
        {
            var res = GetObjectFromRegistry(key, valueName);
            return res.HasErrors 
                ? Err<long, object>(res) 
                : Ok((long) res.Value);
        }
        
        internal static Result<string> GetStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var objectRes = GetObjectFromRegistry(key, valueName);
            if (objectRes.HasErrors)
            {
                return Errs<string>(objectRes.Errors);
            }
            
            var sValue = objectRes.Value.ToString() ?? string.Empty;
            
            return string.IsNullOrEmpty(sValue) 
                ? Err<string>($"Value {valueName} in RegistryKey {key} is null or empty!") 
                : Ok(sValue);
        }
        
        internal static string? GetNullableStringValueFromRegistry(RegistryKey key, string valueName)
        {
            var value = key.GetValue(valueName);
            var sValue = value?.ToString();
            return sValue;
        }
    }
}
