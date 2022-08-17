using System;
using System.IO;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace MobileTerminal.Classes
{
    class Tools
    {
        //Go Back Handler
        public static EventHandler GoBackHandler;

        public static bool IsFileAccessable(string file)
        {
            try
            {
                using (var stream = File.OpenRead(file))
                    return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        public static bool CheckForMaliciousCommand(string command)
        {
            if (command.Contains("\\.\\globalroot\\device\\condrv\\kernelconnect") || command.Contains("%0|%0"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async void ShowDialog(string title, string content)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = "Okay",
                DefaultButton = ContentDialogButton.Primary
            };

            await dialog.ShowAsync();
        }

        public static async void ExitApp()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Question",
                Content = "Do you really want to exit Terminal?",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                DefaultButton = ContentDialogButton.Primary
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                CoreApplication.Exit();
            }
        }
    }
}
