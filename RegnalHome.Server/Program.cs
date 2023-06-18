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

var certPath = "cert.pfx";
if (File.Exists(certPath))
    app.AddCertificate(certPath);
else
    Console.WriteLine(certPath + " doesn't exists.");

app.Urls.Add(Configuration.Server.HostingUrl);

app.UseDeveloperExceptionPage();

app.Run();