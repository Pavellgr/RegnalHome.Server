using IdentityModel.OidcClient;

namespace RegnalHome.App.Authentication
{
    public class OAuth2Client
    {
        private readonly OidcClient oidcClient;

        public OAuth2Client(OAuth2ClientOptions options)
        {
            oidcClient = new OidcClient(new OidcClientOptions
            {
                Authority = options.Authority,
                ClientId = options.ClientId,
                Scope = options.Scope,
                RedirectUri = options.RedirectUri,
                Browser = options.Browser
            });
        }

        public IdentityModel.OidcClient.Browser.IBrowser Browser
        {
            get
            {
                return oidcClient.Options.Browser;
            }
            set
            {
                oidcClient.Options.Browser = value;
            }
        }

        public async Task<LoginResult> LoginAsync()
        {
            return await oidcClient.LoginAsync();
        }
    }
}
