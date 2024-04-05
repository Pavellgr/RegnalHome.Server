namespace RegnalHome.Server.Http.Options
{
    public interface IOAuthOptions
    {
        string IdentityServiceUrl { get; }

        string ApiServiceUrl { get; }

        string ClientId { get; }

        string ClientSecret { get; }

        string Scope { get; }
    }
}
