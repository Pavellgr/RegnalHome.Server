namespace RegnalHome.App.Services
{
    public interface IAuthenticationService
    {
        Task LoginAsync();
        Task LogoutAsync();
    }
}