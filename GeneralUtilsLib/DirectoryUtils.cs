using System.Collections.Generic;
using System.Linq;

using System.IO;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System;

namespace GeneralUtils
{
    public static class DirectoryUtils
    {

        /// <summary>
        /// Rather than deleting the folder, remove all the contents of the folder and keep the folder intact.
        /// </summary>
        /// <param name="folderToClean">The directory to clean, can be null, empty, or whitespaces.</param>
        public static void Clean(string folderToClean)
        {
            if (string.IsNullOrWhiteSpace(folderToClean)
                || folderToClean.Equals("c:", StringComparison.OrdinalIgnoreCase)
                || folderToClean.Equals("c:\\", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (!Directory.Exists(folderToClean))
            {
                return;
            }

            // rather than deleting directories, clean the files and subdirectories
            // Directory.Delete(folderToDelete, true);

            string[] filePaths = Directory.GetFiles(folderToClean);
            foreach (string filePath in filePaths)
                File.Delete(filePath);

            string[] dirPaths = Directory.GetFileSystemEntries(folderToClean);
            foreach (string dirPath in dirPaths)
                Directory.Delete(dirPath, true);
        }


        /// <summary>
        /// Copies a directory to another location.  Folders will be created, and files overwritten if they already exist.
        /// </summary>
        /// <remarks>
        /// This method will throw ArgumentNullException if null is passed in for either parameter.
        /// </remarks>
        /// <param name="sourceFolder"> The relative or absolute path to the directory to copy from. This string is not case-sensitive.</param>
        /// <param name="targetFolder"> The relative or absolute path to the directory to copy to. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.
        /// The search is case-insensitive, *.csv & *.CSV return the same results. 
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, 
        /// but doesn't support regular expressions.</param>
        public static int CopyDirectory(string sourceFolder, string targetFolder, [Optional, DefaultParameterValue("*.*")] string searchPattern)
        {
            // parameter checking
            MethodUtils.NotNullOrEmpty(sourceFolder, "sourceFolder");
            MethodUtils.NotNullOrEmpty(targetFolder, "targetFolder");

            // verify the source folder
            DirectoryInfo dir = new DirectoryInfo(sourceFolder);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceFolder);
            }

            // create the target folder if the folder does not exist.
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceFolder, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceFolder, targetFolder));

