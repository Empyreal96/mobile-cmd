using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telnet;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Command_Prompt
{
    public sealed partial class MainPage : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        string LocalPath = ApplicationData.Current.LocalFolder.Path;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient client = new TelnetClient(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
        public static string CMDCommandText { get; set; }

        public MainPage()
        {
            try
            {
                this.InitializeComponent();
                GetSettings();
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
                BlockTextInput(true);
                CMDtestText.Text = $"Connecting to device...";
                await client.Connect();
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string results = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text =
                    $"mobile-cmd v0.2-beta\n" +
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
        
        private void GetSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string Font = localSettings.Values["Font"] as string;
            if (Font != null)
            {
                CMDtestText.FontFamily = new FontFamily(Font);
            }
        }
        private void SendCommandBtn_Click(object sender, RoutedEventArgs e)
        {
            // Prevent two commands running at the same time in different tabs
            string CommandRunning = localSettings.Values["CommandRunning"] as string;
            if (CommandRunning == "true")
            {
                Exceptions.CustomMessage("You cannot run two commands at the same time");
            }
            else
            {
                // Change "CommandRunning" to "true" to prevent another command from running
                localSettings.Values["CommandRunning"] = "true";
                // Block text input
                BlockTextInput(true);
                // Show loading control
                ProgressBarControl(true);
                var text = CMDtestText.Text;
                CMDtestText.Text = text.Remove(text.Length - 2);
                // Write command to UI
                CMDtestText.Text += $"{SendCommandText.Text}\n";
                // Check if command is malicious
                CheckForMaliciousCommand();
                // Clear Text input
                SendCommandText.Text = "";
                // Hide loading control
                ProgressBarControl(false);
                // Remove text input block
                BlockTextInput(false);
                // Change "CommandRunning" to false, since the command now finish running
                localSettings.Values["CommandRunning"] = "false";
            }
        }

        private async void CheckForMaliciousCommand()
        {
            if (SendCommandText.Text.Contains("\\.\\globalroot\\device\\condrv\\kernelconnect") || SendCommandText.Text.Contains("%0|%0"))
            {
                CMDtestText.Text += "Command aborted. The command you tried to run is malicious";
                await ShowCurrentPath();
                ScrollToBottom();
            }
            else
            {
                SendCommand();
            }
        }

        private async void SendCommand()
        {
            string command = SendCommandText.Text;
            if (command.Length != 0)
            {
                if (command == "cls")
                {
                    // Get current path from device
                    await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                    // Give the command some time to process
                    await Task.Delay(150);
                    // Get current path output from file
                    string currentpath = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                    // Write current path to UI
                    CMDtestText.Text = $"{currentpath}";
                }
                else
                {
                    // send command
                    await client.Send($"{command} > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                    // Give the command some time to process
                    await Task.Delay(1000);
                    GetOutput();
                }
            }
        }

        private async void GetOutput()
        {
            try
            {
                // Append command output to TextBox
                CMDtestText.Text += "\n" + File.ReadAllText($"{LocalPath}\\cmdstring.txt");
            }
            catch
            {
                // If command fails "cmdstring.txt" is still in use becouse the command isnt done thus it needs to retry reading the txt
                GetOutput();
            }
            await ShowCurrentPath();
            ScrollToBottom();
        }

        private async Task ShowCurrentPath()
        {
            // Show current path
            await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
            string currentpath = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
            CMDtestText.Text += "\n\n" + $"{currentpath}";
        }

        private void ScrollToBottom()
        {
            // Scroll to bottom to show results
            var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
            for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
            {
                object obj = VisualTreeHelper.GetChild(grid, i);
                if (!(obj is ScrollViewer)) continue;
                ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                break;
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

        private void BlockTextInput(bool enable)
        {
            try
            {
                if (enable == true)
                {
                    SendCommandText.IsEnabled = false;
                    CMDtestText.IsHitTestVisible = false;
                    SendCommandBtn.IsEnabled = false;
                }
                else
                {
                    SendCommandText.IsEnabled = true;
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
    }
}