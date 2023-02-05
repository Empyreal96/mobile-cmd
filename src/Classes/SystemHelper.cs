using Windows.ApplicationModel.DataTransfer;

namespace MobileTerminal.Classes;

public class SystemHelper
{
    public static void CopyStringToClipboard(string text)
    {
        DataPackage dataPackage = new()
        {
            RequestedOperation = DataPackageOperation.Copy
        };
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);
    }
}