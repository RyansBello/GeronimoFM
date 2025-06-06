using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Geronimo.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

#if WINDOWS
            var mauiWindow = Microsoft.Maui.Controls.Application.Current.Windows.FirstOrDefault();
            var nativeWindow = mauiWindow?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;

            if (nativeWindow is not null)
            {
                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);

                // Sembunyikan dari Alt+Tab
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);
                appWindow.IsShownInSwitchers = false;

                // Sembunyikan dari Taskbar
                int exStyle = Win32Interop.GetWindowLongPtr(hWnd, Win32Interop.GWL_EXSTYLE);
                exStyle &= ~Win32Interop.WS_EX_APPWINDOW;
                exStyle |= Win32Interop.WS_EX_TOOLWINDOW;
                Win32Interop.SetWindowLongPtr(hWnd, Win32Interop.GWL_EXSTYLE, exStyle);
            }
#endif
        }
    }

    internal static class Win32Interop
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int WS_EX_APPWINDOW = 0x00040000;

        [DllImport("user32.dll")]
        public static extern int GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLongPtr(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
