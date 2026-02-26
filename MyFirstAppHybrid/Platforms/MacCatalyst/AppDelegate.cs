using Foundation;

namespace MyFirstAppHybrid
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MyApp.CreateMauiApp();
    }
}
