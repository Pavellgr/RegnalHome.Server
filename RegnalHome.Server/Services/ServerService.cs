using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Server.Grpc;
using DateTime = System.DateTime;

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

  public override async Task<Empty> StartExecutor(Empty request, ServerCallContext context)
  {
    await Task.Run(() => _executor.StartExecuting());
    return new Empty();
  }

  public override async Task<Empty> StopExecutor(Empty request, ServerCallContext context)
  {
    await Task.Run(() => _executor.StopExecuting());
    return new Empty();
  }

  public override async Task GetExecutorLog(Empty request, IServerStreamWriter<Text> responseStream,
    ServerCallContext context)
  {
    var startTime = DateTime.MinValue;

    while (true)
    {
      var logs = _executor.LogQueue.Where(p => p.TimeStamp > startTime).ToList();
      if (logs.Any())
      {
        await responseStream.WriteAsync(new Text {Value = string.Join('\n', logs)});
        startTime = DateTime.Now;
        await Task.Delay(500);
      }
    }
  }

  public override Task<IrrigationModules> GetIrrigationModules(Empty request, ServerCallContext context)
  {
    using (var dbContext = _dbContextFactory.CreateDbContext())
    {
      var modules = dbContext.IrrigationModules;

      return Task.FromResult(new IrrigationModules
      {
        Modules =
        {
          modules.Select(p => new IrrigationModule
          {
            Id = p.Id.ToString(),
            Name = p.Name,
            LastCommunication = new Grpc.DateTime
            {
              Date =
              {
                Day = p.LastCommunication.Day,
                Month = p.LastCommunication.Month,
                Year = p.LastCommunication.Year
              },
              Time =
              {
                Hour = p.LastCommunication.Hour,
                Minute = p.LastCommunication.Minute,
                Second = p.LastCommunication.Second
              }
            }
          })
        }
      });
    }
  }
}