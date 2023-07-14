using RegnalHome.Common.Models;
using RegnalHome.Web.Extensions;

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
            return modules.Modules.Select(p => p.FromGrpc<IrrigationModule>()).ToArray();
        }

        public IrrigationModule GetModule(string id)
        {
            var module = _client.GetIrrigationModule(new Common.Grpc.Id
            {
                Id_ = id
            });

            return module.FromGrpc<IrrigationModule>();
        }
    }
}
