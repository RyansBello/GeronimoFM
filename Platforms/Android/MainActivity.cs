using Android.App;
using Android.Content.PM;
using Android.OS;

namespace Geronimo
{
    [Activity(Theme = "@style/Maui.SplashTheme", ResizeableActivity = true, LaunchMode = LaunchMode.SingleTask, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
