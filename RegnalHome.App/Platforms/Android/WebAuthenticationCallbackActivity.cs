using Android.App;
using Android.Content.PM;
using AndroidSys = Android;

namespace RegnalHome.App.Platforms.Android;

[Activity(NoHistory = true, LaunchMode = LaunchMode.SingleTop, Exported = true)]
[IntentFilter(new[] { AndroidSys.Content.Intent.ActionView },
              Categories = new[] {
                AndroidSys.Content.Intent.CategoryDefault,
                AndroidSys.Content.Intent.CategoryBrowsable
              },
              DataScheme = CALLBACK_SCHEME)]
public class WebAuthenticationCallbackActivity : WebAuthenticatorCallbackActivity
{
    const string CALLBACK_SCHEME = "myapp";
}