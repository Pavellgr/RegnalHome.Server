using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RegnalHome.Server.Http.JsonConverters;
using System.Net;
using System;
using System.Text.Encodings.Web;
using System.Threading;
using RegnalHome.Server.Http.Responses;
using System.Security.Claims;

namespace RegnalHome.Server.Authentication
{
    public class OAuthClientCredentialsHandler : AuthenticationHandler<OAuthClientCredentialsOptions>
    {
        public OAuthClientCredentialsHandler(IOptionsMonitor<OAuthClientCredentialsOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        public OAuthClientCredentialsHandler(IOptionsMonitor<OAuthClientCredentialsOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("client_id", out var clientId))
            {
                return AuthenticateResult.Fail("Missing client_id");
            }

            if (!Request.Headers.TryGetValue("client_secret", out var clientSecret))
            {
                return AuthenticateResult.Fail("Missing client_secret");
            }

            if (!Request.Headers.TryGetValue("scope", out var scope))
            {
                return AuthenticateResult.Fail("Missing scope");
            }

            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, Options.TokenUrl);
            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["scope"] = scope
            });

            using var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new EgdDataResponseConverter());

                var responseObj = JsonConvert.DeserializeObject<TokenResponse>(reader.ReadToEnd(), settings);

                if (responseObj != null &&
                    (!string.IsNullOrWhiteSpace(responseObj.access_token)))
                {
                    return AuthenticateResult.Success(new AuthenticationTicket(
                                                        new ClaimsPrincipal([
                                                                new ClaimsIdentity([
                                                                        new Claim("client_id", clientId),
                                                                        new Claim("access_token", responseObj.access_token),
                                                                        new Claim("scope", scope)
                                                                    ], Scheme.Name)
                                                            ]),
                                                      Scheme.Name));
                }
            }

            return AuthenticateResult.Fail("Not implemented");
        }
    }
}
