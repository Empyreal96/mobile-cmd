using MobileTerminal.Classes;
using PenguinApps.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace MobileTerminal.Pages;

public sealed partial class Terminal : Page
{
    private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    string LocalPath = ApplicationData.Current.LocalFolder.Path;
    static CancellationTokenSource cancellationTokenSource = new();
    TelnetClient client = new(TimeSpan.FromSeconds(1), cancellationTokenSource.Token);
    string TempFileName = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-temp.txt";
    public Terminal()
    {
        this.InitializeComponent();
        
        try
        {
            ApplicationData.Current.LocalFolder.CreateFileAsync(TempFileName, CreationCollisionOption.ReplaceExisting);
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

    public async void DeleteTempFile()
    {
        StorageFile TempFile = await ApplicationData.Current.LocalFolder.GetFileAsync(TempFileName);
        await TempFile.DeleteAsync();
    }

    public async void Connect()
    {
        try
        {
            BlockTextInput(true);
            CMDtestText.Text = $"Connecting to device...";
            await client.Connect();
            await client.Send($"echo %CD%^> > \"{LocalPath}\\{TempFileName}\" 2>&1");
            string currentpath = File.ReadAllText($"{LocalPath}\\{TempFileName}");
            CMDtestText.Text =
                $"Mobile Terminal [Version " + AppVersion.GetAppVersion() + " Beta]\n" +
                $"\n{currentpath}";
            BlockTextInput(false);
            if (Globals.LaunchCommand != null)
            {
                ContentDialog dialog = new()
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
        if (localSettings.Values["FontSize"] is string FontSize)
        {
            CMDtestText.FontSize = double.Parse(FontSize);
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
        // Block text input
        BlockTextInput(true);
        // Clear text input
        ClearTextInput();
        var text = CMDtestText.Text;
        CMDtestText.Text = text.Remove(text.Length - 2);
        // Write command to UI
        CMDtestText.Text += $"{command}\n";
        if (Tools.IsCommandMalicious(command))
        {
            CMDtestText.Text += "Command aborted. The command you tried to run is malicious";
            await ShowCurrentPath();
            BlockTextInput(false);
            return;
        }
        if (command.Length != 0)
        {
            if (command == "cls")
            {
                CMDtestText.Text = "";
                await ShowCurrentPath();
            }
            if (command == "exit")
            {
                DeleteTempFile();
                CoreApplication.Exit();
            }
            else
            {
                if (!command.StartsWith("PS "))
                {
                    // Run CMD command
                    await client.Send($"{command} > \"{LocalPath}\\{TempFileName}\" 2>&1");
                }
                else
                {
                    // Run PS command
                    int n = 3;
                    string pwshcommand = command.Remove(0, n);
                    await client.Send($"PowerShell.exe -Command {pwshcommand} > \"{LocalPath}\\{TempFileName}\" 2>&1");
                }
                TestIfFileAccessable();
            }
            BlockTextInput(false);
            Json.AddItemToJson("History.json", command, DateTime.Now.ToString());
        }
    }

    private async void TestIfFileAccessable()
    {
        bool IsFileUsed = Tools.IsFileAccessable($"{LocalPath}\\{TempFileName}");
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
            CMDtestText.Text += "\n" + File.ReadAllText($"{LocalPath}\\{TempFileName}");
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
        await client.Send($"echo %CD%^> > \"{LocalPath}\\{TempFileName}\" 2>&1");
        string currentpath = File.ReadAllText($"{LocalPath}\\{TempFileName}");
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
}
