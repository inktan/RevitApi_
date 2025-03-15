using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class FileUtils
    {
        [System.Runtime.InteropServices.DllImport(@"C:\Windows\System32\kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [System.Runtime.InteropServices.DllImport(@"C:\Windows\System32\kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public static IntPtr HFILE_ERROR = new IntPtr(-1);

        public static bool DeleteFile(string filePath)
        {
            IntPtr vHandle = _lopen(filePath, OF_READWRITE | OF_SHARE_DENY_NONE);

            if (!File.Exists(filePath))
            {
                //"文件不存在".TaskDialogErrorMessage();
                return true;
            }
            if (vHandle == HFILE_ERROR)// 检测文件是否被占用
            {
                //"文件被占用！".TaskDialogErrorMessage();
                return false;
            }
            CloseHandle(vHandle);
            File.Delete(filePath);
            return true;
        }
        /// <summary>
        /// 删除revit 后缀为 .00**. 备份文件
        /// </summary>
        /// <param name="docPath">revit文档的</param>
        public static void DelBackupFiles(this string docPath)
        {
            DirectoryInfo path = new DirectoryInfo(docPath);
            path = new DirectoryInfo(path.Parent.FullName);
            foreach (var item in path.GetFiles())
            {
                string fullName = item.FullName;
                if (fullName[fullName.Length - 8] == '0' && fullName[fullName.Length - 9] == '.')
                {
                    DeleteFile(fullName);
                }
            }
        }
        /// <summary>
        /// 获取一个文件夹中及其子文件夹中的所有文件路径
        /// </summary>
        /// <param name="folderPaht"></param>
        /// <returns></returns>
        public static List<string> GetAllFilesPath(this string folderPaht)
        {
            List<string> fileNames = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(folderPaht);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo f in files)
            {
                fileNames.Add(f.FullName);//添加文件的路径到列表
            }

            //获取子文件夹内的文件列表，递归遍历
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                fileNames.AddRange(GetAllFilesPath(d.FullName));
            }
            return fileNames;
        }

        public static IEnumerable<string> GetFoldersPath(this string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            DirectoryInfo[] folders = dir.GetDirectories();

            foreach (DirectoryInfo d in folders)
            {
                yield return d.FullName;
            }
        }
        /// <summary>
        /// 将文件备份为临时文件夹的临时问价
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="outputFilePath"></param>
        public static string CopyFileToTempPath(this string sourceFilePath)
        {
            var sourceFileNameExtension = Path.GetExtension(sourceFilePath);
            var tempFilePath = Path.GetTempFileName();
            string outputFilePath = Path.ChangeExtension(tempFilePath, sourceFileNameExtension);
            File.Copy(sourceFilePath, outputFilePath, true);
            return outputFilePath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="filePath"></param>
        public static void ToTxt(this string value, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite), Encoding.UTF8))
            {
                sw.Write(value);
                sw.Close();
            }
        }
    }
}
