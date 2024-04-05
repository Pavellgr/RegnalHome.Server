namespace RegnalHome.Server.Http.Settings
{
    public interface ITokenSettings
    {
        DateTime ExpiresAt { get; set; }
        string ExpiresIn { get; set; }
        string Token { get; set; }
        string TokenType { get; set; }
    }
}