using PenguinApps.Core;
using Windows.UI.Xaml.Controls;

namespace MobileTerminal.Pages;

public sealed partial class About : Page
{
    public About()
    {
        this.InitializeComponent();
        MobileTerminalVersion.Text = "MobileTerminal v" + AppVersion.GetAppVersion() + " Beta";
    }
}