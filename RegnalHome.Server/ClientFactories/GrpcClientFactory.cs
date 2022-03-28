using Grpc.Core;
using Grpc.Net.Client;
using IdentityServer4.Models;
using Newtonsoft.Json;

namespace RegnalHome.Server.ClientFactories
{
    public abstract class GrpcClientFactory<T> where T : ClientBase
    {
        protected abstract string ClientId { get; }

        protected abstract string ClientSecret { get; }

        protected abstract string IdentityServerAddress { get; }

        public abstract Task<T> CreateClient(string address);

        public abstract Task<IEnumerable<(string Address, T Client)>> CreateClients();

        protected GrpcChannelOptions GetGrpcChannelOptions()
        {
            var options = new GrpcChannelOptions();

            var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
            {
                var token = await GetToken(CancellationToken.None);

                if (!string.IsNullOrEmpty(token))
                {
                    metadata.Add("Authorization", $"Bearer {token}");
                }
            });

            options.Credentials = ChannelCredentials.Create(new SslCredentials(), credentials);

            return options;
        }

        async Task<string> GetToken(CancellationToken cancellationToken)
        {

            string baseAddress = IdentityServerAddress;
            using var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };
            var form = new Dictionary<string, string>
            {
                {"grant_type", GrantType.ClientCredentials},
                {"client_id", ClientId},
                {"client_secret", ClientSecret}
            };
            var response = client.PostAsync("/connect/token", new FormUrlEncodedContent(form), cancellationToken).Result;
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseJson = (dynamic)JsonConvert.DeserializeObject(responseString);

            cancellationToken.ThrowIfCancellationRequested();

            return (string)responseJson["access_token"];
        }
    }
}
