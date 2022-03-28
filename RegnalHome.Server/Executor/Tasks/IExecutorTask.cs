namespace RegnalHome.Server.Executor.Tasks;

public interface IExecutorTask
{
    string Name { get; set; }

    Task Execute(CancellationToken cancellationToken);
}