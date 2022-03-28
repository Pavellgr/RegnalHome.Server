using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Therm.Grpc;

namespace RegnalHome.GrpcSim.Services
{
    [Authorize]
    public class ThermService : RegnalHome.Therm.Grpc.Therm.ThermBase
    {
        private readonly ILogger<ThermService> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ThermService(ILogger<ThermService> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        public override async Task<ThermSensorReply> GetThermSensor(EmptyRequest request, ServerCallContext context)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(context.CancellationToken);
            var sensor = await dbContext.Sensors.FirstAsync();

            return await Task.FromResult(new ThermSensorReply
            {
                Id = sensor.Id.ToString(),
                Name = sensor.Name,
                Temperature = Random.Shared.Next(1, 100),
                TargetTemperature = sensor.TargetTemperature ?? 0
            });
        }

        public override async Task<BooleanReply> SetThermSensor(TargetTemperatureRequest request, ServerCallContext context)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(context.CancellationToken);
            var sensor = await dbContext.Sensors.FirstAsync();

            sensor.TargetTemperature = request.TargetTemperature;

            await dbContext.SaveChangesAsync(context.CancellationToken);

            return await Task.FromResult(new BooleanReply
            {
                Value = true
            });
        }
    }
}