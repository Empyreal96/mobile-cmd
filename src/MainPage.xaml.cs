using Microsoft.UI.Xaml.Controls;
using MobileTerminal.Classes;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MobileTerminal
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            HideStatusBar();
        }

        private async void HideStatusBar()
        {
            // Hide the status bar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                await StatusBar.GetForCurrentView().HideAsync();
            }
        }

        private void TabView_Loaded(object sender, RoutedEventArgs e)
        {
            CreateNewTab("cmd", "Terminals.cmd", Symbol.Document);
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            CreateNewTab("cmd", "Terminals.cmd", Symbol.Document);
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            GC.Collect();
        }

        private void NewCMDTab(object sender, RoutedEventArgs e)
        {
            CreateNewTab("cmd", "Terminals.cmd", Symbol.Document);
        }

        private void NewPWSHTab(object sender, RoutedEventArgs e)
        {
            CreateNewTab("PowerShell", "Terminals.pwsh", Symbol.Document);
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

        private void CloseTerminal(object sender, RoutedEventArgs e)
        {
            Tools.ExitApp();
        }

        private void CreateNewTab(string header, object pageTag, Symbol icon)
        {
            TabViewItem newItem = new TabViewItem();

            newItem.Header = header;
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = icon };

            Frame frame = new Frame();
            newItem.Content = frame;
            frame.Navigate(Type.GetType($"MobileTerminal.{pageTag}"));

            Tabs.TabItems.Add(newItem);
            Tabs.SelectedItem = newItem;
        }
    }
}
