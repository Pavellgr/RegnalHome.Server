namespace RegnalHome.Server.Http.Settings
{
    public class EgdSettings : ITokenSettings
    {
        public string Token { get; set; }

        public string ExpiresIn { get; set; }

        public DateTime ExpiresAt { get; set; }

        public string TokenType { get; set; }

        public string Scope { get; set; }
    }
}
