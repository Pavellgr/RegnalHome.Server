using Grpc.Net.Client;
using RegnalHome.Grpc;

namespace RegnalHome.Server.Data;

public static class GrpcService
{
    public static async Task<T> GetResponse<T>()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7171");
        var client = new Grpc.Therm.ThermClient(channel);
        var reply = await client.GetThermSensorAsync(new EmptyRequest());

        return (T)Convert.ChangeType(reply, typeof(T));
    }
}