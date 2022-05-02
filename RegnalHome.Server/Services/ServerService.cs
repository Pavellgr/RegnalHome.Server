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
        private readonly Executor.Executor _executor;

        public ServerService(ILogger<ServerService> logger,
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            Executor.Executor executor)
        {
            _logger = logger;
            _dbContextFactory = dbContextFactory;
            _executor = executor;
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

        public override async Task<EmptyReply> StartExecutor(EmptyRequest request, ServerCallContext context)
        {
            await Task.Run(() => _executor.StartExecuting());
            return new EmptyReply();
        }

        public override async Task<EmptyReply> StopExecutor(EmptyRequest request, ServerCallContext context)
        {
            await Task.Run(() => _executor.StopExecuting());
            return new EmptyReply();
        }

        public override async Task ExecutorLog(EmptyRequest request, IServerStreamWriter<TextReply> responseStream, ServerCallContext context)
        {
            var startTime = DateTime.MinValue;

            while (true)
            {
                var logs = _executor.LogQueue.Where(p => p.TimeStamp > startTime).ToList();
                if (logs.Any())
                {

                    await responseStream.WriteAsync(new TextReply{Text = string.Join('\n', logs) });
                    startTime = DateTime.Now;
                    await Task.Delay(500);
                }
            }
        }
    }
}