using IdentityModel.Client;
using IdentityModel.Jwk;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IBrowser = IdentityModel.OidcClient.Browser.IBrowser;

namespace RegnalHome.App.Authentication;

public class AuthenticationClient
{
  private readonly OidcClient oidcClient;

  public AuthenticationClient(AuthenticationClientOptions options)
  {
    oidcClient = new OidcClient(new OidcClientOptions
    {
      Authority = options.Domain,
      ClientId = options.ClientId,
      Scope = options.Scope,
      RedirectUri = options.RedirectUri,
      Browser = options.Browser,
      BrowserTimeout = TimeSpan.FromSeconds(5),
      ProviderInformation = new ProviderInformation
      {
        AuthorizeEndpoint = $"{options.Domain}/connect/authorize",
        EndSessionEndpoint = $"{options.Domain}/connect/endsession",
        IssuerName = options.Domain,
        TokenEndPointAuthenticationMethods = new[] {"client_secret_basic", "client_secret_post"},
        TokenEndpoint = $"{options.Domain}/connect/token",
        UserInfoEndpoint = $"{options.Domain}/connect/userinfo",
        KeySet = new JsonWebKeySet()
      }
    });
  }

  public IBrowser Browser
  {
    get => oidcClient.Options.Browser;
    set => oidcClient.Options.Browser = value;
  }

  public async Task<LoginResult> LoginAsync()
  {
    return await oidcClient.LoginAsync();
  }

  public async Task<BrowserResult> LogoutAsync()
  {
    var logoutParameters = new Dictionary<string, string>
    {
      {"client_id", oidcClient.Options.ClientId},
      {"returnTo", oidcClient.Options.RedirectUri}
    };

    var logoutRequest = new LogoutRequest();
    var endSessionUrl = new RequestUrl($"{oidcClient.Options.Authority}/v2/logout")
      .Create(new Parameters(logoutParameters));
    var browserOptions = new BrowserOptions(endSessionUrl, oidcClient.Options.RedirectUri)
    {
      Timeout = TimeSpan.FromSeconds(logoutRequest.BrowserTimeout),
      DisplayMode = logoutRequest.BrowserDisplayMode
    };

    var browserResult = await oidcClient.Options.Browser.InvokeAsync(browserOptions);

    return browserResult;
  }
}