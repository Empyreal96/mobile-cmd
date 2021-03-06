using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Telnet;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Core;
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
        TelnetClient client = new TelnetClient(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
        public static string CMDCommandText { get; set; }



        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                ProgressBarControl(false);
                BlockTextInput(true);
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
                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    ProgressBarControl(false);

                    Exceptions.ThrowFullError(ex);

                }
            }
            catch (Exception ex)
            {
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
                await Task.Delay(200);
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text =
                    $"Microsoft Windows [{Globals.FullBuildNumber}]\n" +
                    $"(c) Microsoft Corporation. All rights reserved." +
                    $"\n\n{results}";

                ProgressBarControl(false);
                BlockTextInput(false);

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
                BlockTextInput(true);
                ProgressBarControl(true);
                if (SendCommandText.Text.Contains("%0|%0"))
                {
                    CMDtestText.Text = "Aborting Command Execution, Malicious code found\n\n";

                    await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                    string res = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                    CMDtestText.Text += $"{res}";
                    ProgressBarControl(false);
                    BlockTextInput(false);
                    return;
                }

                if (SendCommandText.Text.Contains("/?"))
                {
                    if (SendCommandText.Text.ToLower() == "reg /?")
                    {
                        
                        var text = CMDtestText.Text;
                        CMDtestText.Text = text.Remove(text.Length - 1);
                        CMDtestText.Text += $"{SendCommandText.Text}\n\n";

                        StorageFile localizationDirectory = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("Assets\\reghelp.txt");
                        string res = File.ReadAllText(localizationDirectory.Path);
                        CMDtestText.Text += $"{res}\n\n";
                        await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                        string cd = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                        CMDtestText.Text += $"{cd}";

                        SendCommandText.Text = "";



                        var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
                        for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                        {
                            object obj = VisualTreeHelper.GetChild(grid, i);
                            if (!(obj is ScrollViewer)) continue;
                            ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                            break;
                        }
                        ProgressBarControl(false);
                        BlockTextInput(false);

                    }
                    else
                    {
                      
                        var text = CMDtestText.Text;
                        CMDtestText.Text = text.Remove(text.Length - 1);
                        CMDtestText.Text += $"{SendCommandText.Text}\n\n";
                        StorageFile tmp = await ApplicationData.Current.LocalFolder.GetFileAsync("cmdstring.txt");

                        string command = SendCommandText.Text;

                       
                        if (command.Length != 0)
                        {
                            CMDtestText.Text += "\n";
                            await client.Send($"{command} > \"{LocalPath}\\cmdstring.txt\"");

                            //await Task.Delay(500);
                            //string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                            string[] resArray = File.ReadAllLines($"{LocalPath}\\cmdstring.txt");
                            await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                            string cd = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                            foreach (string result in resArray)
                            {
                                string results = $"\r\n{result}";
                                CMDtestText.Text += $"\r\n{results}";
                                // await Task.Delay(500);
                            }
                            CMDtestText.Text += $"\n\n{cd}";

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
                        BlockTextInput(false);

                    }
                }
                else
                {

                    
                    var text = CMDtestText.Text;
                    CMDtestText.Text = text.Remove(text.Length - 1);
                    CMDtestText.Text += $"{SendCommandText.Text}\n\n";
                    StorageFile tmp = await ApplicationData.Current.LocalFolder.GetFileAsync("cmdstring.txt");

                    string command = SendCommandText.Text;
                    if (command.Length != 0)
                    {

                        await client.Send($"{command} > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                        //await Task.Delay(500);
                        string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                        //string[] resArray = File.ReadAllLines($"{LocalPath}\\cmdstring.txt");
                        await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                        string cd = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                        //foreach (string results in resArray)
                        //{
                        CMDtestText.Text += $"\n\n{results}\n\n";
                        // await Task.Delay(500);
                        //}
                        CMDtestText.Text += $"{cd}";

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
                    BlockTextInput(false);
                }
            }
            catch (Exception ex)
            {

                Exceptions.ThrowFullError(ex);
                ProgressBarControl(false);
                BlockTextInput(false);
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic commands
                Frame.Navigate(typeof(BasicCommandsPage));
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // clear
                ProgressBarControl(true);
                BlockTextInput(true);

                CMDtestText.Text =
                       $"Microsoft Windows [{Globals.FullBuildNumber}]\n" +
                       $"(c) Microsoft Corporation. All rights reserved." +
                       $"\n\n";
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{results}";
                ProgressBarControl(false);
                BlockTextInput(false);

            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
                ProgressBarControl(false);
                BlockTextInput(false);
            }
        }

        private async void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarControl(true);
                BlockTextInput(true);

                // Load Script
                FileOpenPicker file = new FileOpenPicker();
                file.FileTypeFilter.Add(".bat");
                file.FileTypeFilter.Add(".cmd");
                //file.FileTypeFilter.Add(".exe");

                StorageFile storage = await file.PickSingleFileAsync();
                if (storage == null)
                {
                    ProgressBarControl(false);
                    BlockTextInput(false);

                    return;
                }
                string scriptPath = storage.Path;
                string scriptParent = Path.GetDirectoryName(scriptPath);

                // Check for "fork bomb"

                var tempsript = await storage.CopyAsync(ApplicationData.Current.LocalCacheFolder);

                    string fbomb = File.ReadAllText(tempsript.Path);
                    if (fbomb.Contains("%0|%0"))
                    {
                        CMDtestText.Text = "Aborting Script load, Malicious code found\n\n";
                    await tempsript.DeleteAsync();
                    await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                    string res = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                    CMDtestText.Text += $"{res}";
                    ProgressBarControl(false);
                    BlockTextInput(false);
                    return;
                    }
              
               


                // goto script dir
                await client.Send($"cd /d \"{scriptParent}\" > \"{LocalPath}\\cmdstring.txt\" 2>&1");


                CMDtestText.Text += $"\n\nChanging to Script Directory: \"{scriptParent}\" and executing Script\nPlease Wait\n\n";
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{results}\n\n";



                await client.Send($"\"{storage.Path}\" > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                //string results2 = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                string[] results2 = File.ReadAllLines($"{LocalPath}\\cmdstring.txt");
                foreach (string var in results2)
                {
                    CMDtestText.Text += $"\n{var}";
                }
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string results3 = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"\n\n{results3}\n\n";


                var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
                for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                {
                    object obj = VisualTreeHelper.GetChild(grid, i);
                    if (!(obj is ScrollViewer)) continue;
                    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                    break;
                }
                ProgressBarControl(false);
                BlockTextInput(false);

            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
                ProgressBarControl(false);
                BlockTextInput(false);


            }
        }

        private async void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            try
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
                    client.Disconnect();
                    client.Dispose();
                    Windows.UI.Xaml.Application.Current.Exit();
                }
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }



        private void BtnFlyout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }

        private void ProgressBarControl(bool enable)
        {
            try
            {
                if (enable == true)
                {
                    progbar.IsEnabled = true;
                    progbar.IsIndeterminate = true;
                    progbar.Visibility = Visibility.Visible;
                }
                else
                {
                    progbar.IsEnabled = false;
                    progbar.IsIndeterminate = false;
                    progbar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }


        private void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            try
            {
                Exceptions.CustomMessage(
                    "Command Prompt v0.1.9 for Windows 10 Mobile:\n\n" +
                    "- Thanks to Fadil Fadz for CMD Injector and some help with output formatting.\n" +
                    "- Thanks to BAstifan for the TelnetClient library\n\n" +
                    "Note: Only simple scripts are supported at this time, example is scripts with NO user input\n\n" +
                    "Any Issues? Submit an issue on \"https://github.com/empyreal96/mobile-cmd\""
                    );
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }

        }

        private void BlockTextInput(bool enable)
        {
            try
            {
                if (enable == true)
                {
                    //CMDtestText.IsReadOnly = true;
                    SendCommandText.IsEnabled = false;
                    CMDtestText.IsHitTestVisible = false;
                    SendCommandBtn.IsEnabled = false;
                    //SendCommandText.Focus(FocusState.Programmatic);
                }
                else
                {
                    SendCommandText.IsEnabled = true;
                    // CMDtestText.IsReadOnly = true;
                    CMDtestText.IsHitTestVisible = true;
                    SendCommandBtn.IsEnabled = true;
                    SendCommandText.Focus(FocusState.Programmatic);
                }

            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                bool isInLandscapeMode =
    Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().Orientation ==
    Windows.UI.ViewManagement.ApplicationViewOrientation.Landscape;

                ApplicationData.Current.LocalSettings.Values["Orientation"] = isInLandscapeMode ?
                "Landscape" : "Portrait";


                if (isInLandscapeMode == true)
                {
                    CMDtestText.Height = 240;

                }
                else
                {
                    CMDtestText.Height = 400;
                }
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }

        private async void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            try
            {
                ProgressBarControl(true);
                BlockTextInput(true);
                FolderPicker folder = new FolderPicker();
                folder.FileTypeFilter.Add(".bat");
                folder.FileTypeFilter.Add(".cmd");

                StorageFolder storage = await folder.PickSingleFolderAsync();
                if (storage == null)
                {
                    return;
                }
                string path = storage.Path;
                CMDtestText.Text += $"Moving to Script folder\n";
                await client.Send($"cd /d \"{path}\" > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{results}\n\n";


                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");

                string cd = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text += $"{cd}";

                var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
                for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                {
                    object obj = VisualTreeHelper.GetChild(grid, i);
                    if (!(obj is ScrollViewer)) continue;
                    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                    break;
                }
                ProgressBarControl(false);
                BlockTextInput(false);
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
                ProgressBarControl(false);
                BlockTextInput(false);
            }
        }

        private void MenuFlyoutItem_Click_6(object sender, RoutedEventArgs e)
        {
            try
            {
                Exceptions.CustomMessage(
                    "v0.1.9 Changelog:\n\n" +
                    "- Added check for some Malicious code being ran\n" +
                    "- Onscreen Keyboard improvements\n" +
                    ""
                    );
            }
            catch (Exception ex)
            {
                Exceptions.ThrowFullError(ex);
            }
        }
    }
}
