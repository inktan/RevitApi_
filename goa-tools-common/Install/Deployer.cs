using System;
using System.IO;
using System.Windows.Forms;

namespace FolderDeployer
{
	public class Deployer
	{
		public Deployer()
		{
		}

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

		private static string FindLineByLabel(string textFilePath, string label)
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

		public static void Main()
		{
			string str = ".\\Config.ini";
			string str1 = Deployer.FindLineByLabel(str, "[AppName]");
			string str2 = Deployer.FindLineByLabel(str, "[SourceDir]");
			string str3 = Deployer.FindLineByLabel(str, "[DestDir]");
			string str4 = Deployer.FindLineByLabel(str, "[Message]");
			try
			{
				Deployer.DirectoryCopy(str2, str3, true);
				MessageBox.Show(string.Concat(str1, str4));
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
			}
		}
	}
}