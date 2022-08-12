using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Metadata;
using Windows.System.Profile;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Command_Prompt
{
    public sealed partial class TabsPage : Page
    {
        public TabsPage()
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
            CreateNewTab("cmd", "MainPage", Symbol.Document);
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            CreateNewTab("cmd", "MainPage", Symbol.Document);
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            GC.Collect();
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
            TabViewItem newItem = new TabViewItem();

            newItem.Header = header;
            newItem.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = icon };

            Frame frame = new Frame();
            newItem.Content = frame;
            frame.Navigate(Type.GetType($"Command_Prompt.{pageTag}"));

            Tabs.TabItems.Add(newItem);
            Tabs.SelectedItem = newItem;
        }
    }
}
