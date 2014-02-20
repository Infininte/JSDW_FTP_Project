using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Forms;                     // *** delete this

namespace FTPBoss
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //System.Windows.DialogBox dialogBox = new DialogBox();

            Contents dirContents = new Contents("", "");

            List<Item> dirItems = dirContents.GetItems();

            // ListView Object
            // ListView.Columns.Add("FileName"); // ("Filesize");

            // ListView.Row.Add["FileName"](dirItems[i].FileName);

            string test = "";
            for (int i = 0; i < dirItems.Count; ++i )
            {
                test += dirItems[i].FileName + " (" + dirItems[i].FileSize + ")\r\n";
            }


                // Delete reference: System.Windows.Forms
                System.Windows.Forms.MessageBox.Show(test);

            //Program2.GetRequest(2, );
            //FTPBoss.Item RemoteItem = new FTPBoss.Item();

            getRootDirectories();
        }

        private void getRootDirectories()
        {

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            DirectoryItem rootDir = new DirectoryItem() { Name = "Drives" }; 

            foreach (DriveInfo drive in allDrives)
            {
                if(drive.DriveType != DriveType.NoRootDirectory && drive.IsReady)
                {
                    //Debug.WriteLine(drive.Name);
                    //Debug.WriteLine(drive.DriveType);
                    //Debug.WriteLine(drive.RootDirectory);
                    DirectoryItem driveRoot = new DirectoryItem() { Name = drive.Name, Path = drive.RootDirectory.ToString() };
                    rootDir.DirectoryItems.Add(driveRoot);
                    getDirectories(driveRoot.Path, driveRoot);
                    getFiles(driveRoot.Path, driveRoot);
                }
            }
            localDirectoryBrowser.Items.Add(rootDir);
        }

        private void getFiles(string path, DirectoryItem parent)
        {
            var files = Directory.EnumerateFiles(path);

            foreach (string file in files)
                parent.DirectoryItems.Add(new DirectoryItem() { Name = Path.GetFileName(file), Path = file });
        }

        private void getDirectories(string path, DirectoryItem parent)
        {
            var directories = Directory.EnumerateDirectories(path);

            foreach (string directory in directories)
            {
                DirectoryItem newDirectory = new DirectoryItem() { Name= Path.GetFileName(directory), Path = directory};

                try
                {
                    if (!IsDirectoryEmpty(directory))
                        newDirectory.DirectoryItems.Add(new DirectoryItem { Name = " ", Path = " " });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                

                parent.DirectoryItems.Add(newDirectory);
                //Debug.WriteLine(directory);
            }
        }

        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private void TreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(e.OriginalSource.ToString());
            DirectoryItem directory = e.OriginalSource as DirectoryItem;


            if(directory != null)
            {
                Console.WriteLine(directory.Name);
            }
        }

    }

    public class DirectoryItem
    {
        public DirectoryItem()
        {
            this.DirectoryItems = new ObservableCollection<DirectoryItem>();
        }

        public string Name { get; set; }

        public string Path { get; set; }

        public ObservableCollection<DirectoryItem> DirectoryItems { get; set; }
    }

    public class Remote
    {
        public void AutoConnect()
        {
            
        }
    }
}
