using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Command_Prompt
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BasicCommandsPage : Page
    {
        public BasicCommandsPage()
        {
            this.InitializeComponent();
            cdSyntax.Text = 
                "CD [/D] (drive:)(path)\n" +
                "CD /D C:\\Users\\Data\\Public";
            dirSyntax.Text =
                            "DIR (drive:)(path)(filename)\n" +
                            "DIR C:\\Windows\n" +
                            "DIR C:\\Windows\\*.exe\n" +
                            "DIR";
            setSyntax.Text =
                    "SET (variable=(string))\n" +
                    "SET \"MyNewVariable=0000001\"\n" +
                    "SET \"PATH=%PATH%;C:\\Users\"\n" +
                    "SET";
            copySyntax.Text =
                "XCOPY (source) (destination)\n" +
                "COPY (source) (destination)" +
                "XCOPY .\\MyFile.txt C:\\Windows\\Temp\n" +
                "COPY .\\MyFile.txt \"C:\\Program Files\"";
            mkdirSyntax.Text =
                "MKDIR (drive:)(path)\n" +
                "MKDIR C:\\MyNewFolder";
            renameSyntax.Text =
                "RENAME (drive:)(path)filename1 filename2\n" +
                "RENAME C:\\MyFolder\\File1.txt NewFileName.txt";
            moveSyntax.Text =
                "MOVE (drive:)(path)filename1[,filename2] destination\n" +
                "MOVE C:\\MyFolder\\File1.txt C:\\Windows\\Temp\n" +
                "MOVE C:\\MyFolder\\File1.txt,File2.txt C:\\Windows\\Temp";
            rmdirSyntax.Text =
                "RMDIR [/S] (drive:)(path)\n" +
                "RMDIR C:\\MyFolder\n" +
                "RMDIR /S C:\\MyFolder\n\n" +
                "/S removes all containing files and directories";
            typeSyntax.Text =
                "TYPE (drive:)(path)filename\n" +
                "TYPE C:\\MyFolder\\File1.txt";
        }

        private void cmdsBackBtn_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
