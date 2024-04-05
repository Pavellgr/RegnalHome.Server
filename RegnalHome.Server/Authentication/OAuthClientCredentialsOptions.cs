using Microsoft.AspNetCore.Authentication;

namespace RegnalHome.Server.Authentication
{
    public class OAuthClientCredentialsOptions : AuthenticationSchemeOptions
    {
        public string TokenUrl { get; set; }
    }
}
