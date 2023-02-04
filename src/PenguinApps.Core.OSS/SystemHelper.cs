using Windows.ApplicationModel.DataTransfer;

namespace MobileTerminal.PenguinApps.Core.OSS
{
    public class SystemHelper
    {
        public static void CopyStringToClipboard(string text)
        {
            DataPackage dataPackage = new DataPackage
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            dataPackage.SetText(text);
            Clipboard.SetContent(dataPackage);
        }
    }
}
