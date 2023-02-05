using MobileTerminal.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static MobileTerminal.Classes.Globals;

namespace MobileTerminal.Pages
{
    public sealed partial class History : Page
    {
        public History()
        {
            this.InitializeComponent();
            LoadHistory();
        }

        private async void LoadHistory()
        {
            try
            {
                StorageFile fileData = (StorageFile)await Globals.localFolder.GetItemAsync("History.json");
                string categoriesfilecontent = await FileIO.ReadTextAsync(fileData);
                JsonItemsList = JsonConvert.DeserializeObject<List<JsonItems>>(categoriesfilecontent);
                if (JsonItemsList != null) HistoryListView.ItemsSource = JsonItemsList;
            }
            catch
            {
                await UI.ShowDialog("Error", "Command history is empty!");
            }
        }

        private async void HistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get listview sender
            ListView listView = (ListView)sender;
            if (listView.ItemsSource != null)
            {
                // Get selected item
                JsonItems item = (JsonItems)listView.SelectedItem;
                SystemHelper.CopyStringToClipboard(item.Command);
                await UI.ShowDialog("Success", "Copied command to keyboard!");
            }
        }

        private void HistorySearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            TextBox searchbox = (TextBox)sender;
            // Get all ListView items with the entered search query
            var SearchResults = from s in JsonItemsList where s.Command.Contains(searchbox.Text) select s;
            // Set SearchResults as ItemSource for Applist
            HistoryListView.ItemsSource = SearchResults;
        }
    }
}
