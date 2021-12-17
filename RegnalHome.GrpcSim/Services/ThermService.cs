using Grpc.Core;
using RegnalHome.Common.Models;
using RegnalHome.Grpc;

namespace RegnalHome.GrpcSim.Services
{
    public class ThermService : Therm.ThermBase
    {
        private readonly ILogger<ThermService> _logger;
        public ThermService(ILogger<ThermService> logger)
        {
            _logger = logger;
        }

        public override Task<ThermSensorReply> GetThermSensor(EmptyRequest request, ServerCallContext context)
        {
            var sensor = new ThermSensor
            {
                Id = Guid.NewGuid(),
                Temperature = 5
            };

            return Task.FromResult(new ThermSensorReply
            {
                Id = sensor.Id.ToString(),
                Temperature = sensor.Temperature ?? 0
            });
        }
    }
}