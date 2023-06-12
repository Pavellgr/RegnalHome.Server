namespace RegnalHome.Server.Executor.Tasks;

public class DummyExecutorTask : ExecutorTaskBase
{
  public DummyExecutorTask(IServiceProvider serviceProvider)
    : base(serviceProvider)
  {
  }

  public override string Name { get; set; } = nameof(DummyExecutorTask);

  public override async Task Execute(CancellationToken cancellationToken)
  {
    await Task.Delay(TimeSpan.FromSeconds(7), cancellationToken);
  }
}