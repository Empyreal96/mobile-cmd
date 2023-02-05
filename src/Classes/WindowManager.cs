using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace MobileTerminal.Classes;

public class WindowManager
{
    // Function to open a page as a new window
    public static async Task<bool> OpenPageAsNewWindowAsync(Type t)
    {
        var view = CoreApplication.CreateNewView();
        int id = 0;

        await view.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
        {
            var frame = new Frame();
            frame.Navigate(t, null);
            Window.Current.Content = frame;
            Window.Current.Activate();
            id = ApplicationView.GetForCurrentView().Id;
        });

        return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(id);
    }

    public static void EnterFullScreen(bool fs)
    {
        if (fs)
        {
            var view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();
        }
        else
        {
            var view = ApplicationView.GetForCurrentView();
            view.ExitFullScreenMode();
        }
    }
}