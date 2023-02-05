using static MobileTerminal.Classes.Globals;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using MobileTerminal.Classes;

namespace MobileTerminal.Pages;

public sealed partial class Settings : Page
{
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
        string FontSize = localSettings.Values["FontSize"] as string;
        if (FontSize != null)
        {
            FontSizeSelection.Value = double.Parse(FontSize);
        }
        else
        {
            FontSizeSelection.Value = 14;
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

    private void FontSizeSelection_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        NumberBox numberbox = (NumberBox)sender;
        string FontSize = numberbox.Value.ToString();
        localSettings.Values["FontSize"] = FontSize;
    }

    private void FontSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string Font = e.AddedItems[0].ToString();
        localSettings.Values["Font"] = Font;
    }

    private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        switch((sender as Button).Tag)
        {
            case "ClearHistory":
                await FileHelper.DeleteLocalFile("History.json");
                await UI.ShowDialog("Success", "History cleared successfully!");
                break;
        }
    }
}
