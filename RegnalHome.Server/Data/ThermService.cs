using RegnalHome.Common.Dtos;
using RegnalHome.Common.Enums;
using RegnalHome.Common.Models;
using RegnalHome.Grpc;
using RegnalHome.Server.Pages;

namespace RegnalHome.Server.Data
{
    public class ThermService
    {
        private readonly ThermSensor[] sensors = new[]
        {
            new ThermSensor
            {
                Id = Guid.NewGuid(),
                Name = "First",
                ConnectionState = ConnectionState.Online,
                Temperature = 5
            },
            new ThermSensor
            {
                Id = Guid.NewGuid(),
                Name = "Second",
                ConnectionState = ConnectionState.Unknown
            },
            new ThermSensor
            {
                Id = Guid.NewGuid(),
                Name = "Third",
                ConnectionState = ConnectionState.Offline
            }
        };

        public async Task<ThermSensorDto[]> GetThermSensors()
        {
            return await Task.FromResult(sensors);
        }

        public async Task<ThermSensorDto?> GetThermSensor(string? id)
        {
            if (id != null &&
                Guid.TryParse(id, out var guid))
            {
                var sensorBase = await Task.FromResult(sensors.FirstOrDefault(p => p.Id == guid));
                
                if (sensorBase != null)
                {
                    var sensor = new ThermSensor
                    {
                        Id = guid,
                        Name = sensorBase.Name,
                        ConnectionState = ConnectionState.Offline
                    };

                    var sensorData = await GrpcService.GetResponse<ThermSensorReply>();

                    sensor.ConnectionState = ConnectionState.Online;
                    sensor.Temperature = sensorData.Temperature;
                    
                    return sensor;
                }
            }

            return null;
        }

        public async Task<ThermSensor?> ReloadThermSensor(string? id)
        {
            Thread.Sleep(2000);

            if (id != null &&
                Guid.TryParse(id, out var guid))
            {
                var sensor = await Task.FromResult(sensors.FirstOrDefault(p => p.Id == guid));
                if (sensor != null)
                {
                    sensor.ConnectionState = ConnectionState.Online;
                    sensor.Temperature = 10;
                }

                return sensor;
            }

            return null;
        }
    }
}
