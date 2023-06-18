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

app.Urls.Add(Configuration.Server.HostingUrl);

app.UseDeveloperExceptionPage();

app.Run();