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
using System.ComponentModel;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Forms;                     // *** delete this
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace FTPBoss
{
    public class Nav
    {
        private List<string> nav = null;
        public Nav() { nav = new List<string>(); }
        public void AddRoot() { nav.Add(""); }
        public void Add(string dirName) { nav.Add(dirName); }
        public void Remove() { nav.RemoveAt(nav.Count() - 1); }
        public string ToString(string separator = "/")
        {
            string output = string.Join(separator, nav);
            return output;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static BackgroundWorker bgwU = new BackgroundWorker();
        public static BackgroundWorker bgwD = new BackgroundWorker();

        public static Nav navigation = new Nav();



        public MainWindow()
        { 
            InitializeComponent();
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            Program2.credProfiles = new CredentialProfiles();
            Program2.credProfiles.LoadFromFile(Program2.CredentialFile);
            //Program2.credProfiles.Add("Westfall", "drwestfall.net", "ftp04", "project", "21");
            //Program2.credProfiles.Add("bugs3", "pftp.bugs3.com", "u631161179.ftp", "testftp1", "21");
            //Program2.credProfiles.SaveToFile(Program2.CredentialFile);

            // Nav bar!

            //System.Windows.ShutdownMode.OnMainWindowClose;
            

            //Program2.Upload("C:/", "pdf.pdf", "", "pdf.pdf");

            //System.Windows.MessageBox.Show("1 - " + Utility.FormatPath("/", ""));
            //System.Windows.MessageBox.Show("2 - " + Utility.FormatPath("/new", ""));

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
            //getFTPRootDirectory();
            //RemoteDirectoryItem directory = getRemoteDirectory("/", "new");
            //ServerDirectoryBrowser.ItemsSource = directory.RemoteDirectoryItems;

            //Background worker properties
            bgwU.WorkerReportsProgress = true;
            bgwU.WorkerSupportsCancellation = true;
            bgwU.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgwU.DoWork += new DoWorkEventHandler(bgw_Upload);
            bgwU.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);

            bgwD.WorkerReportsProgress = true;
            bgwD.WorkerSupportsCancellation = true;
            bgwD.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgwD.DoWork += new DoWorkEventHandler(bgw_Download);
            bgwD.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
        }

        private void RefreshRemoteListBox()
        {
            RemoteDirectoryItem dir = getRemoteDirectory(navigation.ToString(), "");
            ServerDirectoryBrowser.ItemsSource = dir.RemoteDirectoryItems;
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

            Program2.PrevDirectory = Program2.CurrentDirectory;
            Program2.CurrentDirectory = "";

            navigation.AddRoot();

            RemoteDirectoryItem rootDir = getRemoteDirectory("", "");

            ServerDirectoryBrowser.ItemsSource = rootDir.RemoteDirectoryItems;
        }

        //Gets remote directory contents of a directory and then creates and returns a RemoteDirectoryItem
        //
        private RemoteDirectoryItem getRemoteDirectory(string path, string name)
        {
            Contents dirConents = new Contents(path, name);

            //System.Windows.MessageBox.Show("Contents: " + dirConents.Count());

            List<Item> dirItems = dirConents.GetItems();

            RemoteDirectoryItem directory = new RemoteDirectoryItem() {Name = name, Path= path};
            
            foreach (Item item in dirItems)
            {
                //Debug.WriteLine("Contents Item Name:    " + item.FileName);

                string itemPath = "";

               if(name != "..")
               { 
                   if(name == "")
                       itemPath = path;
                   else
                       itemPath = path + "/" + name;
                   //itemPath = path;

                    if(path == "/")
                        itemPath = path + name;
               }
               else
               {
                   if(path != "/")
                   {
                       try
                       {
                           int index = path.LastIndexOf("/");
                           if(index != -1)
                           {
                                itemPath = path.Substring(0, index);
                           }
                       }
                       catch(Exception e)
                       {
                           Debug.WriteLine(e.Message);
                       }
                        
                   }
               }

               int fileSize = 0;
               if (!item.Directory)
               {
                   fileSize = item.FileSize;
               }

                RemoteDirectoryItem ftpItem = new RemoteDirectoryItem() { Name = item.FileName, IsDirectory = item.Directory, Path = itemPath, FileSize = fileSize };
                directory.RemoteDirectoryItems.Add(ftpItem);

                Debug.WriteLine("Remote Dir Item Name:  " + ftpItem.Name + ", " + ftpItem.Path + ", " + ftpItem.FileSize);
            }

            try 
            { 
                Debug.WriteLine("Current Directory: " + directory.RemoteDirectoryItems.Last().Path);
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
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
                RemoteDirectoryItem directoryItem = ServerDirectoryBrowser.SelectedItem as RemoteDirectoryItem;
                Debug.WriteLine(directoryItem.Path);

                // Added this to fix navigation incompetence
                if(directoryItem.IsDirectory)
                {
                    //System.Windows.MessageBox.Show("Name: " + directoryItem.Name + "; Path: " + directoryItem.Path);

                    //Contents dirContents = new Contents("", directoryItem.Name);

                    RemoteDirectoryItem directory = null;
                    
                    if (directoryItem.Name == "..")
                    {
                        navigation.Remove();
                        string parentPath = directoryPathFTP(directoryItem.Path);
                        Debug.WriteLine("Parent Path: " + parentPath);
                        directory = getRemoteDirectory(parentPath, "");
                    }
                    else if (directoryItem.Name == ".")
                    {
                        return;
                    }
                    else
                    {
                        navigation.Add(directoryItem.Name);
                    }

                    //navbar_text.Text = navigation.ToString();

                    // uncomment this when ready
                    directory = getRemoteDirectory(navigation.ToString(), "");
                    
                    ServerDirectoryBrowser.ItemsSource = directory.RemoteDirectoryItems;
                }
            }
        }

        //
        // Local event handlers
        //

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            if (ServerDirectoryBrowser.SelectedItem != null)
            {
                //Debug.WriteLine(ServerDirectoryBrowser.SelectedItem.ToString());

                RemoteDirectoryItem directoryItem = ServerDirectoryBrowser.SelectedItem as RemoteDirectoryItem;

                Debug.WriteLine(directoryItem.Name);

                if (directoryItem.Name == ".." || directoryItem.Name == ".")
                    return;

                if (directoryItem.IsDirectory)
                {
                    Program2.DeleteDirectory(directoryItem.Path, directoryItem.Name);
                }
                else
                {
                    Program2.DeleteFile(directoryItem.Path, directoryItem.Name);
                }

                RefreshRemoteListBox();
            }
        }

        private void upload_Click(object sender, RoutedEventArgs e)
        {
            if (localDirectoryBrowser.SelectedItem != null)
            {
                if (bgwU.IsBusy != true)
                {
                    bgwU.RunWorkerAsync(localDirectoryBrowser.SelectedItem as DirectoryItem);
                }
            }
        }

        private void download_Click(object sender, RoutedEventArgs e)
        {
            if (localDirectoryBrowser.SelectedItem != null && ServerDirectoryBrowser.SelectedItem != null)
            {
                DirectoryItem directoryItem = localDirectoryBrowser.SelectedItem as DirectoryItem;
                if (bgwD.IsBusy != true)
                {
                    ObservableCollection<object> directoryStuff = new ObservableCollection<object>();
                    directoryStuff.Add(localDirectoryBrowser.SelectedItem as DirectoryItem);
                    directoryStuff.Add(ServerDirectoryBrowser.SelectedItem as RemoteDirectoryItem);
                    bgwD.RunWorkerAsync(directoryStuff);
                }

            }
        }

        //
        // Remote event handlers
        //

        private async void createDirectoryRemote_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowInputAsync("Create Directory", "Enter Directory Name");

            if (result == null)
                return;

            ObservableCollection<RemoteDirectoryItem> dirItems = ServerDirectoryBrowser.ItemsSource as ObservableCollection<RemoteDirectoryItem>;

            Debug.WriteLine("New Dir Path: " + dirItems.Last().Path + "New Dir Name: " + result);
            
            Program2.CreateDirectory(dirItems.Last().Path, "/" + result);

            RefreshRemoteListBox();

            //await this.ShowMessageAsync("Hello", "Hello" + dirItems.Last().Path + "!");
        }

        private string directoryPath(string path)
        {
            int index = path.LastIndexOf("\\");

            if (index != -1)
            {
                if (path == @"C:\")
                {
                    return path;
                }
                else
                {
                    var count = path.Count(x => x == '\\');
                    Debug.WriteLine(count);
                    //if(count == 1)
                    //{
                    //path = path.Substring(0, index+1);
                    //}
                    //else
                    //{
                    path = path.Substring(0, index);
                    //}
                }
            }

            return path;
        }

        private string directoryPathFTP(string path)
        {
            int index = path.LastIndexOf("/");

            if (path == "")
                return path;

            if (index != -1)
            {
                if (path == "/")
                {
                    return path;
                }
                else
                {
                    //var count = path.Count(x => x == '/');
                    //Debug.WriteLine(count);
                    //if(count == 1)
                    //{
                    //path = path.Substring(0, index+1);
                    //}
                    //else
                    //{
                    Debug.WriteLine("Index of /: " + index);
                    path = path.Substring(0, index);

                    //}
                }
            }

            return path;
        }



        private void bgw_Upload(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            DirectoryItem directoryItem = e.Argument as DirectoryItem;

            Debug.WriteLine(directoryItem.Path);

            if (directoryItem.Path != null)
            {
                string dirPath = directoryPath(directoryItem.Path) + "\\";
                //Debug.WriteLine(dirPath);

                Program2.Upload(dirPath, directoryItem.Name, "", directoryItem.Name);
            }
        }

        private void bgw_Download(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ObservableCollection<object> directoryStuff = e.Argument as ObservableCollection<object>;
            DirectoryItem directoryItem = directoryStuff[0] as DirectoryItem;
            RemoteDirectoryItem remoteDirectoryItem = directoryStuff[1] as RemoteDirectoryItem;

            FileAttributes attr = File.GetAttributes(directoryItem.Path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                //RemoteDirectoryItem remoteDirectoryItem = ServerDirectoryBrowser.SelectedItem as RemoteDirectoryItem;

                Debug.WriteLine("Its a directory! Name: " + directoryItem.Name);

                if (!remoteDirectoryItem.IsDirectory)
                {
                    string localPath = directoryPath(directoryItem.Path) + "\\";
                    //Debug.Write("Local Directory Path: " + localPath);
                    Debug.WriteLine("Dir Item path: " + directoryItem.Path); ;
                    Debug.WriteLine("Download item path: " + directoryItem.Path + remoteDirectoryItem.Name);
                    Debug.WriteLine("localPath: " + localPath);
                    //Debug.WriteLine("Last index of \\: " + directoryItem.Path.LastIndexOf(@"\"));
                    //Debug.WriteLine("Path length: " + directoryItem.Path.Length);


                    if (directoryItem.Path.LastIndexOf(@"\") == directoryItem.Path.Length - 1)
                    {
                        Debug.WriteLine("Just append file name");
                        Program2.Download(remoteDirectoryItem.Path, remoteDirectoryItem.Name, directoryItem.Path, remoteDirectoryItem.Name, remoteDirectoryItem.FileSize.ToString());
                    }
                    else
                        Program2.Download(remoteDirectoryItem.Path, remoteDirectoryItem.Name, directoryItem.Path + "\\", remoteDirectoryItem.Name, remoteDirectoryItem.FileSize.ToString());

                    //Program2.Download(remoteDirectoryItem.Path, remoteDirectoryItem.Name, directoryItem.Path, remoteDirectoryItem.Name);

                }

            }
        }

        private void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Debug.WriteLine("Finished loading");
            progressBar.Value = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ConnectionManagement newWin = new ConnectionManagement();
            newWin.Show();
        }

        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            if(Program2.Host.Length == 0 || Program2.User.Length == 0 || Program2.Pass.Length == 0)
            {
                System.Windows.MessageBox.Show("You need to log in first!");
                return;
            }

            getFTPRootDirectory();
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

        public int FileSize { get; set; }

        //A list of the files and directories in this directory
        public ObservableCollection<RemoteDirectoryItem> RemoteDirectoryItems { get; set; }
    }
}
