using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegnalHome.Common.RegnalIdentity;
using RegnalHome.GrpcSim;
using RegnalHome.GrpcSim.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(RegnalHome.Common.Configuration.RegnalHomeGrpcSimConnectionString));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = $"https://{RegnalHome.Common.Configuration.Server.IPAddress}:{Configuration.IdentityServer.Port}";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<ThermService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseAuthentication();
app.UseAuthorization();

app.Run();
