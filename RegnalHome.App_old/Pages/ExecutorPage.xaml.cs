using System.Text;
using Grpc.Core;

namespace RegnalHome.App.Pages;

public partial class ExecutorPage : ContentPage
{
  private readonly StringBuilder _logBuilder;
  private readonly ServerClient _serverClient;
  private Task LogReadLoop;


  public ExecutorPage()
  {
    InitializeComponent();

    _logBuilder = new StringBuilder();
    _serverClient = ServiceResolver.GetService<ServerClient>();
  }

  public bool ExecutorIsRunning { get; private set; }

  public string Log => _logBuilder.ToString();

  protected override void OnAppearing()
  {
    base.OnAppearing();
    LogReadLoop = Task.Run(LogReadLoopInner);
  }

  protected override void OnDisappearing()
  {
    base.OnDisappearing();
    LogReadLoop.Dispose();
  }

  private async Task LogReadLoopInner()
  {
    var response = await _serverClient.GetExecutorLog();
    while (await response.ResponseStream.MoveNext())
    {
      var current = response.ResponseStream.Current;
      _logBuilder.AppendLine(current.Text_);
    }
  }

  private void StartExecutorOnClicked(object sender, EventArgs e)
  {
    _serverClient.StartExecutor();
  }

  private void StopExecutorOnClicked(object sender, EventArgs e)
  {
    _serverClient.StopExecutor();
  }
}