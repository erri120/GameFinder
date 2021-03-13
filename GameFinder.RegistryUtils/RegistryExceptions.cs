using System;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace GameFinder.RegistryUtils
{
    [PublicAPI]
    public class RegistryException : Exception
    {
        public RegistryKey? RegistryKey { get; internal set; }
        
        internal RegistryException() { }

        internal RegistryException(string message, RegistryKey? registryKey = null) : base(message)
        {
            RegistryKey = registryKey;
        }

        internal RegistryException(string message, Exception innerException, RegistryKey? registryKey = null) : base(message, innerException)
        {
            RegistryKey = registryKey;
        }
    }
    
    [PublicAPI]
    public sealed class RegistryKeyNullException : RegistryException
    {
        internal RegistryKeyNullException(string key) : base($"Unable to open RegistryKey {key}!") { }
    }

    [PublicAPI]
    public sealed class RegistryValueNotExistException : RegistryException
    {
        internal RegistryValueNotExistException(string message, RegistryKey registryKey) : base(message, registryKey) { }
    }
    
    [PublicAPI]
    public sealed class RegistryValueNullException : RegistryException
    {
        internal RegistryValueNullException(string message, RegistryKey registryKey) : base(message, registryKey) { }
    }
}
