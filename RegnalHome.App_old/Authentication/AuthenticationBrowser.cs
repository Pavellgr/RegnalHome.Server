using IdentityModel.OidcClient.Browser;
using IBrowser = IdentityModel.OidcClient.Browser.IBrowser;

namespace RegnalHome.App.Authentication;

internal class AuthenticationBrowser : IBrowser
{
    public Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = new())
    {
        throw new NotImplementedException();
    }
}