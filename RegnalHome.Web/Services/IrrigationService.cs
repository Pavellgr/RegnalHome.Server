using RegnalHome.Common.Models;
using RegnalHome.Server.Extensions;

namespace RegnalHome.Web.Services
{
    public class IrrigationService
    {
        private Irrigation.Grpc.Irrigation.IrrigationClient _client { get; init; }

        public IrrigationService(Irrigation.Grpc.Irrigation.IrrigationClient client)
        {
            _client = client;
        }

        public async Task<IrrigationModule[]> GetModules()
        {
            var modules = await _client.GetIrrigationModulesAsync(new Common.Grpc.Empty());
            return modules.Modules.Select(p => p.FromGrpc()).ToArray();
        }

        public async Task<IrrigationModule> GetModule(string id)
        {
            var module = await _client.GetIrrigationModuleAsync(new Common.Grpc.Id
            {
                Id_ = id
            });

            return module.FromGrpc();
        }

        public async Task UpdateModule(IrrigationModule module)
        {
            await _client.UpdateIrrigationModuleAsync(module.ToGrpc());
        }
    }
}
