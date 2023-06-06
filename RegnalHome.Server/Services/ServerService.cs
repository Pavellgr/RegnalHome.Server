using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Server.Grpc;

namespace RegnalHome.Server.Services;

public class ServerService : Grpc.Server.ServerBase
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly Executor.Executor _executor;

    public ServerService(IDbContextFactory<ApplicationDbContext> dbContextFactory,
        Executor.Executor executor)
    {
        _dbContextFactory = dbContextFactory;
        _executor = executor;
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

    public override async Task GetExecutorLog(EmptyRequest request, IServerStreamWriter<TextReply> responseStream,
        ServerCallContext context)
    {
        var startTime = DateTime.MinValue;

        while (true)
        {
            var logs = _executor.LogQueue.Where(p => p.TimeStamp > startTime).ToList();
            if (logs.Any())
            {
                await responseStream.WriteAsync(new TextReply {Text = string.Join('\n', logs)});
                startTime = DateTime.Now;
                await Task.Delay(500);
            }
        }
    }
}