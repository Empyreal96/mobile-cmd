using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Telnet;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Command_Prompt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string LocalPath = ApplicationData.Current.LocalFolder.Path;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient client = new TelnetClient(TimeSpan.FromSeconds(3), cancellationTokenSource.Token);
        public static string CMDCommandText { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            ProgressBarControl(false);

            bool isDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;
            if (isDark == true)
            {
                HeaderBorder.Background = new SolidColorBrush(Colors.DimGray);
            }

                ApplicationData.Current.LocalFolder.CreateFileAsync("cmdstring.txt", CreationCollisionOption.ReplaceExisting);


            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            ulong major = (version & 0xFFFF000000000000L) >> 48;
            ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            ulong revision = (version & 0x000000000000FFFFL);
            var osVersion = $"{major}.{minor}.{build}.{revision}";

            Globals.ReportedBuildVersion = build;
            Globals.FullBuildNumber = osVersion;
            ProgRing.IsEnabled = false;
            ProgRing.Visibility = Visibility.Collapsed;
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                ProgRing.IsEnabled = false;
                ProgRing.Visibility = Visibility.Collapsed;
                ProgressBarControl(false);

                Exceptions.ThrowFullError(ex);
                
            }
        }



        public async void Connect()
        {
           
            try
            {
                ProgressBarControl(true);
                CMDtestText.Text = $"Connecting to session";


                await client.Connect();
                // await Task.Delay(1000);
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text =
                    $"Microsoft Windows [{Globals.FullBuildNumber}]\n" +
                    $"(c) Microsoft Corporation. All rights reserved." +
                    $"\n\n{results}";

                ProgressBarControl(false);
            }
            catch (Exception ex)
            {
                ProgressBarControl(false);

                Exceptions.ThrowFullError(ex);
            }
        }
        private void HandleErrorReceived(object sender, string e)
        {
            try
            {
                Exceptions.CustomMessage($"Returned string: {e}");
            }
            catch (Exception ex)
            {
                ProgressBarControl(false);

            }
        }

        private void HandleMessageReceived(object sender, string e)
        {
            try
            {
                Exceptions.CustomMessage($"Returned string: {e}");
            }
            catch (Exception ex)
            {
                ProgressBarControl(false);

            }
        }

        private void HandleConnectionClosed(object sender, EventArgs e)
        {
            try
            {
                Exceptions.CustomMessage($"Returned string: {e}");
            }
            catch (Exception ex)
            {
                ProgressBarControl(false);

            }
        }
        private async void SendCommandBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarControl(true);
                var text = CMDtestText.Text;
                CMDtestText.Text = text.Remove(text.Length - 1);
                CMDtestText.Text += $"{SendCommandText.Text}\n";
                StorageFile tmp = await ApplicationData.Current.LocalFolder.GetFileAsync("cmdstring.txt");
                ProgRing.IsEnabled = true;
                ProgRing.Visibility = Visibility.Visible;
                string command = SendCommandText.Text;
                if (command.Length != 0)
                {
                    CMDtestText.Text += "\n";
                    await client.Send($"{command} > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                    //await Task.Delay(500);
                    string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");

                    await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                    CMDtestText.Text += $"{results}";
                    // await Task.Delay(500);
                    string cd = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                    CMDtestText.Text += $"\n\n{cd}";

                    ProgRing.IsEnabled = false;
                    ProgRing.Visibility = Visibility.Collapsed;
                    SendCommandText.Text = "";



                    var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
                    for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                    {
                        object obj = VisualTreeHelper.GetChild(grid, i);
                        if (!(obj is ScrollViewer)) continue;
                        ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                        break;
                    }
                }

                ProgressBarControl(false);
            }
            catch (Exception ex)
            {
                ProgRing.IsEnabled = false;
                ProgRing.Visibility = Visibility.Collapsed;
                Exceptions.ThrowFullError(ex);
                ProgressBarControl(false);

            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            // Basic commands
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            // clear
            ProgressBarControl(true);
            CMDtestText.Text =
                   $"Microsoft Windows [{Globals.FullBuildNumber}]\n" +
                   $"(c) Microsoft Corporation. All rights reserved." +
                   $"\n\n";
            await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
            string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
            CMDtestText.Text += $"{results}";
            ProgressBarControl(false);
        }

        private async void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarControl(true);
                // Load Script
                FileOpenPicker file = new FileOpenPicker();
                file.FileTypeFilter.Add(".bat");
                file.FileTypeFilter.Add(".cmd");
                file.FileTypeFilter.Add(".exe");

                StorageFile storage = await file.PickSingleFileAsync();
                if (storage == null)
                {
                    return;
                }
                string scriptPath = storage.Path;
                string scriptParent = Path.GetDirectoryName(scriptPath);

                await client.Send($"cd /d \"{scriptParent}\" > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{results}\n";


                await client.Send($"\"{storage.Path}\" > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string results2 = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{results2}\n";

                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string results3 = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{results3}";


                var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
                for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                {
                    object obj = VisualTreeHelper.GetChild(grid, i);
                    if (!(obj is ScrollViewer)) continue;
                    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                    break;
                }
                ProgressBarControl(false);
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
                ProgressBarControl(false);

            }
        }

        private async void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            //Exit
            StackPanel Panel = new StackPanel();

            TextBlock nameHeader = new TextBlock();
            nameHeader.Margin = new Thickness(5, 5, 5, 5);
            nameHeader.Text = $"Quit application?";
            nameHeader.HorizontalAlignment = HorizontalAlignment.Stretch;
            Panel.Children.Add(nameHeader);
            ContentDialog dialog = new ContentDialog();
            dialog.Content = Panel;
            dialog.VerticalContentAlignment = VerticalAlignment.Center;
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Confirm";
            dialog.SecondaryButtonText = "Cancel";

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                Windows.UI.Xaml.Application.Current.Exit();
            }
        }



        private void BtnFlyout_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void ProgressBarControl(bool enable)
        {
            if (enable == true)
            {
                progbar.IsEnabled = true;
                progbar.IsIndeterminate = true;
                progbar.Visibility = Visibility.Visible;
            } else
            {
                progbar.IsEnabled = false;
                progbar.IsIndeterminate = false;
                progbar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
