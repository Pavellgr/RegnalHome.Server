namespace RegnalHome.Server.Executor.Tasks
{
    public class DummyExecutorTask : IExecutorTask
    {
        string IExecutorTask.Name { get; set; } = nameof(DummyExecutorTask);

        public async Task Execute(CancellationToken cancellationToken)
        {
            await Task.Delay(TimeSpan.FromSeconds(7), cancellationToken);
        }
    }
}
