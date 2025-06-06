#if WINDOWS
using Microsoft.UI.Windowing;
using WinRT.Interop;
using static Microsoft.UI.Win32Interop;
using Windows.Graphics;
using Windows.Graphics.Display;
using System.Runtime.InteropServices;
#endif

using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace Geronimo
{
    public static class MauiProgram
    {

#if WINDOWS
        // Win32 API untuk always on top
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_SHOWWINDOW = 0x0040;
#endif

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkitMediaElement()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<INavigation>(sp =>
            {
                var navigationPage = new NavigationPage(new MainPage());
                return navigationPage.Navigation;
            });
            // Hook ke handler window Windows
            builder.ConfigureLifecycleEvents(events =>
            {
#if WINDOWS
                events.AddWindows(windows =>
                {
                    windows.OnWindowCreated(window =>
                    {
                        var hwnd = WindowNative.GetWindowHandle(window);
                        var windowId = GetWindowIdFromWindow(hwnd);
                        var appWindow = AppWindow.GetFromWindowId(windowId);

                        // Atur ukuran jendela
                        int width = 475;
                        int height = 275;

                        // Ambil informasi layar
                        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                        var screenBounds = displayArea.WorkArea;

                        // Hitung posisi pojok kanan bawah
                        int x = screenBounds.X + screenBounds.Width - width;
                        int y = screenBounds.Y + screenBounds.Height - height;

                        // Set posisi & ukuran jendela
                        appWindow.MoveAndResize(new Windows.Graphics.RectInt32(x, y, width, height));

                        // Cegah resize, maximize, minimize
                        if (appWindow.Presenter is OverlappedPresenter presenter)
                        {
                            presenter.IsResizable = false;
                            presenter.IsMaximizable = false;
                            presenter.IsMinimizable = false;
                        }

                        // Set always on top
                        SetWindowPos(hwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
                    });
                });
#endif
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
