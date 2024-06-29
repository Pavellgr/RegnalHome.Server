using Newtonsoft.Json;
using RegnalHome.Server.Http.Endpoints;
using RegnalHome.Server.Http.JsonConverters;
using RegnalHome.Server.Http.Options;
using RegnalHome.Server.Http.Responses;
using RegnalHome.Server.Http.Settings;
using System.Net;
using System.Net.Http.Headers;

namespace RegnalHome.Server.Http.HttpClients
{
    public abstract class OAuthHttpClient<TSettings, TOptions, TEndpoints> where TSettings : ITokenSettings
                                                                           where TOptions : IOAuthOptions
                                                                           where TEndpoints : IOAuthEndpoints
    {
        private readonly IHttpClientFactory _httpClientFactory;
        protected readonly TSettings _settings;
        protected readonly TOptions _options;
        protected readonly TEndpoints _endpoints;
        private readonly ILogger<OAuthHttpClient<TSettings, TOptions, TEndpoints>> _logger;

        public OAuthHttpClient(IHttpClientFactory httpClientFactory,
            TSettings settings,
            TOptions options,
            TEndpoints endpoints,
            ILogger<OAuthHttpClient<TSettings, TOptions, TEndpoints>> logger)
        {
            _httpClientFactory = httpClientFactory;
            _settings = settings;
            _options = options;
            _endpoints = endpoints;
            _logger = logger;
        }

        public async Task<TokenResponse> Authenticate(CancellationToken cancellationToken)
        {
            var response = await Post(_endpoints.TokenEndpoint,
                                      new FormUrlEncodedContent(new Dictionary<string, string>
                                                                {
                                                                    { "grant_type", "client_credentials" },
                                                                    { "client_id", _options.ClientId },
                                                                    { "client_secret", _options.ClientSecret },
                                                                    { "scope", _options.Scope }
                                                                }),
                                      isIdentityCall: true,
                                      responseChecks: (TokenResponse response) => response != null && !string.IsNullOrWhiteSpace(response.access_token),
                                      cancellationToken: cancellationToken);

            response.CopyTo(_settings);

            return response;
        }

        protected async Task<THttpResponse> Get<THttpResponse>(string uri, bool isApiCall = false, bool isIdentityCall = false, Func<THttpResponse, bool>? responseChecks = null, CancellationToken cancellationToken = default)
        where THttpResponse : IHttpResponse
        => await Call(uri, (client, uri) => client.GetAsync(uri, cancellationToken), isApiCall, isIdentityCall, responseChecks, cancellationToken);

        protected async Task<THttpResponse> Post<THttpResponse>(string uri, HttpContent content, bool isApiCall = false, bool isIdentityCall = false, Func<THttpResponse, bool>? responseChecks = null, CancellationToken cancellationToken = default)
            where THttpResponse : IHttpResponse
            => await Call(uri, (client, uri) => client.PostAsync(uri, content, cancellationToken), isApiCall, isIdentityCall, responseChecks, cancellationToken);

        private async Task<THttpResponse> Call<THttpResponse>(string uri, Func<HttpClient, string, Task<HttpResponseMessage>> request, bool isApiCall = false, bool isIdentityCall = false, Func<THttpResponse, bool>? responseChecks = null, CancellationToken cancellationToken = default)
            where THttpResponse : IHttpResponse
        {
            using var httpClient = CreateNewHttpClient();

            httpClient.BaseAddress = new Uri(isApiCall
                                                ? _options.ApiServiceUrl
                                                : isIdentityCall
                                                    ? _options.IdentityServiceUrl
                                                    : string.Empty);

            _logger.LogInformation($"Calling {httpClient.BaseAddress}{uri}");

            using var response = await request.Invoke(httpClient, uri);

            if (new[] {
                        HttpStatusCode.Accepted,
                        HttpStatusCode.Found,
                        HttpStatusCode.OK,
                        HttpStatusCode.Redirect
                      }
                .Contains(response.StatusCode))
            {
                using var reader = new StreamReader(await response.Content.ReadAsStreamAsync(cancellationToken));

                var settings = new JsonSerializerSettings();
                settings.Converters.Add(new EgdDataResponseConverter());

                var responseObj = JsonConvert.DeserializeObject<THttpResponse>(reader.ReadToEnd(), settings);

                if (responseObj != null &&
                    (responseChecks == null || responseChecks.Invoke(responseObj)))
                {
                    return responseObj;
                }
            }

            throw new HttpRequestException($"Request of '{uri}' failed with status {response.StatusCode}, content: {await response.Content.ReadAsStringAsync(cancellationToken)}", null, response.StatusCode);
        }

        private HttpClient CreateNewHttpClient()
        {
            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
            return client;
        }

        protected async Task<THttpResponse> WithAuthenticate<THttpResponse>(Func<CancellationToken, Task<THttpResponse>> action, CancellationToken cancellationToken) where THttpResponse : IHttpResponse
        {
            if (_settings.ExpiresAt == default ||
                _settings.ExpiresAt < DateTime.Now)
            {
                await Authenticate(cancellationToken);
            }

            THttpResponse result;
            try
            {
                result = await action(cancellationToken);
            }
            catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.Unauthorized)
            {
                await Authenticate(cancellationToken);

                result = await action(cancellationToken);
            }

            return result;
        }
    }
}
