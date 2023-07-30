using Blazored.Toast;
using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RegnalHome.Web;
using RegnalHome.Web.Services;
using System.Diagnostics;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseAddress = builder.HostEnvironment.BaseAddress;

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(baseAddress) });

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl;

    options.ProviderOptions.RedirectUri = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.GetRedirectUrl(baseAddress);
    options.ProviderOptions.PostLogoutRedirectUri = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.GetPostLogoutRedirectUrl(baseAddress);
    options.ProviderOptions.ClientId = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.ClientId;
    options.ProviderOptions.DefaultScopes.Add(RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Server);
    options.ProviderOptions.DefaultScopes.Add(RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Irrigation);
    options.ProviderOptions.ResponseType = "code";
});

builder.Services.AddScoped(serviceProvider =>
{
    var httpHandler = new GrpcWebHandler(new HttpClientHandler());

    var credentials = CallCredentials.FromInterceptor(async (context, metadata) =>
    {
        var tokenResult = await serviceProvider.GetRequiredService<IAccessTokenProvider>().RequestAccessToken();
        tokenResult.TryGetToken(out var token);
        metadata.Add("Authorization", $"Bearer {token.Value}");
    });
    var grpcAddress = RegnalHome.Common.Configuration.Server.GetUrl(baseAddress);
    Console.WriteLine($"GRPC address: {grpcAddress}");

    return GrpcChannel.ForAddress(grpcAddress, new GrpcChannelOptions
    {
        HttpHandler = httpHandler,
        //Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
    });
});

builder.Services.AddBlazoredToast();

builder.Services.AddScoped(provider =>
{
    var client = new RegnalHome.Irrigation.Grpc.Irrigation.IrrigationClient(provider.GetRequiredService<GrpcChannel>());
    return client;
});

builder.Services.AddScoped<IrrigationService>();
builder.Services.AddSingleton<ITranslationService, TranslationService>();

await builder.Build().RunAsync();
