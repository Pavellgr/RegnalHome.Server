using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using RegnalHome.Common.Models;
using RegnalHome.Grpc;

namespace RegnalHome.GrpcSim.Services
{
    [Authorize]
    public class ThermService : Therm.ThermBase
    {
        private readonly ILogger<ThermService> _logger;

        ThermSensor Sensor = new ThermSensor
        {
            Id = Guid.NewGuid(),
            Temperature = 5
        };

        public ThermService(ILogger<ThermService> logger)
        {
            _logger = logger;
        }

        public override Task<ThermSensorReply> GetThermSensor(EmptyRequest request, ServerCallContext context)
        {
            return Task.FromResult( new ThermSensorReply
            {
                Id = Sensor.Id.ToString(),
                Temperature = Sensor.Temperature ?? 0,
                TargetTemperature = Sensor.TargetTemperature ?? 0
            });
        }

        public override Task<BooleanReply> SetThermSensor(ThermSensorRequest request, ServerCallContext context)
        {
            Sensor.TargetTemperature = request.TargetTemperature;

            return Task.FromResult(new BooleanReply
            {
                Value = true
            });
        }
    }
}