using Microsoft.EntityFrameworkCore;
using RegnalHome.Common;
using RegnalHome.Server;
using RegnalHome.Server.Executor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(Configuration.RegnalHomeServerConnectionString);
    options.LogTo(Console.WriteLine, LogLevel.Critical);
    options.EnableDetailedErrors();
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<Executor>();

var app = builder.Build();

app.MapControllers();

app.MapGet("/", async _ => await Task.FromResult("Welcome to RegnalHome.Server"));

app.Urls.Add(Configuration.Server.HostingUrl);
app.Urls.Add(Configuration.Server.HostingUrl.Replace("https://", "http://").Replace(":443", ":80"));
app.Urls.Add(Configuration.Server.HostingUrl.Replace("https://", "http://").Replace(":443", ":8080"));

app.UseDeveloperExceptionPage();

app.Run();