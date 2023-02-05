using System;
using System.Threading.Tasks;

namespace MobileTerminal.Classes;

public class FileHelper
{
    public static async Task DeleteLocalFile(string fileName)
    {
        var file = await Globals.localFolder.TryGetItemAsync(fileName);
        if (file != null)
        {
            await file.DeleteAsync();
        }
    }
}