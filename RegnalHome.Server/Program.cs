using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RegnalHome.Common;
using RegnalHome.Server;
using RegnalHome.Server.Authentication;
using RegnalHome.Server.Executor;
using RegnalHome.Server.Extensions;
using RegnalHome.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddDbContextFactory<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(Configuration.Server.Sql.ConnectionStrings.RegnalHome.Server);
    })
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = builder.Environment.ApplicationName, Version = "v1" });
    })
    .AddGrpc();

builder.Services
    .AddLogging(o => o.AddConsole())
    .AddServices()
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddScheme<OAuthClientCredentialsOptions, OAuthClientCredentialsHandler>(Constants.Egd, o =>
    {
        o.TokenUrl = $"{RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl}/connect/token";
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
        options =>
        {
            options.Authority = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false
            };
        });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Constants.Egd, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddAuthenticationSchemes(Constants.Egd);
        policy.RequireClaim("scope", RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.HomeAssistant.Scope);
    });
});

builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddCors(cors =>
    cors.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
            policy.AllowAnyOrigin();
        }
    )
);

builder.Services.AddSingleton<Executor>();

var app = builder.Build();

await InitDatabase(app);

app.UseAuthentication()
   .UseAuthorization()
   .UseSwagger(options =>
   {
       options.RouteTemplate = ".less-known/api-docs/{documentName}.json";
       options.PreSerializeFilters.Add((swagger, httpRequest) =>
       {
           //Clear servers -element in swagger.json because it got the wrong port when hosted behind reverse proxy
           swagger.Servers.Clear();
       });
   })
   .UseSwaggerUI(options =>
   {
       options.SwaggerEndpoint("/.less-known/api-docs/v1.json", "v1");
       options.RoutePrefix = ".less-known/api-docs/ui";
   });

app.MapControllers();

app.MapGrpcService<ServerService>().EnableGrpcWeb();//.RequireAuthorization();
app.MapGrpcService<IrrigationService>().EnableGrpcWeb();//.RequireAuthorization();

app.MapGet("/", () => "Welcome to RegnalHome.Server");

app.Urls.Add(Configuration.Server.HostingUrl);
app.Urls.Add(Configuration.Server.SslHostingUrl);

app.UseCors()
   .UseGrpcWeb()
   .UseDeveloperExceptionPage();

app.Run();

async Task InitDatabase(WebApplication webApplication)
{
    var dbContextFactory = webApplication.Services.GetService<IDbContextFactory<ApplicationDbContext>>();
    using var dbContext = await dbContextFactory.CreateDbContextAsync();

    await dbContext.Database.MigrateAsync();
}