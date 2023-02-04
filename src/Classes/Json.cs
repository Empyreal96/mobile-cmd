using static MobileTerminal.Classes.Globals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;

namespace MobileTerminal.Classes;

public static class Json
{
    public static async void CreateJsonFile(string file, string command, string executiontimedate)
    {
        // Generate json
        string json = "[{\"command\":\"" + command + "\"," + "\"executiontimedate\":\"" + executiontimedate + "\"}]";
        // create json file
        await localFolder.CreateFileAsync(file, CreationCollisionOption.ReplaceExisting);
        // get json file
        var fileData = await ApplicationData.Current.LocalFolder.GetFileAsync(file);
        // write json to json file
        await FileIO.WriteTextAsync(fileData, json);
    }

    public static async void AddItemToJson(string file, string command, string executiontimedate)
    {
        var fileData = await localFolder.TryGetItemAsync(file);
        if (fileData == null) CreateJsonFile(file, command, executiontimedate);
        else
        {
            // get json file content
            string json = await FileIO.ReadTextAsync((IStorageFile)fileData);
            // new historyitem
            JsonItems newHistoryitem = new()
            {
                Command = command,
                ExecutionTimeDate = executiontimedate
            };
            // Convert json to list
            List<JsonItems> historylist = JsonConvert.DeserializeObject<List<JsonItems>>(json);
            // Add new historyitem
            historylist.Insert(0, newHistoryitem);
            // Convert list to json
            string newJson = JsonConvert.SerializeObject(historylist);
            // Write json to json file
            await FileIO.WriteTextAsync((IStorageFile)fileData, newJson);
        }
    }


}
