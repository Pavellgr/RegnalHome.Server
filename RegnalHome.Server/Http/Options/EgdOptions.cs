namespace RegnalHome.Server.Http.Options
{
    public class EgdOptions : IOAuthOptions
    {
        public string IdentityServiceUrl => "https://idm.distribuce24.cz/";

        public string ApiServiceUrl => "https://data.distribuce24.cz/";

        public string ClientId => "064239187e2a0784520e345726ace752";

        public string ClientSecret => "824d3ca3d7a0de9bb4a873e89fba0c29";

        public string Scope => "namerena_data_openapi";

        public string Ean => "859182400202690899";

        public DateTime BeginDateTime => new DateTime(2024, 3, 18);
    }
}
