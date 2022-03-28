using RegnalHome.Common.Enums;
using RegnalHome.Server.Executor;
using RegnalHome.Server.Executor.Tasks;

namespace RegnalHome.Server.Data
{
    public class ExecutorService
    {
        private readonly Executor.Executor _executor;

        public ExecutorService(Executor.Executor executor)
        {
            _executor = executor;
        }

        public (ExecutingState State, IReadOnlyCollection<IExecutorTask> tasks) GetModel()
        {
            return (_executor.State, _executor.Tasks);
        }

        public void Stop()
        {
            _executor.StopExecuting();
        }

        public void Start()
        {
            _executor.StartExecuting();
        }
    }
}
