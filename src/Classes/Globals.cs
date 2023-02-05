using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.Storage;
using System.Collections.Generic;

namespace MobileTerminal.Classes;

class Globals
{
    public static StorageFolder localFolder = ApplicationData.Current.LocalFolder;

    public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

    public static string TelnetIP { get; set; }
    public static bool CommandRunning { get; set; }

    public static string LaunchCommand { get; set; }

    // Access public elements/methods from MainPage
    public static MainPage MainPageContent
    {
        get { return (Window.Current.Content as Frame)?.Content as MainPage; }
    }

    public class JsonItems
    {
        public string Command { get; set; }
        public string ExecutionTimeDate { get; set; }
    }

    public static List<JsonItems> JsonItemsList;
}