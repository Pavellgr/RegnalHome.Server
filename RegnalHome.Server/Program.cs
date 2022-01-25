using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.ClientFactory;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using RegnalHome.Common;
using RegnalHome.Grpc;
using RegnalHome.Server.Areas.Identity;
using RegnalHome.Server.Data;
using RegnalHome.Server.Executor;
using RegnalHome.Server.Grpc;
using System.Reflection;
using RegnalHome.Server;
using RegnalHome.Server.Grpc.ClientFactories;
using Configuration = RegnalHome.Common.RegnalIdentity.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

// Add services to the container.
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
    options.UseSqlServer(RegnalHome.Common.Configuration.RegnalHomeServerConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddScoped<ThermService>();
builder.Services.AddScoped<ExecutorService>();
builder.Services.AddSingleton<DataStore>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultChallengeScheme = "oidc";
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority =
            $"https://{RegnalHome.Common.Configuration.Server.IPAddress}:{Configuration.IdentityServer.Port}";
        options.ClientId = Configuration.IdentityServer.Clients.RegnalHome.Server.ClientId;
        options.ClientSecret = Configuration.IdentityServer.Clients.RegnalHome.Server.ClientSecret;

        options.ResponseType = "code";
        options.SaveTokens = true;
        options.Scope.Add(Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Server);
        options.Scope.Add(Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Therm);

        options.Events = new OpenIdConnectEvents
        {
            OnRemoteFailure = (context) =>
            {
                context.Response.Redirect("/Identity/Account/Login");
                context.HandleResponse();

                return Task.CompletedTask;
            }
        };
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = $"https://{RegnalHome.Common.Configuration.Server.IPAddress}:{Configuration.IdentityServer.Port}";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddSingleton(_ => Mapper.CreateMapper(Assembly.GetAssembly(typeof(ApplicationDbContext))));

builder.Services.AddTransient<ThermClientFactory>();

builder.Services.AddSingleton<Executor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapGrpcService<GrpcServerService>();

app.Services.GetRequiredService<Executor>();

app.Urls.Add(RegnalHome.Common.Configuration.Server.HostingUrl);

app.Run();