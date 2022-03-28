using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegnalHome.Common;
using RegnalHome.Server.ClientFactories;
using RegnalHome.Server.Data;
using RegnalHome.Server.Executor;
using RegnalHome.Server.Services;
using Configuration = RegnalHome.Common.RegnalIdentity.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(RegnalHome.Common.Configuration.RegnalHomeServerConnectionString));

builder.Services.AddScoped<ThermService>();
builder.Services.AddScoped<ExecutorService>();
builder.Services.AddSingleton<DataStore>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = "oidc";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = $"https://{RegnalHome.Common.Configuration.Server.Address}:{Configuration.IdentityServer.Port}";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddSingleton(_ => Mapper.CreateMapper(Assembly.GetAssembly(typeof(ApplicationDbContext))));

builder.Services.AddTransient<ThermClientFactory>();

builder.Services.AddSingleton<Executor>();

var app = builder.Build();

var certPath = "/https/cert.pfx";
if (File.Exists(certPath))
{
    app.AddCertificate(certPath);
}
else
{
    Console.WriteLine(certPath + "doesn't exists.");
}

// Configure the HTTP request pipeline.
app.MapGrpcService<ServerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Services.GetRequiredService<Executor>();

app.Urls.Add(RegnalHome.Common.Configuration.Server.HostingUrl);

app.Run();
