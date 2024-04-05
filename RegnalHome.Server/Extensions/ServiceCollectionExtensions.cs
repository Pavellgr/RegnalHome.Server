using RegnalHome.Server.Http.Endpoints;
using RegnalHome.Server.Http.HttpClients;
using RegnalHome.Server.Http.Options;
using RegnalHome.Server.Http.Settings;
using System.Diagnostics.CodeAnalysis;

namespace RegnalHome.Server.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
        => services
                .AddScoped<IEgdHttpClient, EgdHttpClient>()
                .AddKeyedSingleton<ITokenSettings, EgdSettings>(Constants.Egd)
                .AddKeyedSingleton<IOAuthOptions, EgdOptions>(Constants.Egd)
                .AddKeyedSingleton<IOAuthEndpoints, EgdEndpoints>(Constants.Egd);
}
