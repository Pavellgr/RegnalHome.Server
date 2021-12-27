using RegnalHome.Common;
using RegnalHome.Common.Dtos;
using RegnalHome.Common.Enums;
using RegnalHome.Common.Models;
using RegnalHome.Grpc;

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

        public async Task<ThermSensorDto[]> GetThermSensors(CancellationToken cancellationToken = default)
        {
            return await Task.WhenAll(sensors.Select(p =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                return p.GetOnlyBaseProps();
            }));
        }

        public async Task<ThermSensorDto?> GetThermSensor(string? id, CancellationToken cancellationToken = default)
        {
            if (id != null &&
                Guid.TryParse(id, out var guid))
            {
                var sensorBase = sensors.FirstOrDefault(p => p.Id == guid);

                if (sensorBase != null)
                {
                    var sensor = new ThermSensor
                    {
                        Id = guid,
                        Name = sensorBase.Name,
                        ConnectionState = ConnectionState.Offline
                    };

                    var sensorData = await new GrpcService().CallGrpc(
                        async (client, headers) => await client.GetThermSensorAsync(new EmptyRequest(), headers,
                            cancellationToken: cancellationToken), cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

                    sensor.ConnectionState = ConnectionState.Online;
                    sensor.Temperature = sensorData.Temperature;
                    sensor.TargetTemperature = sensorData.TargetTemperature;

                    return sensor;
                }
            }

            return null;
        }

        public async Task<bool> SaveThermSensor(ThermSensor sensor, CancellationToken cancellationToken = default)
        {
            await new GrpcService().CallGrpc(
                async (client, headers) =>
                    await client.SetThermSensorAsync(new ThermSensorRequest
                    {
                        Id = sensor.Id.ToString(),
                        TargetTemperature = sensor.TargetTemperature ?? 0
                    }, headers, cancellationToken: cancellationToken),
                cancellationToken);

            return true;
        }
    }
}
