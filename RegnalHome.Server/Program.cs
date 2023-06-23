using Microsoft.EntityFrameworkCore;
using RegnalHome.Common;
using RegnalHome.Server;
using RegnalHome.Server.Executor;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(Configuration.Server.Sql.ConnectionStrings.RegnalHome.Server);
    options.LogTo(Console.WriteLine, LogLevel.Critical);
    options.EnableDetailedErrors();
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<Executor>();

var app = builder.Build();

await InitDatabase(app);

app.MapControllers();

app.MapGet("/", () => "Welcome to RegnalHome.Server");

app.Urls.Add(Configuration.Server.HostingUrl);

app.UseDeveloperExceptionPage();

app.Run();

async Task InitDatabase(WebApplication webApplication, CancellationToken cancellationToken)
{
    using var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope();
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    Console.Write("Connecting to database....");

    if (dbContext.Database.CanConnect())
    {
        Console.WriteLine("Connected.");

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
    else
    {
        Console.WriteLine("Not connected.");
    }
}