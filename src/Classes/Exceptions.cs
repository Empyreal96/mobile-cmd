using System;
using Windows.UI.Popups;

namespace MobileTerminal.Classes
{
    class Exceptions
    {
        public static async void ThrowFullError(Exception ex)
        {
            var ThrownException = new MessageDialog($"{ex.Message}\n\n{ex.Source}\n\n{ex.ToString()}\n\n{ex.StackTrace}");
            ThrownException.Commands.Add(new UICommand("Close"));
            await ThrownException.ShowAsync();
        }

        public static async void CustomMessage(String ex)
        {
            var ThrownException = new MessageDialog(ex);
            ThrownException.Commands.Add(new UICommand("Close"));
            await ThrownException.ShowAsync();
        }
    }
}
