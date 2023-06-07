using IBrowser = IdentityModel.OidcClient.Browser.IBrowser;

namespace RegnalHome.App.Authentication;

public class AuthenticationClientOptions
{
    public AuthenticationClientOptions()
    {
        Scope = "openid";
        RedirectUri = "myapp://callback";
        Browser = new WebBrowserAuthenticator();
    }

    public string Domain { get; set; }

    public string ClientId { get; set; }

    public string RedirectUri { get; set; }

    public string Scope { get; set; }

    public IBrowser Browser { get; set; }
}