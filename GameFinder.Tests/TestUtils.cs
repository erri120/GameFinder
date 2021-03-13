using System;

namespace GameFinder.Tests
{
    public static class TestUtils
    {
        public static readonly bool IsCI;
        
        static TestUtils()
        {
            var ciEnv = Environment.GetEnvironmentVariable("CI", EnvironmentVariableTarget.Process);
            if (ciEnv == null) return;
            if (!bool.TryParse(ciEnv, out var isCI))
                IsCI = false;
            IsCI = isCI;
        }
    }
}
