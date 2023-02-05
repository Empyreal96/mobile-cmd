using static MobileTerminal.Classes.Globals;
using Windows.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls;
using MobileTerminal.Classes;
using System.Threading.Tasks;

namespace MobileTerminal.Pages;

public sealed partial class Settings : Page
{
    public Settings()
    {
        this.InitializeComponent();
        GetSettings();
    }

    private async void GetSettings()
    {
        await ApplySettingsToUiElements();
        // Set event handlers
        SendBtnSwitch.Toggled += SendBtnSwitch_Toggled;
        FontSizeSelection.ValueChanged += FontSizeSelection_ValueChanged;
        FontSelection.SelectionChanged += FontSelection_SelectionChanged;
        ClearHistoryBtn.Click += ClearHistoryBtn_Click;
        FullscreenToggle.Toggled += FullscreenToggle_Toggled;
    }

    private async Task ApplySettingsToUiElements()
    {
        if (localSettings.Values["HideSendBtn"] as string == "true")
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
        if (localSettings.Values["Fullscreen"] as string == "true")
        {
            FullscreenToggle.IsOn = true;
        }
    }
    #region Apperance page

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

    private void FontSelection_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        var fonts = Microsoft.Graphics.Canvas.Text.CanvasTextFormat.GetSystemFontFamilies();
        FontSelection.ItemsSource = fonts;
    }

    private void FontSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string Font = e.AddedItems[0].ToString();
        localSettings.Values["Font"] = Font;
    }

    #endregion

    #region History page
    private async void ClearHistoryBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        switch((sender as Button).Tag)
        {
            case "ClearHistory":
                await FileHelper.DeleteLocalFile("History.json");
                await UI.ShowDialog("Success", "History cleared successfully!");
                break;
        }
    }

    #endregion

    #region Startup page
    private void FullscreenToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (FullscreenToggle.IsOn == true)
        {
            WindowManager.EnterFullScreen(true);
            localSettings.Values["Fullscreen"] = "true";
        }
        else
        {
            WindowManager.EnterFullScreen(false);
            localSettings.Values["Fullscreen"] = "false";
        }
    }
    #endregion
}
