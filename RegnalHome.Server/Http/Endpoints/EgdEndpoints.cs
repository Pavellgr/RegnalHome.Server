namespace RegnalHome.Server.Http.Endpoints
{
    public class EgdEndpoints : IOAuthEndpoints
    {
        public string TokenEndpoint => "/oauth/token";

        public string DataEndpoint => "/rest/spotreby";
    }
}
