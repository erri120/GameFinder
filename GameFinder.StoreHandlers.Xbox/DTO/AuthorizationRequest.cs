using System.Collections.Generic;
using JetBrains.Annotations;
#pragma warning disable 1591

namespace GameFinder.StoreHandlers.Xbox.DTO
{
    [PublicAPI]
    public class AuthorizationRequest
    {
        public string RelyingParty { get; set; } = "http://xboxlive.com";
        public string TokenType { get; set; } = "JWT";
        public AuthorizationRequestProperties Properties { get; set; } = new AuthorizationRequestProperties();
        
        [PublicAPI]
        public class AuthorizationRequestProperties
        {
            public string SandboxId { get; set; } = "RETAIL";
            public List<string>? UserTokens { get; set; }
        }
    }
}
