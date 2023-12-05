using RegnalHome.App.Authentication;

namespace RegnalHome.App.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly OAuth2Client _oauth2Client;
        private readonly ISettingsService _settings;

        public AuthenticationService(OAuth2Client oauth2Client, ISettingsService settingsService)
        {
            _oauth2Client = oauth2Client;
            _settings = settingsService;
        }

        public async Task LoginAsync()
        {
            var loginResult = await _oauth2Client.LoginAsync();

            if (!loginResult.IsError)
            {
                _settings.IsAuthenticated = true;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", loginResult.ErrorDescription, "OK");
            }
        }

        public async Task LogoutAsync()
        {
            _settings.IsAuthenticated = false;
            await Task.CompletedTask;
        }
    }
}
