using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
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
                    val = f.Name + "is " + f.Length/1048576 + " GB in size";
                    return val;
                }
                else if (f.Length >= 1048576)
                {
                    val = f.Name + "is " + f.Length/1048576 + " MB in size";
                    return val;
                }
                else if (f.Length >= 1024)
                {
                    val = f.Name + "is " + f.Length/1024 + " KB in size";
                    return val;
                }
                else
                {
                     val = f.Name + "is " + f.Length + " bytes in size";
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


        static void Main(string[] args)
        {
            decimal size;
            CreateDirectory("test");
            CreateDirectory("test2");
            CreateFile("test", "file1.txt");
            CreateFile("test", "file2.txt");
            CreateFile("test2", "oldFile.txt");
            CreateFile("test2", "newFile.txt");
            Console.WriteLine("FINDING FILE SIZES");
            getFileSize("test");
            if (size >= 1073741824)
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
            ListFiles("test");
            //RenameFile("test", "oldFile.txt", "newFile.txt");
            //MoveFile("test", "test2", "file1.txt", "file1.txt");
            Console.ReadKey();
            //DeleteFile("test", "file1");
            //DeleteDirectory("test");
            //DeleteDirectory("test2");
        }
    }
}