using System.Collections.Concurrent;
using System.Reflection;
using RegnalHome.Common.Enums;
using RegnalHome.Server.Executor.Tasks;

namespace RegnalHome.Server.Executor;

public class Executor
{
  private readonly IServiceProvider _serviceProvider;

  private readonly object _stateLock = new();

  public readonly ExecutorLog LogQueue = new();
  private readonly ConcurrentBag<IExecutorTask> TasksBag = new();
  private readonly Timer Timer;
  private CancellationTokenSource? _cts;
  private ExecutingState _state;

  public TimeSpan ExecutorRepeatTime = TimeSpan.FromSeconds(20);

  public Executor(IServiceProvider serviceProvider)
  {
    var tasks = Assembly.GetExecutingAssembly().DefinedTypes
      .Where(p => p.GetInterface(nameof(IExecutorTask)) != null)
      .Select(p => ActivatorUtilities.CreateInstance(serviceProvider, p.AsType()))
      .OfType<IExecutorTask>();

    foreach (var task in tasks) TasksBag.Add(task);

    Timer = new Timer(Execute, null, Timeout.Infinite, Timeout.Infinite);
  }

  public ExecutingState State
  {
    get
    {
      lock (_stateLock)
      {
        return _state;
      }
    }
    private set
    {
      lock (_stateLock)
      {
        _state = value;
      }
    }
  }

  public IReadOnlyCollection<IExecutorTask> Tasks => TasksBag;

  public void StartExecuting()
  {
    Timer.Change(TimeSpan.Zero, ExecutorRepeatTime);
  }

  public void StopExecuting()
  {
    State = ExecutingState.Stopped;
    Timer.Change(Timeout.Infinite, Timeout.Infinite);
    _cts?.Cancel(true);
    Log("Executor stopped.");
  }

  private void Execute(object? obj)
  {
    _cts = new CancellationTokenSource(ExecutorRepeatTime / 7 * 6);

    Log("Executor started executing.");
    State = ExecutingState.Running;

    Task.Factory.StartNew(() =>
    {
      var task = Task.WhenAll(Tasks.Select(p => Task.Factory.StartNew(() =>
        {
          _cts.Token.ThrowIfCancellationRequested();

          Log($"Task {p.Name} started executing.");

          _cts.Token.ThrowIfCancellationRequested();

          try
          {
            p.Execute(_cts.Token).GetAwaiter().GetResult();
          }
          catch (Exception e)
          {
            Log($"Task {p.Name} failed. {e.Message}");
          }

          _cts.Token.ThrowIfCancellationRequested();

          Log($"Task {p.Name} finished executing.");

          _cts.Token.ThrowIfCancellationRequested();
        })).ToArray())
        .ContinueWith(result =>
        {
          if (result.IsCompleted &&
              result.Exception?.InnerExceptions.FirstOrDefault() is OperationCanceledException)
          {
            State = ExecutingState.Cancelled;
            Log("Executor Cancelled.");
          }
          else if (result.IsCompletedSuccessfully)
          {
            State = ExecutingState.Finished;
            Log("Executor finished all tasks.");
          }
          else if (result.IsFaulted)
          {
            State = ExecutingState.Failed;
            Log("Executor failed.");
          }
        });

      return task;
    });
  }

  private void Log(string message)
  {
    LogQueue.Enqueue(message);
    Console.WriteLine(message);
  }
}