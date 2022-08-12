using System;
using System.Threading;
using Telnet;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Command_Prompt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRunPage : Page
    {
        bool IsCMDPresent;
       
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        TelnetClient client = new TelnetClient(TimeSpan.FromSeconds(3), cancellationTokenSource.Token);
      

        public FirstRunPage()
        {
            this.InitializeComponent();

            

            bool isDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;
            if (isDark == false)
            {
                Color color = Colors.LightGray;
                SolidColorBrush lightBrush = new SolidColorBrush(color);
                //#FF686868
                LoopText.Background = lightBrush;
                LoopText.BorderBrush = lightBrush;
                LoopText.RequestedTheme = ElementTheme.Default;
                InfoText.Background = lightBrush;
                InfoText.BorderBrush = lightBrush;

            }else
            {
                HeaderBorder.Background = new SolidColorBrush(Colors.DimGray);
            }



            progbar.IsEnabled = true;

            CMDpresent.Text = "Checking for CMD access, please wait...";
            try
            {
               
                Connect();
                progbar.IsEnabled = false;
            }
            catch (Exception ex)
            {
                progbar.IsEnabled = false;

                Exceptions.ThrowFullError(ex);
            }

        }

        private async void Connect()
        {
            try
            {

                await client.Connect();
              //  await Task.Delay(1000);
                await client.Send($"set");
                IsCMDPresent = true;

            }
            catch (Exception ex)
            {
                IsCMDPresent = false;
            }

            if (IsCMDPresent == true)
            {
                CMDpresent.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                CMDpresent.Text = "CMD Access Found!";
                FinishBtn.IsEnabled = true;
                progbar.IsEnabled = false;
                progbar.IsIndeterminate = false;

            }
            else
            {
                CMDpresent.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                CMDpresent.Text = "CMD Access Not Found!";

                progbar.IsEnabled = false;
                progbar.IsIndeterminate = false;


            }
            progbar.IsEnabled = false;
        }
        string LocalPath = ApplicationData.Current.LocalFolder.Path;
       
       
        IPropertySet roamingProperties = ApplicationData.Current.RoamingSettings.Values;
        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            //await ApplicationData.Current.LocalFolder.CreateFileAsync("FirstRunComplete.txt", CreationCollisionOption.ReplaceExisting);
            roamingProperties["FirstRunDone"] = bool.TrueString;
            this.Frame.Navigate(typeof(TabsPage));
        }

        private void LoopCmd_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            string command = "checknetisolation loopbackexempt -a -n=WPCommandPrompt_g5rj6pc6gbtrg";
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(command);
            Clipboard.SetContent(dataPackage);
            Exceptions.CustomMessage("'checknetisolation loopbackexempt -a -n=WPCommandPrompt_g5rj6pc6gbtrg' copied to clipboard");
        }
    }
}
