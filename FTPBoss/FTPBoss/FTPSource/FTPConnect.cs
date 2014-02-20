using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace FTPBoss
{
    class RequestMethods
    {
        public const int FileSize   = 1,
                         ListDir    = 2,
                         ListDir2   = 3,
                         MakeDir    = 4,
                         Download   = 5,
                         Upload     = 6,
                         Rename     = 7,
                         DeleteFile = 8,
                         DeleteDir  = 9;
    }

    class Utility
    {
        public const int FILE = 1,
                         DIR  = 2;

        public static string chmod2Num(string input)
        {
            if (input[0] == 'd' || input[0] == '-')
                input = input.Substring(1);

            int sum = 0;

            if (input[0] == 'r')
                sum += 400;
            if (input[1] == 'w')
                sum += 200;
            if (input[2] == 'x')
                sum += 100;
            if (input[3] == 'r')
                sum += 40;
            if (input[4] == 'w')
                sum += 20;
            if (input[5] == 'x')
                sum += 10;
            if (input[6] == 'r')
                sum += 4;
            if (input[7] == 'w')
                sum += 2;
            if (input[8] == 'x')
                sum += 1;

            return "0" + Convert.ToString(sum);
        }

        public static int GetCountInDir(string path, string dirName, int type = 0)
        {
            Contents contents = new Contents(path, dirName);
            return contents.Count(type);

            /*
            try
            {
                int total = 0;
                FtpWebRequest request = Program.GetRequest(RequestMethods.ListDir, path + dirName);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());

                while (reader.ReadLine() != null)
                    ++total;

                reader.Close();
                response.Close();
                return total - 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return 0;
            }
             */
        }

        public static string FormatPath(string path, string name)
        {
            path = path.TrimEnd(Path.DirectorySeparatorChar);

            if (path.Length > 0)
                path += "/";

            return path + name;
        }
    }

    class Local
    {
        public const int bufferSize = 2048;
        public static bool FileExists(string path, string fileName)
        {
            return File.Exists(path + fileName);
        }

        public static bool DirectoryExists(string path, string dirName)
        {
            return true;
        }

        public static bool WriteFile(string path, string fileName, Stream stream)
        {
            FileStream localFileStream = new FileStream(path + fileName, FileMode.Create);
            byte[] byteBuffer = new byte[bufferSize];
            int bytesRead = stream.Read(byteBuffer, 0, bufferSize);

            try
            {
                while (bytesRead > 0)
                {
                    localFileStream.Write(byteBuffer, 0, bytesRead);
                    bytesRead = stream.Read(byteBuffer, 0, bufferSize);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }

            localFileStream.Close();
            return true;
        }

        static bool CreateDirectory(string dirName)
        {
            string path = @"c:\" + dirName + @"\";

            try
            {
                // Determine whether the directory exists. 
                if (Directory.Exists(path))
                {   //If exists, fail
                    Console.WriteLine("That path exists already.");
                    Console.ReadKey();
                    return false;
                }

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path);
                Console.WriteLine("The directory was created successfully.");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return false;
            }
        }


        static bool CreateFile(string dir, string fileName)
        {
            string path = @"c:\" + dir + @"\" + fileName;
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                using (FileStream fs = File.Create(path))
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file");
                    //Add info to the file
                    fs.Write(info, 0, info.Length);
                    return true;
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
                return false;
            }
        }

        static bool CheckFile(string dir, string file)
        {
            if (File.Exists(@"c:\" + dir + @"\" + file))
                return true;
            else
                return false;
        }


        static bool CheckDirectory(string dir)
        {
            if (Directory.Exists(dir))
                return true;
            else
                return false;
        }


        static void getFileSize(string dir)
        {

            decimal size;
            //make reference to a directory
            DirectoryInfo di = new DirectoryInfo(@"c:\" + dir);
            //get reference to each file in the directory
            FileInfo[] fileArray = di.GetFiles();
            foreach (FileInfo f in fileArray)
            {
                if (f.Length >= 1073741824)
                {
                    size = decimal.Divide(f.Length, 1073741824);
                    Console.WriteLine("{0} is {1} GB in size", f.Name, size);
                }
                else if (f.Length >= 1048576)
                {
                    size = decimal.Divide(f.Length, 1048576);
                    Console.WriteLine("{0} is {1} MB in size", f.Name, size);
                }
                else if (f.Length >= 1024)
                {
                    size = decimal.Divide(f.Length, 1024);
                    Console.WriteLine("{0} is {1} KB in size", f.Name, size);
                }
                else
                    Console.WriteLine("{0} is {1} bytes in size", f.Name, f.Length);
            }
        }

        //list files in the directory
        static bool ListFiles(string dir)
        {
            string path = @"c:\" + dir;
            try
            {
                string[] files = Directory.GetFiles(path);
                Console.WriteLine("--- Files: ---");
                foreach (string name in files)
                {
                    Console.WriteLine(name);
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }


        //ONLY RENAMES FILE IN SAME FOLDER.
        //To move file into another directory, use MoveFile
        static bool RenameFile(string dir, string oldFile, string newFile)
        {
            try
            {
                string oldPath = @"c:\" + dir + @"\" + oldFile;
                string newPath = @"c:\" + dir + @"\" + newFile;

                //Ensures that file is created
                if (!File.Exists(oldPath))
                    using (FileStream fs = File.Create(oldPath)) { }

                //Ensures target does not exist
                if (File.Exists(newPath))
                    File.Delete(newPath);

                File.Move(oldPath, newPath);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return false;
            }
        }

        //move from one folder to another, can be in different folders
        static bool MoveFile(string dir, string dir2, string source, string target)
        {
            string path = @"c:\" + dir + @"\" + source;
            string path2 = @"c:\" + dir2 + @"\" + target;
            try
            {
                if (!File.Exists(path))
                {
                    // This statement ensures that the file is created, 
                    // but the handle is not kept. 
                    using (FileStream fs = File.Create(path)) { }
                }


                if (File.Exists(path2))
                    File.Delete(path2);

                // Move the file.
                File.Move(path, path2);
                Console.WriteLine("{0} was moved to {1}.", path, path2);

                // See if the original exists now. 
                if (File.Exists(path))
                {
                    Console.WriteLine("The original file still exists, which is unexpected.");
                }
                else
                {
                    Console.WriteLine("The original file no longer exists.");
                }
                return true;

            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return false;
            }
        }


        static bool DeleteDirectory(string dirName)
        {
            string path = @"c:\" + dirName + @"\";

            try
            {
                //Determine if directory does not exist
                if (!(Directory.Exists(path)))
                {
                    Console.WriteLine("That path does not exist.");
                    Console.ReadKey();
                    return false;
                }
                // Delete the directory.
                Directory.Delete(path, true);
                Console.WriteLine("The directory was deleted successfully.");
                Console.ReadKey();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("The process failed: {0}", e.ToString());
                return false;
            }
        }


        static bool DeleteFile(string dir, string file)
        {
            string path = @"c:\" + dir + @"\" + file;
            if (File.Exists(path))
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }
            }
            Console.WriteLine("File Deleted Successfully");
            return true;
        }
    }

    /*
     * TODO
     * - MoveFile() - Moves file from one directory to another
     * */

    class Contents
    {
        private List<Item> Items = new List<Item>();
        private int total,
                    fileCount,
                    dirCount;

        public Contents() { total = fileCount = dirCount = 0; }

        public Contents(string path, string dirName)
        {
            total = fileCount = dirCount = 0;

            // check if dir exists, except root directory ("")
            if (!Program2.DirectoryExists(path, dirName) && (path + dirName) != "")
            {
                Console.WriteLine("Could not instantiate Contents(): '"+ dirName +"' does not exist");
                return;
            }

            try
            {
                FtpWebRequest request = Program2.GetRequest(RequestMethods.ListDir2, Utility.FormatPath(path, dirName));
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();

                while (line != null)
                {
                    line = reader.ReadLine();
                    this.Add(line);
                }

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }
        
        /* MODIFIED */
        public int Count(int type = 0)
        {
            if (type == Utility.FILE)
                return this.fileCount;
            else if (type == Utility.DIR)
                return this.dirCount;
            else
                return this.total;
        }

        public void Print(int index = -1)
        {
            if (index > 0 && index < this.Items.Count)
            {
                Console.WriteLine(this.Items[index]);
            }
            else
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    Console.WriteLine(this.Items[i]);
                }
            }
        }
        public void Clear() { this.Items.Clear(); }

        /* NEW ADDITION */
        public List<Item> GetItems()
        {
            return this.Items;
        }
        /* NEW ADDITION */
        public string[] GetFileNames()
        {
            if (this.fileCount == 0)
                return null;

            string[] strArray = new string[this.fileCount];
            int arrayCount = 0;

            for (int i = 0; i < this.total; ++i)
            {
                if (!this.Items[i].Directory)
                {
                    strArray[arrayCount++] = this.Items[i].FileName;
                }
            }

            return strArray;
        }

        /* MODIFIED */
        public bool Add(string line)
        {
            if (line == null)
                return false;
                                 // @"^([d-])([rwxt-]{3}){3}\s+\d{1,}\s+.*?(\d{1,})\s+(\w+\s+\d{1,2}\s+(?:\d{4})?)(\d{1,2}:\d{2})?\s+(.+?)\s?$"
            Regex regex = new Regex(@"^([d-])((?:[rwxt-]{3}){3})\s+\d{1,}\s+.*?(\d{1,})\s+(\w+)\s+(\d{1,2})\s+(\d{4‌​})?(\d{1,2}:\d{2})?\s+(.+?)\s?$",
                RegexOptions.Compiled |
                RegexOptions.Multiline | 
                RegexOptions.IgnoreCase | 
                RegexOptions.IgnorePatternWhitespace);

            var matches = regex.Matches(line);

            /* Matches
             * 0    Original line
             * 1    - or d (file or directory)
             * 2    Permissions
             * 3    Filesize
             * 4    Month*           * Last modified date/time
             * 5    Day*
             * 6    Year*
             * 7    Time*
             * 8    Filename
             * */

            string filename = matches[0].Groups[8].Value;

            int filesize = Convert.ToInt32(matches[0].Groups[3].Value);

            string dirTag = matches[0].Groups[3].Value.ToString();

            bool directory = (matches[0].Groups[1].Value == "d");

            string permissions = Utility.chmod2Num(matches[0].Groups[2].Value);

            int month = DateTime.Parse(matches[0].Groups[4].Value + " 01, 1900").Month;
            int day = Convert.ToInt32(matches[0].Groups[5].Value);
            int year;
            DateTime time = Convert.ToDateTime(matches[0].Groups[7].Value);

            if (matches[0].Groups[6].Value == "")
                year = DateTime.Now.Year;
            else
                year = Convert.ToInt32(matches[0].Groups[6].Value);

            DateTime modified = new DateTime(year, month, day, time.Hour, time.Minute, time.Second);

            this.Items.Add(new Item(
                                    filename,
                                    filesize,
                                    "type",
                                    modified,
                                    permissions,
                                    directory
                                ));

            if (filename != "..")
            {
                ++this.total;

                if (directory)
                    ++this.dirCount;
                else
                    ++this.fileCount;
            }

            return true;
        }

        /* MODIFIED */
        public bool Add(string iName, int iSize, string iType, DateTime iModified, string iPermissions, bool iDirectory = false)
        {
            this.Items.Add(new Item(iName, iSize, iType, iModified, iPermissions, iDirectory));

            if (iName != "..")
            {
                ++this.total;

                if (iDirectory)
                    ++this.dirCount;
                else
                    ++this.fileCount;
            }

            return true;
        }
    }

    class Item
    {
        public string FileName,
                      FileType,
                      Permissions;
        public DateTime LastModified;
        public bool Directory,
                    UpOneLevel;
        public int FileSize;
        public Item(string iName, int iSize, string iType, DateTime iModified, string iPermissions, bool iDirectory = false)
        {
            if (iName == "..")
            {
                this.UpOneLevel = true;

                iType = iPermissions = null;
                iSize = -1;
                iDirectory = true;
            }
            else
                this.UpOneLevel = false;

            this.FileName     = iName;
            this.FileSize     = iSize;
            this.FileType     = iType;
            this.LastModified = iModified;
            this.Permissions  = iPermissions;
            this.Directory    = iDirectory;
        }
        public override string ToString()
        {
            if (this.UpOneLevel)
                return "..";
            else
                return this.FileName + " " + this.FileSize + " " + this.LastModified.ToString() + " " + this.Permissions + " " + Convert.ToString(this.Directory);
        }
    }

    class Program2
    {
        static string Host = "pftp.bugs3.com",
                      User = "u631161179.ftp",
                      Pass = "testftp1";

        static string currentFtpDir    = "",
                      currentLocalDir  = "",
                      previousFtpDir   = "",
                      previousLocalDir = "";

        static Contents DirectoryContents;

/*
        static void Main(string[] args)
        {
            DirectoryContents = new Contents();

            currentFtpDir = "";
            currentLocalDir = "C:/";

            //ListFiles("", "");

            //Console.WriteLine("File Count: " + Utility.GetCountInDir("", "", Utility.FILE));
            //Console.WriteLine(" Dir Count: " + Utility.GetCountInDir("", "", Utility.DIR));

            //Upload("C:/", "pdf.pdf", "", "pdf.pdf");
            //Download("", "pdf.pdf", "C:/", "pdf.pdf");
            //RenameFile("", "pdf.pdf", "pdf2.pdf");
            //CreateDirectory("", "TestDir");
            //Rename("", "Test", "TestDir33", DIR);
            //DeleteFile("", "testfile.txt");

            Console.WriteLine(DirectoryExists("", "TestDir"));
            //Console.WriteLine(FileExists("", "test.txt"));

            DeleteDirectory("", "TestDir33");

            Console.ReadKey();
        }
 * /

        /* Utility Functions */
        static Contents GetContents(string path, string dirName)
        {
            if (!DirectoryExists(path, dirName))
            {
                Console.WriteLine("Directory does not exist!");
                return null;
            }

            return new Contents(path, dirName);
        }

        /* NEWLY MODIFIED */
        static string[] GetFileNames(string path, string dirName)
        {
            Contents contents = new Contents(path, dirName);
            return contents.GetFileNames();
        }

        public static bool FileExists(string path, string fileName)
        {
            Console.WriteLine("FileExists('"+path+"', '"+fileName+"')");
            FtpWebResponse response = null;
            bool returnValue = false;

            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.FileSize, Utility.FormatPath(path, fileName));
                response = (FtpWebResponse)request.GetResponse();
                request = null;
                returnValue = true;
            }
            catch (WebException ex)
            {
                response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    returnValue = false;
                }
                else
                {
                    Console.WriteLine("Unknown error in FileExists(): " + ex.Message.ToString());
                }
            }

            response.Close();
            return returnValue;
        }
        public static FtpWebRequest GetRequest(int method, string fileName = "", bool keepAlive = true, bool usePassive = true, bool useBinary = true)
        {
            string requestString = "ftp://" + Host;

            if (fileName.Length > 0)
                requestString += "/" + fileName;

            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(requestString);
                request.Credentials = new NetworkCredential(User, Pass);
                request.KeepAlive = keepAlive;
                request.UsePassive = usePassive;
                request.UseBinary = useBinary;

                switch (method)
                {
                    case RequestMethods.FileSize:
                        request.Method = WebRequestMethods.Ftp.GetFileSize;
                        break;
                    case RequestMethods.Download:
                        request.Method = WebRequestMethods.Ftp.DownloadFile;
                        break;
                    case RequestMethods.ListDir:
                        request.Method = WebRequestMethods.Ftp.ListDirectory;
                        break;
                    case RequestMethods.ListDir2:
                        request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                        break;
                    case RequestMethods.MakeDir:
                        request.Method = WebRequestMethods.Ftp.MakeDirectory;
                        break;
                    case RequestMethods.Upload:
                        request.Method = WebRequestMethods.Ftp.UploadFile;
                        break;
                    case RequestMethods.Rename:
                        request.Method = WebRequestMethods.Ftp.Rename;
                        break;
                    case RequestMethods.DeleteFile:
                        request.Method = WebRequestMethods.Ftp.DeleteFile;
                        break;
                    case RequestMethods.DeleteDir:
                        request.Method = WebRequestMethods.Ftp.RemoveDirectory;
                        break;
                }

                return request;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return null;
            }
        }

        static bool DeleteDirectory(string path, string dirName)
        {
            // Check if directory exists
            if (!DirectoryExists(path, dirName))
            {
                Console.WriteLine("Directory '" + dirName + "' does not exist!");
                return false;
            }

            // Don't delete the root
            if ((path + dirName) == "")
            {
                Console.WriteLine("Don't delete the root!!!");
                return false;
            }

            Contents contents = new Contents(path, dirName);

            // If no files or directories, delete directory
            if (contents.Count() == 0)
            {
                try
                {
                    Console.WriteLine("Deleting directory!");
                    FtpWebRequest request = GetRequest(RequestMethods.DeleteDir, path + dirName);
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    response.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return false;
                }
            }
            else
            {
                List<Item> items = contents.GetItems();

                for (int i = 0; i < items.Count; ++i)
                {
                    // If item is a directory
                    if (items[i].Directory)
                    {
                        // Recursion is fun!
                        DeleteDirectory(path + dirName, items[i].FileName);
                    }
                    else // yay, it's a file!
                    {
                        FTPBoss.Program2.DeleteFile2(path + dirName, items[i].FileName);
                    }
                }

                DeleteDirectory(path, dirName);
                return true;
            }


                /*
                for (int i = 0; i < filenames.Count(); ++i)
                {
                    DeleteFile(path + dirName, filenames[i]);
                }
                 * */
        }

        static bool DeleteFile2(string path, string fileName)
        {
            Console.WriteLine("DeleteFile('" + path + "', '" + fileName + "')");

            // Check if file exists
            if (!FileExists(path, fileName))
            {
                Console.WriteLine("File '" + fileName + "' does not exist!");
                return false;
            }

            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.DeleteFile, Utility.FormatPath(path, fileName));
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return false;
            }
        }

        static bool ChangeFilePermission(string path, string fileName)
        {
            // client.SiteChangeMode("file.txt", new UnixPermissionSet(UnixPermission.Write));

            /*
             * List<FtpItem> items = client.GetList();
             * List<FtpPermission> permissions = items[0].Permissions;
             * bool canRead = permissions.Contains(FtpPermission.Read);
             * bool canWrite = permissions.Contains(FtpPermission.Write);
             * bool canCreateFile = permissions.Contains(FtpPermission.CreateFile);
             * bool canChangeFolder = permissions.Contains(FtpPermission.ChangeFolder);
             * */

            return false;
        }

        static bool CreateDirectory(string path, string dirName)
        {
            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.MakeDir, path + dirName);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                request = null;
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message.ToString());
                return false;
            }
        }

        public static bool DirectoryExists(string path, string dirName)
        {
            Console.WriteLine("DirectoryExists('" + path + "', '" + dirName + "')");

            bool returnValue = false;

            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.ListDir, path);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    Console.WriteLine("DirName: " + dirName + "; Line: " + line);

                    if (dirName == line)
                    {
                        returnValue = true;
                        break;
                    }

                    line = reader.ReadLine();
                }

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DirectoryExists(): " + ex.Message.ToString());
            }

            return returnValue;
        }

        static bool Rename(string path, string oldName, string newName, int type = Utility.FILE)
        {
            if (type == Utility.FILE)
            {
                // Check if current file exists
                if (!FileExists(path, oldName))
                {
                    Console.WriteLine("File '" + oldName + "' does not exist!");
                    return false;
                }

                // Check if new filename exists
                if (FileExists(path, newName))
                {
                    Console.WriteLine("File '" + newName + "' already exists!");
                    return false;
                }
            }
            else if (type == Utility.DIR)
            {
                // Check if current directory exists
                if (!DirectoryExists(path, oldName))
                {
                    Console.WriteLine("Directory '" + oldName + "' does not exist!");
                    return false;
                }

                // Check if new directory exists
                if (DirectoryExists(path, newName))
                {
                    Console.WriteLine("Directory '" + newName + "' already exists!");
                    return false;
                }
            }
            else
                return false;

            try
            {
                FtpWebRequest ftpRequest = GetRequest(RequestMethods.Rename, path + oldName);
                ftpRequest.RenameTo = newName;
                ftpRequest.GetResponse();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Rename error: " + ex.Message.ToString());
                return false;
            }
        }

        static bool Download(string fromPath, string fromFile, string toPath, string toFile)
        {
            // Check if from file exists
            if (!FileExists(fromPath, fromFile))
            {
                Console.WriteLine("File '" + fromFile + "' does not exist!");
                return false;
            }
         
            // Check if to file already exists
            if (Local.FileExists(toPath, toFile))
            {
                Console.WriteLine("File '" + toFile + "' already exists on local drive!");
                return false;
            }

            bool returnValue = false;

            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.Download, fromPath + fromFile);
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();

                Console.WriteLine("Download Complete, status {0}", response.StatusDescription);

                // Write to file
                if (Local.WriteFile(toPath, toFile, responseStream))
                {
                    Console.WriteLine("Successfully written to file: " + toPath + toFile);
                    returnValue = true;
                }
                else
                {
                    Console.WriteLine("Failed to write to file: " + toPath + toFile);
                    returnValue = false;
                }

                responseStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            return returnValue;
        }

        static bool Upload(string fromPath, string fromFile, string toPath, string toFile)
        {
            // Check if from file exists
            if (!Local.FileExists(fromPath, fromFile))
            {
                Console.WriteLine("File '" + toFile + "' does not exist on local drive!");
                return false;
            }

            // Check if to file already exists
            if (FileExists(fromPath, fromFile))
            {
                Console.WriteLine("File '" + toFile + "' already exists on server!");
                return false;
            }

            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.Upload, toPath + toFile);

                StreamReader sourceStream = new StreamReader(fromPath + fromFile);
                byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                sourceStream.Close();
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);
                response.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }

            return true;
        }

        static bool ListFiles(string path, string dirName)
        {
            // check if dir exists

            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.ListDir2, path + dirName);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();

                while (line != null)
                {
                    line = reader.ReadLine();
                    DirectoryContents.Add(line);
                }

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:\n" + ex.Message);
                return false;
            }

            DirectoryContents.Print();
            DirectoryContents.Clear();
            return true;
        }
    }

    //test
}