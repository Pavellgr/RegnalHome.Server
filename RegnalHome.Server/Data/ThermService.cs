using RegnalHome.Common.RegnalHome.Therm;

namespace RegnalHome.Server.Data
{
    public class ThermService
    {
        private ThermSensor[] sensors;

        public ThermService()
        {
            sensors = new[] {
                    new ThermSensor { Name = "First", ConnectionState = Common.ConnectionState.Online, Temperature = 5 },
                    new ThermSensor { Name = "Second", ConnectionState = Common.ConnectionState.Unknown },
                    new ThermSensor { Name = "Third", ConnectionState = Common.ConnectionState.Offline }
                };
        }

        public async Task<ThermSensor[]> GetThermSensors()
        {
            return await Task.FromResult(sensors);
        }

        public async Task<ThermSensor?> GetThermSensor(string? id)
        {
            if (id != null &&
                Guid.TryParse(id, out var guid))
            {
                return await Task.FromResult(sensors.FirstOrDefault(p => p.Id == guid));
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
                    sensor.ConnectionState = Common.ConnectionState.Online;
                    sensor.Temperature = 10;
                }

                return sensor;
            }

            return null;
        }
    }
}
