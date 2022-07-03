using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Command_Prompt
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
