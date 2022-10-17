using System.IO;

namespace MobileTerminal.Classes
{
    class Tools
    {
        public static bool CheckForMaliciousCommand(string command)
        {
            if (command.Contains("\\.\\globalroot\\device\\condrv\\kernelconnect") || command.Contains("%0|%0"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsFileAccessable(string file)
        {
            try
            {
                using (var stream = File.OpenRead(file))
                    return false;
            }
            catch (IOException)
            {
                return true;
            }
        }
    }
}