using RegnalHome.Common;

namespace RegnalHome.Server.Executor
{
    public class ExecutorLog : ObservableConcurrentQueue<(DateTime TimeStamp, string Message)>
    {
        public void Enqueue(string message)
        {
            Enqueue((DateTime.Now, message));
        }
    }
}
