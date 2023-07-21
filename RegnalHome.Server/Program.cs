using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegnalHome.Common;
using RegnalHome.Server;
using RegnalHome.Server.Executor;
using RegnalHome.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(Configuration.Server.Sql.ConnectionStrings.RegnalHome.Server);
});

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer",
        options =>
        {
            options.Authority = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl;

            options.TokenValidationParameters = new
                TokenValidationParameters
                {
                    ValidateAudience = false
                };
        });

builder.Services.AddControllers();

builder.Services.AddSingleton<Executor>();

var app = builder.Build();

await InitDatabase(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGrpcService<ServerService>().EnableGrpcWeb();//.RequireAuthorization();
app.MapGrpcService<IrrigationService>().EnableGrpcWeb();//.RequireAuthorization();

app.MapGet("/", () => "Welcome to RegnalHome.Server");

app.Urls.Add(Configuration.Server.HostingUrl);
app.Urls.Add(Configuration.Server.SslHostingUrl);

app.UseCors(c => c.AllowAnyOrigin()
.AllowAnyHeader()
.AllowAnyMethod());
app.UseGrpcWeb();

app.UseDeveloperExceptionPage();

app.Run();

async Task InitDatabase(WebApplication webApplication)
{
    var dbContextFactory = webApplication.Services.GetService<IDbContextFactory<ApplicationDbContext>>();
    using var dbContext = await dbContextFactory.CreateDbContextAsync();

    await dbContext.Database.MigrateAsync();
}