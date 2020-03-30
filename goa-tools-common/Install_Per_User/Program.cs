using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace FolderDeployer
{
    public static class Deployer
    {
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            int i;
            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException(string.Concat("Source directory does not exist or could not be found: ", sourceDirName));
            }
            DirectoryInfo[] directories = directoryInfo.GetDirectories();
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = directoryInfo.GetFiles();
            for (i = 0; i < (int)files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                string str = Path.Combine(destDirName, fileInfo.Name);
                fileInfo.CopyTo(str, true);
            }
            if (copySubDirs)
            {
                DirectoryInfo[] directoryInfoArray = directories;
                for (i = 0; i < (int)directoryInfoArray.Length; i++)
                {
                    DirectoryInfo directoryInfo1 = directoryInfoArray[i];
                    string str1 = Path.Combine(destDirName, directoryInfo1.Name);
                    Deployer.DirectoryCopy(directoryInfo1.FullName, str1, copySubDirs);
                }
            }
        }

        private static string findLineByLabel(string textFilePath, string label)
        {
            string str = null;
            using (StreamReader streamReader = new StreamReader(textFilePath))
            {
                while (true)
                {
                    string str1 = streamReader.ReadLine();
                    string str2 = str1;
                    if (str1 == null)
                    {
                        break;
                    }
                    if (str2.Contains(label))
                    {
                        str = str2;
                    }
                }
            }
            str = str.Remove(0, label.Length);
            return str;
        }

        private static List<string> findLinesByLabel(string textFilePath, string label)
        {
            List<string> list = new List<string>();
            using (StreamReader streamReader = new StreamReader(textFilePath))
            {
                while (true)
                {
                    string str = null;
                    string str1 = streamReader.ReadLine();
                    string str2 = str1;
                    if (str1 == null)
                    {
                        break;
                    }
                    if (str2.Contains(label))
                    {
                        str = str2;
                        str = str.Remove(0, label.Length);
                        list.Add(str);
                    }
                }
            }
            return list;
        }

        public static void Main()
        {
            string str = ".\\Config.ini";
            string str1 = Deployer.findLineByLabel(str, "[AppName]");
            string str2 = Deployer.findLineByLabel(str, "[SourceDir]");
            var destDirList = Deployer.findLinesByLabel(str, "[DestDir]");
            string str4 = Deployer.findLineByLabel(str, "[Message]");
            destDirList = destDirList
                .Select(x => x.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
                .ToList();

            deleteOldPath();

            try
            {
                foreach (var destDir in destDirList)
                {
                    Deployer.DirectoryCopy(str2, destDir, true);
                }
                MessageBox.Show(string.Concat(str1, str4));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private static void deleteOldPath()
        {
            string oldPath = @"C:\ProgramData\Autodesk\ApplicationPlugins\goa tools.bundle";
            try
            {
                if (Directory.Exists(oldPath))
                {
                    Directory.Delete(oldPath, true);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
    }
}