using MobileTerminal.Classes;
using MobileTerminal.PenguinApps.Core.OSS;
using Newtonsoft.Json;
using PenguinApps.Core.OSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Gaming.Preview.GamesEnumeration;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static MobileTerminal.Classes.Globals;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MobileTerminal.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommandHistory : Page
    {
        public CommandHistory()
        {
            this.InitializeComponent();
            LoadHistory();
        }

        private async void LoadHistory()
        {
            StorageFile fileData = (StorageFile)await Globals.localFolder.GetItemAsync("History.json");
            string categoriesfilecontent = await FileIO.ReadTextAsync(fileData);
            JsonItemsList = JsonConvert.DeserializeObject<List<JsonItems>>(categoriesfilecontent);
            if (JsonItemsList != null) CommandHistoryListView.ItemsSource = JsonItemsList;
            else
            {
                CommandHistoryListView.ItemsSource = null;
            }
        }

        private async void CommandHistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get listview sender
            ListView listView = sender as ListView;
            if (listView.ItemsSource != null)
            {
                // Get selected item
                JsonItems item = (JsonItems)listView.SelectedItem;
                SystemHelper.CopyStringToClipboard(item.Command);
                await UI.ShowDialog("Success", "Copied command to keyboard!");
            }
        }

        private void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            
        }
    }
}
