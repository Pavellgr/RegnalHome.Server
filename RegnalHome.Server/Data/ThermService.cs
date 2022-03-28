using Microsoft.EntityFrameworkCore;
using RegnalHome.Common;
using RegnalHome.Common.Dtos;
using RegnalHome.Server.ClientFactories;
using RegnalHome.Therm.Grpc;

namespace RegnalHome.Server.Data
{
    public class ThermService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly Mapper _mapper;
        private readonly DataStore _dataStore;
        private readonly ThermClientFactory _thermClientFactory;

        public ThermService(IDbContextFactory<ApplicationDbContext> dbContextFactory,
            Mapper mapper,
            DataStore dataStore,
            ThermClientFactory thermClientFactory)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _dataStore = dataStore;
            _thermClientFactory = thermClientFactory;
        }

        public async Task<ThermDto[]> GetThermCus(CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var cus = await dbContext.GetThermCus(cancellationToken);
            return _mapper.Map<ThermDto>(cus).ToArray();
        }

        public async Task<ThermDto[]> GetThermSensors(CancellationToken cancellationToken = default)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

            var sensors = await dbContext.GetThermSensors(cancellationToken);
            return _mapper.Map<ThermDto>(sensors).ToArray();
        }

        public async Task<ThermDto?> GetThermSensor(string? id, CancellationToken cancellationToken = default)
        {
            if (id != null &&
                Guid.TryParse(id, out var guid))
            {
                cancellationToken.ThrowIfCancellationRequested();

                await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

                var sensorBase = await dbContext.GetThermSensor(guid, cancellationToken);

                if (sensorBase != null)
                {
                    var dataStoreEntry = _dataStore.TryGetValue(Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientName);

                    if (dataStoreEntry is List<ThermDto> sensors)
                    {
                        return sensors.FirstOrDefault(p => p.Id == guid);
                    }
                }
            }

            return null;
        }

        public async Task<bool> SaveThermSensor(Common.Models.Therm sensor, CancellationToken cancellationToken = default)
        {

            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
            var dbSensor = await dbContext.GetThermSensor(sensor.Id, cancellationToken);

            if (dbSensor == null)
            {
                return false;
            }

            var client = await _thermClientFactory.CreateClient(dbSensor.Address);

            var sensorData = await client.GetThermSensorAsync(new EmptyRequest(), cancellationToken: cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (Guid.TryParse(sensorData.Id, out var sensorDataGuidId) &&
                sensorDataGuidId == sensor.Id &&
                sensor.TargetTemperature != null)
            {
                var boolReply = await client.SetThermSensorAsync(new TargetTemperatureRequest { TargetTemperature = sensor.TargetTemperature.Value });

                return boolReply.Value;
            }

            return true;
        }
    }
}
