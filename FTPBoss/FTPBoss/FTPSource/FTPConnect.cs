using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;   // For reg expression
using System.Net.Sockets;               // For file permissions

namespace FTPBoss
{
    /*
 * TODO
 * - MoveFile() - Moves file from one directory to another
 * */

    #region Utilitiez
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

        public static int chmod2Num(string input)
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

            return sum;
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

            path += name;
            return path.Replace("//", "/");
        }
    }
    #endregion

    #region Local localities
    class Local
    {
        public const int bufferSize = 2048;
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


        static string getFileSize(string dir)
        {

            string val = "Error in getFileSize function";
            //make reference to a directory
            DirectoryInfo di = new DirectoryInfo(@"c:\" + dir);
            //get reference to each file in the directory
            FileInfo[] fileArray = di.GetFiles();
            foreach (FileInfo f in fileArray)
            {
                if (f.Length >= 1073741824)
                {
                    val = "GB";
                    return val;
                }
                else if (f.Length >= 1048576)
                {
                    val = "MB";
                    return val;
                }
                else if (f.Length >= 1024)
                {
                    val = "KB";
                    return val;
                }
                else
                {
                    val = "bytes";
                    return val;
                }
            }
            return val;
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

               /*
        public const int bufferSize = 2048;
        public static bool FileExists(string path, string fileName)
        {
            return File.Exists(path + fileName);
        }

        public static bool DirectoryExists(string path, string dirName)
        {
            return true;
        }
        */

        public static bool WriteFile(string path, string fileName, Stream stream, int filesize = -1)
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
                    int loadProgress = 0;
                    if (filesize > 0)
                    {
                        loadProgress = (int)(localFileStream.Length * 100 / filesize);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Could not get filesize");
                        loadProgress = (int)(localFileStream.Length * 100 / 100000000);
                    }
                    MainWindow.bgwD.ReportProgress(loadProgress);
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

    }
    #endregion

    #region FTP class just for changing file permissions!
    class FtpSocket
    {
        const int TIMEOUT = 10000;
        TcpClient client;
        private string Host, User, Pass, Error, Response;
        int Port, ResponseCode;

        public string GetError() { return Error; }
        public string GetResponse() { return Response; }
        public int GetResponseCode() { return ResponseCode; }

        public FtpSocket(string host, string user, string pass, int port = 21, bool connect = true)
        {
            Host = host;
            User = user;
            Pass = pass;
            Port = port;
            Error = Response = null;
            client = new TcpClient();
            ResponseCode = 0;

            if (connect)
                Connect();
        }
        ~FtpSocket()
        {
            client.Close();
        }

        public void Flush()
        {
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite && stream.CanRead)
                {
                    Byte[] bytes = new Byte[client.ReceiveBufferSize];
                    stream.ReadTimeout = TIMEOUT;
                    stream.Read(bytes, 0, Convert.ToInt32(client.ReceiveBufferSize));
                }
            }
            catch (Exception ex)
            {
                Error = "Flush() error: " + ex.Message.ToString();
                Console.WriteLine(Error);
            }
        }

        public string Command(string command)
        {
            string response = "";

            try
            {
                NetworkStream stream = client.GetStream();

                if (stream.CanWrite && stream.CanRead)
                {
                    Byte[] bytes = Encoding.ASCII.GetBytes(command + "\r\n");
                    stream.Write(bytes, 0, bytes.Length);
                    StreamReader reader = new StreamReader(stream);
                    response = reader.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Error = "Command() error: " + ex.Message.ToString();
                Console.WriteLine(Error);
            }

            ResponseCode = Convert.ToInt32(response.Split(' ')[0]);
            Response = response;

            return response;
        }

        public bool Connect()
        {
            string response;

            try
            {
                client.Connect(Host, Port);
                Flush();
                response = Command("USER " + User);

                if (response.IndexOf("331") >= 0)
                {
                    response = Command("PASS " + Pass);
                    if (response.IndexOf("230") >= 0)
                    {
                        Console.WriteLine("Successfully logged in");
                        Console.WriteLine(response);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Error = "Connect() error: " + ex.Message.ToString();
                Console.WriteLine(Error);
            }

            return false;
        }

        public bool Send(string command)
        {
            string response = Command(command);

            if (response.IndexOf("200") >= 0)
            {
                Console.WriteLine("Success");
                return true;
            }

            return false;
        }
    }
    #endregion

    #region Directory Contents stuff
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

            if (dirName == ".." || dirName == ".")
                dirName = "";

            //System.Windows.MessageBox.Show("Contents() Path: '"+path+"'; dirName: '"+dirName+"'");

            /* check if dir exists, except root directory ("")
            if (!Program2.DirectoryExists(path, dirName) && (path + dirName) != "")
            {
                System.Windows.MessageBox.Show("Could not instantiate Contents(): '" + dirName + "' does not exist");
                //return;
            }*/

            try
            {
                FtpWebRequest request = Program2.GetRequest(RequestMethods.ListDir2, Utility.FormatPath(path, dirName));
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();

                while (line != null)
                {
                    this.Add(line);
                    line = reader.ReadLine();
                }

                AddExtraNavDirs();

                reader.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Error 2: " + ex.Message);
                Console.WriteLine("ERROR: " + ex.Message);
            }
        }

        /* ADDED */
        public void AddExtraNavDirs()
        {
            bool dot = false,       // .  (maybe obsolete)
                 dotdot = false;    // .. (previous directory)

            for (int i = 0; i < Items.Count; ++i)
            {
                if (Items[i].FileName == ".")
                    dot = true;
                else if (Items[i].FileName == "..")
                    dotdot = true;
            }

            if (!dot)
            {
                //Add(".", 0, "", new DateTime(), 0, true); // Obsolete?
            }

            if(!dotdot)
            {
                Add("..", 0, "", new DateTime(), 0, true);
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
            // Sort items so directories are first
            return Items.OrderBy(o => !o.Directory).ToList();
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
            // edited

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

            int permissions = Utility.chmod2Num(matches[0].Groups[2].Value);

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

            //if (filename != "..")
            //{
                ++this.total;

                if (directory)
                    ++this.dirCount;
                else
                    ++this.fileCount;
            //}

            return true;
        }

        /* MODIFIED */
        public bool Add(string iName, int iSize, string iType, DateTime iModified, int iPermissions, bool iDirectory = false)
        {
            this.Items.Add(new Item(iName, iSize, iType, iModified, iPermissions, iDirectory));

            //if (iName != "..")
            //{
                ++this.total;

                if (iDirectory)
                    ++this.dirCount;
                else
                    ++this.fileCount;
            //}

            return true;
        }
    }
    class Item
    {
        public string FileName,
                      FileType;
        public DateTime LastModified;
        public bool Directory,
                    UpOneLevel;
        public int FileSize, Permissions;
        public Item(string iName, int iSize, string iType, DateTime iModified, int iPermissions, bool iDirectory = false)
        {
            if (iName == "..")
            {
                UpOneLevel = iDirectory = true;
                iSize = iPermissions = -1;
                iType = null;
            }
            else
                UpOneLevel = false;

            FileName     = iName;
            FileSize     = iSize;
            FileType     = iType;
            LastModified = iModified;
            Permissions  = iPermissions;
            Directory    = iDirectory;
        }
        public override string ToString()
        {
            if (this.UpOneLevel)
                return "..";
            else
                return FileName + " " + FileSize + " " + LastModified.ToString() + " " + Permissions + " " + Convert.ToString(Directory);
        }
    }
    #endregion

    #region Connection Management Sweetness
    class Credentials
    {
        public string Host, User, Pass, Port;
        public Credentials(string host, string user, string pass, string port) { Host = host; User = user; Pass = pass; Port = port; }
        public override string ToString() { return "host=" + Host + ";user=" + User + ";pass=" + Pass + ";port=" + Port + ";"; }
    }
    class CredentialProfiles
    {
        private const int host = 1, user = 2, pass = 3, port = 4;
        private string selectedProfile;
        private Dictionary<string, Credentials> creds;
        public CredentialProfiles()
        {
            selectedProfile = "";
            creds = new Dictionary<string, Credentials>();
        }
        public string[] GetProfiles() { return creds.Keys.ToArray(); }
        public bool SelectProfile(string profileName)
        {
            if (!creds.ContainsKey(profileName))
            {
                Console.WriteLine("Profile '" + profileName + "' does not exist!");
                return false;
            }

            selectedProfile = profileName;
            return true;
        }
        public string GetHost() { return getStuff(host); }
        public string GetUser() { return getStuff(user); }
        public string GetPass() { return getStuff(pass); }
        public string GetPort() { return getStuff(port); }
        private string getStuff(int type)
        {
            if (selectedProfile.Length == 0)
            {
                Console.WriteLine("No profile selected!");
                return null;
            }

            switch (type)
            {
                case host:
                    return creds[selectedProfile].Host;
                case user:
                    return creds[selectedProfile].User;
                case pass:
                    return creds[selectedProfile].Pass;
                case port:
                    return creds[selectedProfile].Port;
                default:
                    return null;
            }
        }
        public bool Contains(string profileName) { return creds.ContainsKey(profileName); }
        public Credentials GetCredentials(string profileName)
        {
            Credentials credOut = null;
            creds.TryGetValue(profileName, out credOut);
            return credOut;
        }
        public void Add(string profileName, string host, string user, string pass, string port)
        {
            if (creds.ContainsKey(profileName))
            {
                Console.WriteLine("profile name already exists!");
                return;
            }

            creds.Add(profileName, new Credentials(host, user, pass, port));
        }
        public void Edit(string profileName, string profile, string host, string user, string pass, string port)
        {
            if (creds.ContainsKey(profileName))
            {
                if (profileName != profile)
                {
                    Credentials newcred = new Credentials(host, user, pass, port);
                    creds.Add(profile, newcred);
                    creds.Remove(profileName);
                }
                else
                {
                    creds[profileName].Host = host;
                    creds[profileName].User = user;
                    creds[profileName].Pass = pass;
                    creds[profileName].Port = port;
                }
            }
        }
        public void Delete(string profileName)
        {
            if (!creds.ContainsKey(profileName))
            {
                Console.WriteLine("That profile does not existed and therefore it cannot be deleteded!");
                return;
            }
            creds.Remove(profileName);
        }
        public void Clear() { creds.Clear(); }
        public bool SaveToFile(string file)
        {
            List<string> lines = new List<string>();
            foreach (KeyValuePair<string, Credentials> cred in this.creds)
            {
                lines.Add("profile=" + cred.Key + ";" + cred.Value);
                Console.WriteLine("profile=" + cred.Key + ";" + cred.Value);
            }
            System.IO.File.WriteAllLines(file, lines.ToArray());
            return true;
        }
        public bool LoadFromFile(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine("File '" + file + "' does not exist!");
                return false;
            }

            string[] creds = System.IO.File.ReadAllLines(file);

            for (int i = 0; i < creds.Count(); ++i)
            {
                string[] line = creds[i].Split(';');

                if (line.Count() != 6)
                {
                    Console.WriteLine("Invalid file contents! f");
                    return false;
                }

                string profile = "", host = "", user = "", pass = "", port = "";

                for (int j = 0; j < line.Count(); ++j)
                {
                    string[] pair = line[j].Split('=');

                    if (pair.Count() != 2)
                        continue;

                    switch (pair[0])
                    {
                        case "profile":
                            profile = pair[1];
                            break;
                        case "host":
                            host = pair[1];
                            break;
                        case "user":
                            user = pair[1];
                            break;
                        case "pass":
                            pass = pair[1];
                            break;
                        case "port":
                            port = pair[1];
                            break;
                    }
                }

                Add(profile, host, user, pass, port);
            }

            return true;
        }
    }
    #endregion

    class Program2
    {
        static public CredentialProfiles credProfiles = null;
        static public string CredentialFile = AppDomain.CurrentDomain.BaseDirectory + "/credentials.dat";
        static public string Host = "", User = "", Pass = "", Port = "";

        static public string CurrentDirectory = "";
        static public string PrevDirectory = "";

        static Contents DirectoryContents;

        /*
         *  THESE CREDENTIALS ARE OBSOLETE
        static string Host = "ftp.drwestfall.net",
                      User = "ftp04",
                      Pass = "project";         
        static string Host = "pftp.bugs3.com",
                      User = "u631161179.ftp",
                      Pass = "testftp1";
        */

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
            /*
            if (!DirectoryExists(path, dirName))
            {
                Console.WriteLine("Directory does not exist!");
                return null;
            }*/

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

        public static bool DeleteDirectory(string path, string dirName)
        {
            if (dirName == "." || dirName == ".." || (path + dirName) == "")
                return false;

            //System.Windows.MessageBox.Show("DeleteDirectory('"+path+"', '"+dirName+"');");

            Contents contents = new Contents(path, dirName);

            // If no files or directories, delete directory
            if (contents.Count() <= 1)
            {
                try
                {
                    //System.Windows.MessageBox.Show("Deleting directory empty: " + Utility.FormatPath(path, dirName));
                    FtpWebRequest request = GetRequest(RequestMethods.DeleteDir, Utility.FormatPath(path, dirName));
                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                    response.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Delete dir error: " + ex.Message);
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
                        DeleteDirectory(Utility.FormatPath(path, dirName), items[i].FileName);
                    }
                    else // yay, it's a file!
                    {
                        DeleteFile(Utility.FormatPath(path, dirName), items[i].FileName);
                    }
                }

                DeleteDirectory(path, dirName);
                return true;
            }
        }

        public static bool DeleteFile(string path, string fileName)
        {
            // Check if file exists
            if (!FileExists(path, fileName))
            {
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

        public static bool ChangeFilePermission(string path, string fileName, int permission)
        {
            FtpSocket socket = new FtpSocket(Host, User, Pass, 21);

            // Chmod
            socket.Command("SITE CHMOD " + permission + " " + Utility.FormatPath(path, fileName));

            Console.WriteLine(socket.GetResponse());
            Console.WriteLine(socket.GetResponseCode());

            return true;
        }

        public static bool CreateDirectory(string path, string dirName)
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
                Console.WriteLine("Error in CreateDirectory(): " + ex.Message.ToString());
                return false;
            }
        }

        /*
        public static bool DirectoryExists(string path, string dirName)
        {
            // edited
            bool returnValue = false;

            System.Windows.MessageBox.Show("DirectoryExists('"+path+"','"+dirName+"')");


            try
            {
                FtpWebRequest request = GetRequest(RequestMethods.ListDir, path);
                WebResponse response = request.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream());
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (dirName == "")
                        break;

                    if (line.Length > 0 && line[0] != '/')
                        line = "/" + line;

                    System.Windows.MessageBox.Show(Utility.FormatPath(path, dirName) + " == " + line);

                    if (Utility.FormatPath(path, dirName) == line)
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
                System.Windows.MessageBox.Show("Error in DirectoryExists(): " + ex.Message.ToString());
            }

            return returnValue;
        }
        */

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
                /* Check if current directory exists
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
                }*/
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

        public static bool Download(string fromPath, string fromFile, string toPath, string toFile, string filesize = "")
        {
            // Check if from file exists
            if (!FileExists(fromPath, fromFile))
            {
                Console.WriteLine("File '" + fromFile + "' does not exist!");
                return false;
            }
         
            // Check if to file already exists
            if (File.Exists(toPath + toFile))
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
                if (Local.WriteFile(toPath, toFile, responseStream, Convert.ToInt32(filesize)))
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

        public static bool Upload(string fromPath, string fromFile, string toPath, string toFile)
        {
            System.Windows.MessageBox.Show("Upload('"+fromPath+"','"+fromFile+"','"+toPath+"','"+toFile+"')");

            // Check if from file exists
            if (!File.Exists(fromPath + fromFile))
            {
                Console.WriteLine("File '" + toFile + "' does not exist on local drive!");
                //return false;
            }

            // Check if to file already exists
            if (File.Exists(fromPath + fromFile))
            {
                Console.WriteLine("File '" + toFile + "' already exists on server!");
                //return false;
            }

            try
            {
                FileInfo fileinfo = new FileInfo(fromPath + fromFile); //added

                FtpWebRequest request = GetRequest(RequestMethods.Upload, toPath + toFile);
                request.KeepAlive = false; //added
                request.ContentLength = fileinfo.Length; //added

                /* Added: buffer size */
                int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int contentLength;

                /* Added File/transfer streams */
                FileStream filestream = fileinfo.OpenRead();
                Stream stream = request.GetRequestStream();
                contentLength = filestream.Read(buffer, 0, bufferLength);

                /* loop through streaming */
                long loadSize = 0;
                while (contentLength != 0)
                {
                    // Write Content from the file stream to the FTP Upload Stream
                    loadSize += contentLength;
                    Console.Write(".");

                    stream.Write(buffer, 0, contentLength);
                    contentLength = filestream.Read(buffer, 0, bufferLength);

                    int uploadProgress = (int)(loadSize * 100 / filestream.Length);
                    MainWindow.bgwU.ReportProgress(uploadProgress);
                }

                stream.Close();
                filestream.Close();
                System.Windows.MessageBox.Show("Completed upload!", "Dude");

                /*
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
                */ 
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
}