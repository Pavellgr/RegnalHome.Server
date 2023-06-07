using RegnalHome.App.Authentication;
using RegnalHome.App.Pages;
using RegnalHome.App.ViewModels;
using RegnalHome.Common;

namespace RegnalHome.App;

public static class MauiProgram
{
  public static bool IsAuthenticated { get; internal set; }

  public static MauiApp CreateMauiApp()
  {
    var builder = MauiApp.CreateBuilder();
    builder
      .UseMauiApp<App>()
      .ConfigureFonts(fonts =>
      {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
      })
      .Services.AddGrpcClient<ServerClient>(o => { o.Address = new Uri(Configuration.Server.HostingUrl); });

    builder.Services.AddSingleton(new AuthenticationClient(new AuthenticationClientOptions
    {
      Domain = $"{Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl}",
      ClientId = $"{Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.ClientId}",
      Scope =
        $"{Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Server}",
      RedirectUri = "myapp://callback"
    }));

    builder.Services.AddTransient<ExecutorPage>();
    builder.Services.AddTransient<ServerClient>();

    builder.Services.AddTransient<IrrigationViewModel>();

    var app = builder.Build();

    ServiceResolver.Register(app.Services);

    return app;
  }
}

public static class ServiceResolver
{
  private static IServiceProvider _serviceProvider;

  public static void Register(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  public static T GetService<T>()
  {
    if (_serviceProvider == null) throw new InvalidOperationException();

    return _serviceProvider.GetRequiredService<T>();
  }

  public static T GetViewModel<T>() where T : BaseViewModel
  {
    if (_serviceProvider == null) throw new InvalidOperationException();

    return _serviceProvider.GetRequiredService<T>();
  }
}