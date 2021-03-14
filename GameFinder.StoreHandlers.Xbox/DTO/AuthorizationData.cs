using System;
using System.Collections.Generic;
using JetBrains.Annotations;
// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace GameFinder.StoreHandlers.Xbox.DTO
{
    [PublicAPI]
    public class AuthorizationData
    {
        public string? Token { get; set; }
        public DateTime IssueInstant { get; set; }
        public DateTime NotAfter { get; set; }

        public DisplayClaimsData? DisplayClaims { get; set; }
        
        [PublicAPI]
        public class DisplayClaimsData
        {
            public List<XuiData>? xui { get; set; }
            
            [PublicAPI]
            public class XuiData
            {
                public string? uhs { get; set; }
                public string? usr { get; set; }
                public string? utr { get; set; }
                public string? prv { get; set; }
                public string? xid { get; set; }
                public string? gtg { get; set; }
            }
        }
    }
}
