using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace FolderDeployer
{
    public static class Deployer
    {
        public static void Main()
        {
            // 找到当前
            string str = ".\\Config.ini";
            string str1 = Deployer.findLineByLabel(str, "[AppName]");// goa tools
            string str2 = Deployer.findLineByLabel(str, "[SourceDir]");// .\Content 最新dll文件所在目录
            var destDirList = Deployer.findLinesByLabel(str, "[DestDir]");
            string str4 = Deployer.findLineByLabel(str, "[Message]");// 已成功安装

            // 对所有的 %AppData%\Autodesk\Revit\Addins\2020 目录进行循环
            destDirList = destDirList
                .Select(x => x.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)))
                //替换为 C:\Users\wang.tan\AppData\Roaming
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

        /// <summary>
        /// 从文本第一行进行遍历搜索，找到含有标签的首行
        /// </summary>
        /// <param name="textFilePath">文件路径</param>
        /// <param name="label">标签</param>
        /// <returns></returns>
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
                        break;
                    }
                }
            }
            str = str.Remove(0, label.Length);
            return str;
        }
        /// <summary>
        /// 从文本第一行进行遍历搜索，找到含有标签的所有行
        /// </summary>
        /// <param name="textFilePath"></param>
        /// <param name="label"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 删除--
        /// </summary>
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
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            int i;
            DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
            if (!directoryInfo.Exists)// 判断源dll文件夹是否存在
            {
                throw new DirectoryNotFoundException(string.Concat("Source directory does not exist or could not be found: ", sourceDirName));
            }
     
            if (!Directory.Exists(destDirName))// 判断是否存在路径，不存在则创建
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = directoryInfo.GetFiles();// 获取所有源dll文件，不包含子文件夹

            for (i = 0; i < (int)files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                string str = Path.Combine(destDirName, fileInfo.Name);
                Console.WriteLine(str);
                fileInfo.CopyTo(str, true);// 这里使用的全文件路径
            }
            // 判断在源dll文件夹中，是否存在子文件夹，如果存在使用递归进行复制

            if (copySubDirs)
            {
                DirectoryInfo[] directories = directoryInfo.GetDirectories();// 获取子文件夹路径

                DirectoryInfo[] directoryInfoArray = directories;
                for (i = 0; i < (int)directoryInfoArray.Length; i++)
                {
                    DirectoryInfo directoryInfo1 = directoryInfoArray[i];
                    string str1 = Path.Combine(destDirName, directoryInfo1.Name);
                    Deployer.DirectoryCopy(directoryInfo1.FullName, str1, copySubDirs);
                }
            }
        }
     

    }
}