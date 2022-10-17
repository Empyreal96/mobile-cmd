using MobileTerminal.Classes;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace MobileTerminal.Pages
{
    public sealed partial class Settings : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public Settings()
        {
            this.InitializeComponent();
            GetSettings();
            GetAllFonts();
        }

        private void GetSettings()
        {
            string IsSendBtnHidden = localSettings.Values["HideSendBtn"] as string;
            if (IsSendBtnHidden == "true")
            {
                SendBtnSwitch.IsOn = true;
            }
            string Font = localSettings.Values["Font"] as string;
            if (Font != null)
            {
                FontSelection.PlaceholderText = Font;
            }
            else
            {
                FontSelection.PlaceholderText = "Select a font...";
            }
        }

        private void GetAllFonts()
        {
            var fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
            FontSelection.ItemsSource = fonts;
        }

        private void SendBtnSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (SendBtnSwitch.IsOn == true)
            {
                localSettings.Values["HideSendBtn"] = "true";
            }
            else
            {
                localSettings.Values["HideSendBtn"] = "false";
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Font = e.AddedItems[0].ToString();
            localSettings.Values["Font"] = Font;
        }

    }
}
