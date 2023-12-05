namespace RegnalHome.App.Authentication
{
    public class OAuth2ClientOptions
    {
        public OAuth2ClientOptions()
        {
            Scope = "openid";
            RedirectUri = "myapp://callback";
            Browser = new WebBrowserAuthenticator();
        }

        public string Authority { get; set; }

        public string ClientId { get; set; }

        public string RedirectUri { get; set; }

        public string Scope { get; set; }

        public IdentityModel.OidcClient.Browser.IBrowser Browser { get; set; }
    }
}
