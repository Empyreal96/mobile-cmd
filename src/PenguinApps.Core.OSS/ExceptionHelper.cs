using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace PenguinApps.Core.OSS
{
    public class ExceptionHelper
    {
        public static async void ThrowFullError(Exception ex)
        {
            var ThrownException = new MessageDialog($"{ex.Message}\n\n{ex.Source}\n\n{ex}\n\n{ex.StackTrace}");
            ThrownException.Commands.Add(new UICommand("Close"));
            await ThrownException.ShowAsync();
        }
    }
}
