using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegnalHome.Common.RegnalIdentity;
using RegnalHome.Server.Areas.Identity;
using RegnalHome.Server.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(RegnalHome.Common.Configuration.RegnalHomeServerConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = "oidc";
})
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = $"https://{RegnalHome.Common.Configuration.Server.IPAddress}:{Configuration.IdentityServer.Port}";
    options.ClientId = Configuration.IdentityServer.Clients.RegnalHome.Server.ClientId;
    options.ClientSecret = Configuration.IdentityServer.Clients.RegnalHome.Server.ClientSecret;

    options.ResponseType = "code";
    options.SaveTokens = true;

    options.Events = new OpenIdConnectEvents
    {
        OnRemoteFailure = (context) =>
        {
            context.Response.Redirect("/Identity/Account/Login");
            context.HandleResponse();

            return Task.CompletedTask;
        }
    };

});

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

app.Run();
