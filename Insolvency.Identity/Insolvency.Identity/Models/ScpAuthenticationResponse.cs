using System;
using Microsoft.AspNetCore.Authentication;

namespace Insolvency.Identity.Models
{
    public class ScpAuthenticationResponse
    {
        public ScpAuthenticationResponse()
        {
            AuthenticationProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddHours(Constants.AuthenticationLifeTimeFromHours)
            };
    }

        public bool IsSuccessful { get; set; }

        public InsolvencyUser InsolvencyUser { get; set; }

        public AuthenticationProperties AuthenticationProperties { get; }

        public string ReturnUrl { get; set; }        
    }
}
