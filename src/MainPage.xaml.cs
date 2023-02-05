using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using MobileTerminal.Classes;

namespace MobileTerminal;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        WindowManager.EnterFullScreen(true);
    }

    private void TabView_Loaded(object sender, RoutedEventArgs e)
    {
        NewLocalTab();
    }

    private void TabView_AddButtonClick(TabView sender, object args)
    {
        NewLocalTab();
    }

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
        GC.Collect();
    }

    private async void NewWindow(object sender, RoutedEventArgs e)
    {
        await WindowManager.OpenPageAsNewWindowAsync(typeof(MainPage));
    }

    public void NewLocalTab()
    {
        Globals.TelnetIP = "127.0.0.1";
        CreateNewTab("Terminal (Local)", "Pages.Terminal", Symbol.Document);
    }
    private void NewLocalTab(object sender, RoutedEventArgs e)
    {
        NewLocalTab();
    }

    private void OpenRemoteConnectionDialog(object sender, RoutedEventArgs e)
    {
        RemoteConnectionDialog.Visibility = Visibility.Visible;
    }
    private void NewRemoteTab(object sender, RoutedEventArgs e)
    {
        RemoteConnectionDialog.Visibility = Visibility.Collapsed;
        Globals.TelnetIP = IPAddressBox.Text;
        CreateNewTab("Terminal (Remote)", "Pages.Terminal", Symbol.Document);
    }

    private void OpenHistory(object sender, RoutedEventArgs e)
    {
        CreateNewTab("History", "Pages.History", Symbol.Placeholder);
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        CreateNewTab("Settings", "Pages.Settings", Symbol.Setting);
    }

    private void OpenHelp(object sender, RoutedEventArgs e)
    {
        CreateNewTab("Help", "Pages.Help", Symbol.Help);
    }
    private void OpenAbout(object sender, RoutedEventArgs e)
    {
        CreateNewTab("About", "Pages.About", Symbol.ContactInfo);
    }

    private void CreateNewTab(string header, object pageTag, Symbol icon)
    {
        TabViewItem newItem = new()
        {
            Header = header,
            IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = icon }
        };

        Frame frame = new();
        newItem.Content = frame;
        frame.Navigate(Type.GetType($"MobileTerminal.{pageTag}"));

        Tabs.TabItems.Add(newItem);
        Tabs.SelectedItem = newItem;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        RuntimeManager.ExitApp();
    }
}
