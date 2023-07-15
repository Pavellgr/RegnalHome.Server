using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RegnalHome.Web;
using RegnalHome.Web.Services;
using System.Net.Mime;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl;

    options.ProviderOptions.RedirectUri = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.RedirectUrl;
    options.ProviderOptions.ClientId = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.ClientId;
    options.ProviderOptions.DefaultScopes.Add(RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Server);
    options.ProviderOptions.DefaultScopes.Add(RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Irrigation);
    options.ProviderOptions.ResponseType = "code";
});

builder.Services.AddSingleton(_ =>
{
    var httpHandler = new GrpcWebHandler(new HttpClientHandler());
    return GrpcChannel.ForAddress(RegnalHome.Common.Configuration.Server.HostingUrl.Replace("0.0.0.0", "localhost"), new GrpcChannelOptions { HttpHandler = httpHandler });
});

builder.Services.AddScoped(provider => new RegnalHome.Irrigation.Grpc.Irrigation.IrrigationClient(provider.GetRequiredService<GrpcChannel>()));

builder.Services.AddScoped<IrrigationService>();
builder.Services.AddSingleton<ITranslationService, TranslationService>();

await builder.Build().RunAsync();
