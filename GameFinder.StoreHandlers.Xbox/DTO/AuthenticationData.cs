using System;
using JetBrains.Annotations;
#pragma warning disable 1591

namespace GameFinder.StoreHandlers.Xbox.DTO
{
    [PublicAPI]
    public class AuthenticationData
    {
        public string? AccessToken { get; set; }
        
        public string? RefreshToken { get; set; }
        
        public string? ExpiresIn { get; set; }
        
        public DateTime CreationDate { get; set; }
        
        public string? UserId { get; set; }
        
        public string? TokenType { get; set; }
    }
}
