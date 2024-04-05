using RegnalHome.Server.Http.Settings;
using System.Text.Json.Serialization;

namespace RegnalHome.Server.Http.Responses
{
    public class TokenResponse : IHttpResponse
    {
        public string access_token { get; init; }

        private string _expiresIn;

        public string expires_in
        {
            get => expires;
            init
            {
                expires = value;
            }
        }


        public string expires
        {
            get => _expiresIn;
            init
            {
                _expiresIn = value;
                ExpiresAt = DateTime.Now.AddSeconds(int.Parse(value));
            }
        }

        public string token_type { get; init; }

        public string scope { get; init; }

        public DateTime ExpiresAt { get; init; }

        public void CopyTo(ITokenSettings settings)
        {
            settings.Token = access_token;
            settings.ExpiresIn = expires;
            settings.ExpiresAt = ExpiresAt;
            settings.TokenType = token_type;
        }
    }
}
