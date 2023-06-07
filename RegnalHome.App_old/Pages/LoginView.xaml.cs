using RegnalHome.App.Authentication;

namespace RegnalHome.App.Pages;

public partial class LoginView : ContentView
{
  private readonly AuthenticationClient _authenticationClient;

  public LoginView()
  {
    InitializeComponent();
    _authenticationClient = ServiceResolver.GetService<AuthenticationClient>();
  }

  private async void OnLogInClicked(object sender, EventArgs e)
  {
    var loginResult = await _authenticationClient.LoginAsync();

    if (!loginResult.IsError) MauiProgram.IsAuthenticated = true;
    else
      await Application.Current.MainPage.DisplayAlert("Error", loginResult.ErrorDescription,
        "OK"); // https://auth0.com/blog/add-authentication-to-dotnet-maui-apps-with-auth0/#Run-Your-MAUI-App
  }

  private async void OnLogOutClicked(object sender, EventArgs e)
  {
    var logoutResult = await _authenticationClient.LogoutAsync();

    if (!logoutResult.IsError) MauiProgram.IsAuthenticated = false;
    else
      await Application.Current.MainPage.DisplayAlert("Error", logoutResult.ErrorDescription, "OK");
  }
}