using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.RegnalIdentity;
using RegnalHome.Server.Data;

namespace RegnalHome.Server.ClientFactories
{
    public class ThermClientFactory : GrpcClientFactory<Therm.Grpc.Therm.ThermClient>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        protected override string ClientId => Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientId;
        protected override string ClientSecret => Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientSecret;

        protected override string IdentityServerAddress => $"https://{Common.Configuration.Server.Address}:{Configuration.IdentityServer.Port}";

        public ThermClientFactory(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public override async Task<Therm.Grpc.Therm.ThermClient> CreateClient(string address)
        {
            var channel = GrpcChannel.ForAddress(address, GetGrpcChannelOptions());
            return await Task.FromResult(new Therm.Grpc.Therm.ThermClient(channel));
        }

        public override async Task<IEnumerable<(string Address, Therm.Grpc.Therm.ThermClient Client)>> CreateClients()
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            var sensors = await dbContext.GetThermSensors();
            var result = new List<(string Address, Therm.Grpc.Therm.ThermClient Client)>();

            foreach (var sensor in sensors)
            {
                var client = await CreateClient(sensor.Address);
                result.Add((sensor.Address, client));
            }

            return result;
        }
    }
}