            // Copy all the files & replaces any files with the same name
            int count = 0;
            string searach = (string.IsNullOrWhiteSpace(searchPattern)) ? "*.*" : searchPattern;
            foreach (string srcFile in Directory.GetFiles(sourceFolder, searach, SearchOption.AllDirectories))
            {
                File.Copy(srcFile, srcFile.Replace(sourceFolder, targetFolder), true);
                count++;
            }
            return count;
        }


        /// <summary>
        /// Moves files from 1 directory to another location.  Folders will be created, and files overwritten if they already exist.
        /// CopyDirectory keeps the orginal, move actual moves the orginal
        /// </summary>
        /// <remarks>
        /// This method will throw ArgumentNullException if null is passed in for either parameter.
        /// </remarks>
        /// <param name="sourceFolder"> The relative or absolute path to the directory to copy from. This string is not case-sensitive.</param>
        /// <param name="targetFolder"> The relative or absolute path to the directory to copy to. This string is not case-sensitive.</param>
        /// <param name="searchPattern">The search string to match against the names of files in path.
        /// The search is case-insensitive, *.csv & *.CSV return the same results. 
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, 
        /// but doesn't support regular expressions.</param>
        public static int MoveDirectory(string sourceFolder, string targetFolder, [Optional, DefaultParameterValue("*.*")] string searchPattern)
        {
            // parameter checking
            MethodUtils.NotNullOrEmpty(sourceFolder, "sourceFolder");
            MethodUtils.NotNullOrEmpty(targetFolder, "targetFolder");

            // verify the source folder
            DirectoryInfo dir = new DirectoryInfo(sourceFolder);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceFolder);
            }

            // create the target folder if the folder does not exist.
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourceFolder, "*",
                SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourceFolder, targetFolder));

            // Copy all the files & replaces any files with the same name
            int count = 0;
            string searach = (string.IsNullOrWhiteSpace(searchPattern)) ? "*.*" : searchPattern;
            foreach (string srcFile in Directory.GetFiles(sourceFolder, searach, SearchOption.AllDirectories))
            {
                File.Move(srcFile, srcFile.Replace(sourceFolder, targetFolder));
                count++;
            }
            return count;
        }


        /// <summary>
        /// Deletes the specified directory and any subdirectories and files, never throwing an exeception.  
        /// Checks to see if the directory exist prior to deletion.
        /// </summary>
        /// <param name="fileName">The directory to delete, can be null, empty, or whitespaces.</param>
        public static bool DeleteQuietly(string directoryName)
        {
            bool result = true;
            if (!string.IsNullOrWhiteSpace(directoryName))
            {
                try
                {
                    DeleteAll(directoryName);
                }
                catch (Exception)
                {
                    // ignore
                    result = false;
                }
            }
            return result;
        }

        /// <summary>
        /// Deletes the specified directory and any subdirectories and files.  
        /// Checks to see if the directory exist prior to deletion.
        /// </summary>
        /// <param name="fileName">The directory to delete.  Must be a valid directory.</param>
        public static void DeleteAll(string directoryName)
        {
            MethodUtils.NotNullOrEmpty(directoryName, "directoryName cannot be null or empty");
            if (Directory.Exists(directoryName))
            {
                Clean(directoryName);
                Directory.Delete(directoryName, true);
            }
        }


        /// <summary>
        /// Compares 1 directory to another directory.  Compares folder structure, file name and size, no file content checking.
        /// </summary>
        /// <remarks>
        /// This method will throw ArgumentNullException if null is passed in for either parameter.
        /// </remarks>
        /// <returns>
        /// Returns true if both folders are identical, else false.
        /// </returns>
        /// <param name="folder1">folder 1</param>
        /// <param name="folder2">folder 2</param>
        public static bool CompareDirectories(string folder1, string folder2, [Optional, DefaultParameterValue("*.*")] string searchPattern)
        {
            // parameter checking
            MethodUtils.NotNullOrEmpty(folder1, "folder1");
            MethodUtils.NotNullOrEmpty(folder2, "folder2");

            DirectoryInfo dir1 = new DirectoryInfo(folder1);
            if (!dir1.Exists)
            {
                return false;
            }
            DirectoryInfo dir2 = new DirectoryInfo(folder2);
            if (!dir2.Exists)
            {
                return false;
            }

            string searach = (string.IsNullOrWhiteSpace(searchPattern)) ? "*.*" : searchPattern;

            IEnumerable<System.IO.FileInfo> sourceList = dir1.GetFiles(searach, System.IO.SearchOption.AllDirectories);
            IEnumerable<System.IO.FileInfo> targetList = dir2.GetFiles(searach, System.IO.SearchOption.AllDirectories);

            // This query determines whether the two folders contain  
            // identical file lists, based on the custom file comparer  
            // that is defined in the FileCompare class.  
            return sourceList.SequenceEqual(targetList, new FileCompare());
        }


        /// <summary>
        /// Verify folder exist, and that the calling application can read & write to folder.
        /// </summary>
        /// <remarks>
        ///  Returns false if something is wrong, else true.
        ///  This method will throw ArgumentNullException if null is passed in.
        ///  This method uses CheckWritePermissionOnDir & CheckReadPermissionOnDir
        /// </remarks>
        /// <param name="path">folder path</param>
        public static bool VerifyFolder(string path)
        {
            MethodUtils.NotNullOrEmpty(path, "path");
            if (!Directory.Exists(path))
            {
                return false;
            }

            if (!CheckReadPermissionOnDir(path))
            {
                return false;
            }

            if (!CheckWritePermissionOnDir(path))
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Verify that the application has write permissions to the folder.
        /// </summary>
        /// <remarks>
        ///  Returns true if the application can write to the folder, else false.
        ///  This method will throw ArgumentNullException if null is passed in.
        /// </remarks>
        /// <param name="path">folder path</param>
        public static bool CheckWritePermissionOnDir(string path)
        {
            MethodUtils.NotNullOrEmpty(path, "path");

            var writeAllow = false;
            var writeDeny = false;

            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity accessControlList = dInfo.GetAccessControl();
            if (accessControlList == null)
            {
                return false;
            }

            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
            {
                return false;
            }

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    writeAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    writeDeny = true;
                }
            }

            return writeAllow && !writeDeny;
        }


        /// <summary>
        /// Verify that the application has read permissions to the folder.
        /// </summary>
        /// <remarks>
        ///  Returns true if the application can read from the folder, else false.
        ///  This method will throw ArgumentNullException if null is passed in.
        /// </remarks>
        /// <param name="path">folder path</param>
        public static bool CheckReadPermissionOnDir(string path)
        {
            MethodUtils.NotNullOrEmpty(path, "path");

            var readAllow = false;
            var readDeny = false;

            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity accessControlList = dInfo.GetAccessControl();
            if (accessControlList == null)
            {
                return false;
            }
            var accessRules = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
            {
                return false;
            }

            foreach (FileSystemAccessRule rule in accessRules)
            {
                if ((FileSystemRights.Read & rule.FileSystemRights) != FileSystemRights.Read)
                {
                    continue;
                }

                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    readAllow = true;
                }
                else if (rule.AccessControlType == AccessControlType.Deny)
                {
                    readDeny = true;
                }
            }

            return readAllow && !readDeny;
        }

        // This implementation defines a very simple comparison  between two FileInfo objects. 
        class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
        {
            public FileCompare() { }

            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                if (f1 == null && f2 == null)
                {
                    return true;
                }
                
                if (f1 != null && f2 == null)
                {
                    return false;
                }

                if (f1 == null && f2 != null)
                {
                    return false;
                }
                return (f1.Name == f2.Name
                    && f1.Length == f2.Length);
            }

            public int GetHashCode(System.IO.FileInfo fi)
            {
                string s = $"{fi.Name}{fi.Length}";
                return s.GetHashCode();
            }
        }
    }
}
