using RegnalHome.Common.Dtos;
using RegnalHome.Common.Enums;
using RegnalHome.Server.ClientFactories;
using RegnalHome.Server.Data;
using System.Collections.Concurrent;
using RegnalHome.Therm.Grpc;

namespace RegnalHome.Server.Executor.Tasks
{
    public class ThermExecutorTask : IExecutorTask
    {
        private readonly DataStore _dataStore;
        private readonly ThermClientFactory _thermClientFactory;

        public string Name { get; set; } = Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientName;

        public ThermExecutorTask(DataStore dataStore,
            ThermClientFactory thermClientFactory)
        {
            _dataStore = dataStore;
            _thermClientFactory = thermClientFactory;
        }

        public async Task Execute(CancellationToken cancellationToken)
        {
            var sensors = new ConcurrentBag<ThermDto>();

            await Parallel.ForEachAsync(await _thermClientFactory.CreateClients(),
                cancellationToken,
                async (clientObjs, token) =>
                {
                    var sensor = new Common.Models.Therm
                    {
                        ConnectionState = ConnectionState.Offline
                    };

                    var sensorData = await clientObjs.Client.GetThermSensorAsync(new EmptyRequest(), cancellationToken: token);

                    cancellationToken.ThrowIfCancellationRequested();

                    sensor.Id = Guid.Parse(sensorData.Id);
                    sensor.Name = sensorData.Name;
                    sensor.ConnectionState = ConnectionState.Online;
                    sensor.ActualTemperature = sensorData.Temperature;
                    sensor.TargetTemperature = sensorData.TargetTemperature;

                    sensors.Add(sensor);
                });

            _dataStore.AddUpdate(Name, sensors.ToList());
        }
    }
}
