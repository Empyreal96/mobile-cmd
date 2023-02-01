using System;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace MobileTerminal.Classes
{
    class RuntimeManager
    {
        public static async void ExitApp()
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = "Close app?",
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
