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
            string Font = localSettings.Values["Font"] as string;
            if (Font != null)
            {
                FontSelection.SelectedItem = Font;
            }
        }

        private void GetAllFonts()
        {
            var fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
            FontSelection.ItemsSource = fonts;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string Font = e.AddedItems[0].ToString();
            localSettings.Values["Font"] = Font;
        }
    }
}
