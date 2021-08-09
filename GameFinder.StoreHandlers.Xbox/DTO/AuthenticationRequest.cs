using JetBrains.Annotations;
#pragma warning disable 1591

namespace GameFinder.StoreHandlers.Xbox.DTO
{
    [PublicAPI]
    public class AuthenticationRequest
    {
        public string RelyingParty { get; set; } = "http://auth.xboxlive.com";
        public string TokenType { get; set; } = "JWT";

        public AuthenticationRequestProperties Properties { get; set; } = new();
        
        [PublicAPI]
        public class AuthenticationRequestProperties
        {
            public string AuthMethod { get; set; } = "RPS";
            public string SiteMethod { get; set; } = "user.auth.xboxlive.com";
            public string? RpsTicket { get; set; }
        }
    }
}
