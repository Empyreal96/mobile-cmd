﻿using MobileTerminal.Classes;
using PenguinApps.Core.OSS;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MobileTerminal.Pages
{
    public sealed partial class Terminal : Page
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        string LocalPath = ApplicationData.Current.LocalFolder.Path;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient client = new TelnetClient(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
        public Terminal()
        {
            this.InitializeComponent();
            ApplicationData.Current.LocalFolder.CreateFileAsync("cmdstring.txt", CreationCollisionOption.ReplaceExisting);
            try
            {
                GetSettings();
                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    ExceptionHelper.ThrowFullError(ex);

                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.ThrowFullError(ex);
            }
        }

        public async void Connect()
        {
            try
            {
                BlockTextInput(true);
                CMDtestText.Text = $"Connecting to device...";
                await client.Connect();
                await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                string currentpath = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                CMDtestText.Text =
                    $"Mobile Terminal v0.4 (Hybrid mode)\n" +
                    $"\n{currentpath}";
                BlockTextInput(false);
                if (Globals.LaunchCommand != null)
                {
                    ContentDialog dialog = new ContentDialog
                    {
                        Title = "Execute command?",
                        Content = "A application requested to execute this code:" + Environment.NewLine + Globals.LaunchCommand + Environment.NewLine + "Do NOT execute malicious code or you might break your device!",
                        PrimaryButtonText = "No",
                        SecondaryButtonText = "Yes, I understand the risks",
                        DefaultButton = ContentDialogButton.Primary
                    };

                    var result = await dialog.ShowAsync();
                    if (result == ContentDialogResult.Secondary)
                    {
                        SendCommand(Globals.LaunchCommand);
                    }
                    Globals.LaunchCommand = null;
                }
            }
            catch (Exception ex)
            {
                ExceptionHelper.ThrowFullError(ex);
            }
        }

        private void GetSettings()
        {
            if (localSettings.Values["HideSendBtn"] as string == "true")
            {
                SendCommandBtn.Visibility = Visibility.Collapsed;
            }
            if (localSettings.Values["Font"] is string Font)
            {
                CMDtestText.FontFamily = new FontFamily(Font);
                SendCommandText.FontFamily = new FontFamily(Font);
            }
        }
        private void SendCommandText_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var textbox = sender as TextBox;
                SendCommandText.Focus(FocusState.Programmatic);
                SendCommand(textbox.Text);
            }
        }
        private void SendCommandBtn_Click(object sender, RoutedEventArgs e)
        {
            SendCommand(SendCommandText.Text);
        }

        private async void SendCommand(string command)
        {
            // Prevent two commands running at the same time in different tabs
            if (Globals.CommandRunning)
            {
                await UI.ShowDialog("Error", "You cannot run two commands at the same time");
            }
            else
            {
                // Change "CommandRunning" to "true" to prevent another command from running
                Globals.CommandRunning = true;
                // Block text input
                BlockTextInput(true);
                // Clear text input
                ClearTextInput();
                var text = CMDtestText.Text;
                CMDtestText.Text = text.Remove(text.Length - 2);
                // Write command to UI
                CMDtestText.Text += $"{command}\n";
                // Check if command is malicious and if it isnt run the command
                bool IsCommandMalicious = Tools.CheckForMaliciousCommand(command);
                if (!IsCommandMalicious)
                {
                    if (command.Length != 0)
                    {
                        if (command == "cls")
                        {
                            CMDtestText.Text = "";
                            await ShowCurrentPath();
                        }
                        if (command == "exit")
                        {
                            RuntimeManager.ExitApp();
                            await ShowCurrentPath();
                        }
                        else
                        {
                            if (!command.StartsWith("PS "))
                            {
                                // Run CMD command
                                await client.Send($"{command} > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                            }
                            else
                            {
                                // Run PS command
                                int n = 3;
                                string pwshcommand = command.Remove(0, n);
                                await client.Send($"PowerShell.exe -Command {pwshcommand} > \"{LocalPath}\\cmdstring.txt\" 2>&1");
                            }
                            TestIfFileAccessable();
                        }

                        Json.AddItemToJson("History.json", command, "2023");
                    }
                }
                else
                {
                    CMDtestText.Text += "Command aborted. The command you tried to run is malicious";
                }
                // Change "CommandRunning" to false, since the command now finish running
                Globals.CommandRunning = false;
            }
        }

        private async void TestIfFileAccessable()
        {
            bool IsFileUsed = Tools.IsFileAccessable($"{LocalPath}\\cmdstring.txt");
            if (IsFileUsed == false)
            {
                GetOutput();
            }
            else
            {
                await Task.Delay(1000);
                TestIfFileAccessable();
            }
        }

        private async void GetOutput()
        {
            try
            {
                // Append command output to TextBox
                CMDtestText.Text += "\n" + File.ReadAllText($"{LocalPath}\\cmdstring.txt");
                await ShowCurrentPath();
                // Scroll to bottom to show results
                var grid = (Grid)VisualTreeHelper.GetChild(CMDtestText, 0);
                for (var i = 0; i <= VisualTreeHelper.GetChildrenCount(grid) - 1; i++)
                {
                    object obj = VisualTreeHelper.GetChild(grid, i);
                    if (!(obj is ScrollViewer)) continue;
                    ((ScrollViewer)obj).ChangeView(0.0f, ((ScrollViewer)obj).ExtentHeight, 1.0f);
                    break;
                }
                // Remove text input block
                BlockTextInput(false);
            }
            catch (Exception ex)
            {
                ExceptionHelper.ThrowFullError(ex);
            }
        }

        private async Task ShowCurrentPath()
        {
            // Show current path
            await client.Send($"echo %CD%^> > \"{LocalPath}\\cmdstring.txt\" 2>&1");
            string currentpath = File.ReadAllText($"{LocalPath}\\cmdstring.txt");
            CMDtestText.Text += "\n\n" + $"{currentpath}";
        }

        private void BlockTextInput(bool enable)
        {
            try
            {
                if (enable == true)
                {
                    SendCommandText.PlaceholderText = "Processing...";
                    CMDtestText.IsHitTestVisible = false;
                    SendCommandBtn.IsEnabled = false;
                }
                else
                {
                    SendCommandText.PlaceholderText = "Enter command here";
                    CMDtestText.IsHitTestVisible = true;
                    SendCommandBtn.IsEnabled = true;
                    SendCommandText.Focus(FocusState.Programmatic);
                }

            }
            catch (Exception ex)
            {
                ExceptionHelper.ThrowFullError(ex);
            }
        }

        private void ClearTextInput()
        {
            // Clear Text input
            SendCommandText.Text = "";
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int currentheight = Convert.ToInt32(((Frame)Window.Current.Content).ActualHeight);
            CMDtestText.Height = Convert.ToDouble(currentheight - 75);
        }
    }
}
