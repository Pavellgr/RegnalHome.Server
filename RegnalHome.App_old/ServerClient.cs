using Grpc.Core;
using RegnalHome.Server.Grpc;

namespace RegnalHome.App;

public class ServerClient : Server.Grpc.Server.ServerClient
{
  public async Task StartExecutor()
  {
    await base.StartExecutorAsync(new Empty());
  }

  public async Task StopExecutor()
  {
    await base.StopExecutorAsync(new Empty());
  }

  public async Task<AsyncServerStreamingCall<Text>> GetExecutorLog()
  {
    return base.GetExecutorLog(new Empty());
  }
}