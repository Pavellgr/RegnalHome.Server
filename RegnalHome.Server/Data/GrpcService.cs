using Grpc.Core;
using Grpc.Net.Client;
using IdentityServer4.Models;
using Newtonsoft.Json;
using RegnalHome.Common.RegnalIdentity;
using RegnalHome.Grpc;

namespace RegnalHome.Server.Data;

public class GrpcService
{
    public async Task<T> CallGrpc<T>(Func<Therm.ThermClient, Metadata, Task<T>> grpcCall, CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7171");
        var client = new Therm.ThermClient(channel);

        var token = await GetToken(cancellationToken);
        var headers = new Metadata
        {
            {"Authorization", $"Bearer {token}"}
        };

        try
        {
            return await grpcCall(client, headers);
        }
        catch (RpcException e)
        {
            if (e.StatusCode == StatusCode.Unauthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            return default;
        }
    }
    private async Task<string> GetToken(CancellationToken cancellationToken)
    {

        string baseAddress = $"https://{Common.Configuration.Server.IPAddress}:{Configuration.IdentityServer.Port}";
        using var client = new HttpClient
        {
            BaseAddress = new Uri(baseAddress)
        };
        var form = new Dictionary<string, string>
        {
            {"grant_type", GrantType.ClientCredentials},
            {"client_id", Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientId},
            {"client_secret", Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientSecret},
        };
        var response = client.PostAsync("/connect/token", new FormUrlEncodedContent(form), cancellationToken).Result;
        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseJson = (dynamic)JsonConvert.DeserializeObject(responseString);

        cancellationToken.ThrowIfCancellationRequested();

        return (string)responseJson["access_token"];
    }
}