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
    public partial class MainWindow
    {
        Program2 ftpConnection = new Program2();
        public MainWindow()
        { 
            InitializeComponent();

            /*
            Contents dirContents = new Contents("", "");

            List<Item> dirItems = dirContents.GetItems();

            string test = "";
            for (int i = 0; i < dirItems.Count; ++i )
            {
                test += dirItems[i].FileName + " (" + dirItems[i].FileSize + ")\r\n";
            }

            
            ObservableCollection<remoteItem> remoteDirectoryList = new ObservableCollection<remoteItem>();
            remoteDirectories dirObject = new remoteDirectories();
            dirObject.populateList(remoteDirectoryList);
            */

            getRootDirectories();

            getFTPRootDirectory();

            //RemoteDirectoryItem directory = getRemoteDirectory("/", "new");

            //ServerDirectoryBrowser.ItemsSource = directory.RemoteDirectoryItems;
        }

        private void getFTPRootDirectory()
        {
            /*
            Contents dirContents = new Contents("", "");

            List<Item> dirItems = dirContents.GetItems();

            RemoteDirectoryItem rootDir = new RemoteDirectoryItem() { Name = "", Path = "/" };

            Debug.WriteLine("************************************FSDFSDF*SDFSFSDFSDF**");

            foreach (Item item in dirItems)
            {
                RemoteDirectoryItem ftpRootItem = new RemoteDirectoryItem() { Name = item.FileName, IsDirectory = item.Directory };
                rootDir.RemoteDirectoryItems.Add(ftpRootItem);
                //Debug.WriteLine(ftpRootItem.Name + "," + ftpRootItem.IsDirectory);
            }
            */

            RemoteDirectoryItem rootDir = getRemoteDirectory("", "");

            ServerDirectoryBrowser.ItemsSource = rootDir.RemoteDirectoryItems;
        }

        //Gets remote directory contents of a directory and then creates and returns a RemoteDirectoryItem
        //
        private RemoteDirectoryItem getRemoteDirectory(string path, string name)
        {

            Contents dirConents = new Contents(path, name);

            List<Item> dirItems = dirConents.GetItems();

            RemoteDirectoryItem directory = new RemoteDirectoryItem() {Name = name};
            
            foreach (Item item in dirItems)
            {
                Debug.WriteLine("Contents Item Name:    " + item.FileName);
                string itemPath = path + "/" + name;

                RemoteDirectoryItem ftpItem = new RemoteDirectoryItem() { Name = item.FileName, IsDirectory = item.Directory, Path = itemPath };
                directory.RemoteDirectoryItems.Add(ftpItem);

                Debug.WriteLine("Remote Dir Item Name:  " + ftpItem.Name + "," + ftpItem.Path);
            }

            return directory;
        }


        //Gets the root directories on the local machine and add them to the local directory browser tree
        private void getRootDirectories()
        {

            DriveInfo[] allDrives = DriveInfo.GetDrives();

            DirectoryItem rootDir = new DirectoryItem() { Name = "Drives" }; 

            foreach (DriveInfo drive in allDrives)
            {
                if(drive.DriveType != DriveType.NoRootDirectory && drive.IsReady)
                {
                    DirectoryItem driveRoot = new DirectoryItem() { Name = drive.Name, Path = drive.RootDirectory.ToString() };
                    rootDir.DirectoryItems.Add(driveRoot);
                    getDirectories(driveRoot.Path, driveRoot);
                    getFiles(driveRoot.Path, driveRoot);
                }
            }
            localDirectoryBrowser.Items.Add(rootDir);
        }

        //Gets local files i na specified path and adds them to the sepcified parent directory
        private void getFiles(string path, DirectoryItem parent)
        {
            var files = Directory.EnumerateFiles(path);

            foreach (string file in files)
                parent.DirectoryItems.Add(new DirectoryItem() { Name = Path.GetFileName(file), Path = file });
        }

        //Gets local directories in a specified path and adds them to the sepcified parent directory
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
            }
        }

        //Checks if a local directory is empty
        private bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        //Handles the event when the treeview item in the local directory tree browser is exanded
        private void TreeViewItemExpanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem treeItem = e.OriginalSource as TreeViewItem;
            DirectoryItem directory = treeItem.Header as DirectoryItem;

            if (directory.Path != null)
            {
                directory.DirectoryItems.Clear();
                getDirectories(directory.Path, directory);
                getFiles(directory.Path, directory);
            }
        }


        //Handles clicking events in the listbox that displays the remote directory items
        private void remoteDirBrowser_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if(ServerDirectoryBrowser.SelectedItem != null)
            {
                //Debug.WriteLine(ServerDirectoryBrowser.SelectedItem.ToString());

                RemoteDirectoryItem directoryItem = ServerDirectoryBrowser.SelectedItem as RemoteDirectoryItem;

                Debug.WriteLine(directoryItem.Path);
                
                //Debug.WriteLine(dir0.ectory.Name + "," + directory.IsDirectory);

                if(directoryItem.IsDirectory)
                {
                    //Contents dirContents = new Contents("", directoryItem.Name);
                    RemoteDirectoryItem directory = getRemoteDirectory(directoryItem.Path, directoryItem.Name);

                    ServerDirectoryBrowser.ItemsSource = directory.RemoteDirectoryItems;
                    
                }
            }
        }

        private void createDirectory_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("I just got clicked out!");

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

    /*
    //This is how I learned Binding it may be (and probably is) possible to write it
    //  like Scott did above.

    //Class to create and give the directory ObservableCollection
    public class remoteDirectories
    {
        public ObservableCollection<remoteItem> getDirectories()
        {
            ObservableCollection<remoteItem> dirList = new ObservableCollection<remoteItem>();
            populateList(dirList);
            return dirList;
        }

        //This may be streamlined by moving it directly into Dane's GetItems() method, but for now it works
        public void populateList(ObservableCollection<remoteItem> dirList)
        {
            Contents dirContents = new Contents("", "");

            List<Item> dirItems = dirContents.GetItems();

            for (int i = 0; i < dirItems.Count; ++i)
            {
                dirList.Add(new remoteItem { Name=dirItems[i].FileName, Type=dirItems[i].FileType});
            }
        }
    }

    //My own item for remote connection. This should really be one and the same with Scott's item, but his initialized
    //  an ObservableCollection and that is not how I learned to do it. Scott and I need to work out a way so that we
    //  only have one Item class
    public class remoteItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    */

    //Class used for local storing and displaying of FTP server directories and files
    public class RemoteDirectoryItem
    {
        public RemoteDirectoryItem()
        {
            this.RemoteDirectoryItems = new ObservableCollection<RemoteDirectoryItem>();
        }

        //Name of this directory item
        public string Name { get; set; }

        public bool IsDirectory { get; set; }

        //This is the path to the directory of this item -- not the actual item. Actual path is: Path + Name.
        public string Path { get; set; }

        //A list of the files and directories in this directory
        public ObservableCollection<RemoteDirectoryItem> RemoteDirectoryItems { get; set; }
    }
}
