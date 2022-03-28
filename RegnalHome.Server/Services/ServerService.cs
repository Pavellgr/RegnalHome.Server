using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.Dtos;
using RegnalHome.Server.Data;
using RegnalHome.Server.Grpc;

namespace RegnalHome.Server.Services
{
    public class ServerService : RegnalHome.Server.Grpc.Server.ServerBase
    {
        private readonly ILogger<ServerService> _logger;
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public ServerService(ILogger<ServerService> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
        }

        public override async Task<BooleanReply> RegisterThermSensor(RegisterRequest request, ServerCallContext context)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync(context.CancellationToken);
            if (Guid.TryParse(request.Id, out var guidId))
            {
                var sensor = await dbContext.GetThermSensor(guidId, context.CancellationToken) ??
                             new ThermDto
                             {
                                 Id = guidId
                             };

                sensor.Name = request.Name;
                sensor.Address = request.Address;

                await dbContext.AddUpdateSensor(sensor, context.CancellationToken);

                return new BooleanReply
                {
                    Value = true
                };
            }

            return new BooleanReply
            {
                Value = false
            };
        }
    }
}