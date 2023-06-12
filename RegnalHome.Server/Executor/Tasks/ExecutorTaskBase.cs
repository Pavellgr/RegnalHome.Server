using Microsoft.EntityFrameworkCore;

namespace RegnalHome.Server.Executor.Tasks;

public abstract class ExecutorTaskBase : IExecutorTask
{
  protected readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
  protected readonly IServiceProvider _serviceProvider;

  public ExecutorTaskBase(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
    _dbContextFactory = serviceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
  }

  public abstract string Name { get; set; }

  public abstract Task Execute(CancellationToken cancellationToken);
}