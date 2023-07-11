using RegnalHome.App.Authentication;
using RegnalHome.App.Services;

namespace RegnalHome.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSingleton(new OAuth2Client(new()
            {
                Authority = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.IdentityServerUrl,
                ClientId = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.ClientId,
                Scope = RegnalHome.Common.RegnalIdentity.Configuration.IdentityServer.Clients.RegnalHome.Server.AllowedScopes.Server,
                RedirectUri = "myapp://callback"
            }));

            //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddRegnalIdentity();

            //builder.Services.AddAuthorization();

            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();
            builder.Services.AddSingleton<ISettingsService, SettingsService>();

            return builder.Build();
        }
    }
}