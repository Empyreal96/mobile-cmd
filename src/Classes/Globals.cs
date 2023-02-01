using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace MobileTerminal.Classes
{
    class Globals
    {
        public static string TelnetIP { get; set; }
        public static bool CommandRunning { get; set; }

        public static string LaunchCommand { get; set; }

        // Access public elements/methods from MainPage
        public static MainPage MainPageContent
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }
    }
}