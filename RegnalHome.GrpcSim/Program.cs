using IdentityServer4.AccessTokenValidation;
using RegnalHome.Common.RegnalIdentity;
using RegnalHome.GrpcSim.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();


builder.Services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
    .AddIdentityServerAuthentication(options =>
    {
        options.Authority = $"https://{RegnalHome.Common.Configuration.Server.IPAddress}:{Configuration.IdentityServer.Port}";
        options.ApiName = Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientId;
        options.ApiSecret = Configuration.IdentityServer.Clients.RegnalHome.Therm.ClientSecret;
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ThermService>().RequireAuthorization();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseAuthentication();
app.UseAuthorization();

app.Run();
