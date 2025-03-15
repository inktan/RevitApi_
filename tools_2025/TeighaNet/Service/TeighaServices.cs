using System;
using System.IO;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using PubFuncWt;
using Teigha.DatabaseServices;
using Teigha.Runtime;

namespace TeighaNet
{
    /// <summary>
    /// Teigha库运行时环境服务器
    /// </summary>
    public class TeighaServices
    {
        //private static TeighaServices _teighaServices;

        //public static TeighaServices Instance
        //{
        //    get
        //    {
        //        if (_teighaServices == null) _teighaServices = new TeighaServices();

        //        return _teighaServices;
        //    }
        //}
        public TeighaServices(UIApplication uiApp)
        {
            // 外部加载dll文件
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string dllPath = currentAssembly.Location;

            string directoryPath = Path.GetDirectoryName(dllPath);

            string assemblyPaht = directoryPath + @"\Teigha_Net64\TD_Mgd.dll";//引用位置
            assemblyPaht = @"W:\BIM_ARCH\03.插件\goa tools 精简版\Content\goa_tools_arch_min\Teigha_Net64\TD_Mgd.dll";
            try
            {
                Assembly.UnsafeLoadFrom(assemblyPaht);
            }
            catch
            {
                try
                {
                    byte[] assemblyBuffer = File.ReadAllBytes(assemblyPaht);
                    Assembly a = Assembly.Load(assemblyBuffer);
                }
                catch { }
            }
        }

        private string tempDwgPath;
        /// <summary>
        /// 将链接cad复制到临时文件
        /// </summary>
        public string TempDwgPath
        {
            get { return tempDwgPath; }
            set
            {
                this.tempDwgPath = value.CopyFileToTempPath();

                if (!File.Exists(this.tempDwgPath))
                {
                    throw new NotImplementedException(value + "所选链接文件，转存为临时文件，失败。");
                }
            }
        }

        /// <summary>
        /// Gets file version of give DWG file.
        /// </summary>
        /// <param name="dwgFile">DWG file to check.</param>
        public CADFileVersion GetDWGFileVersion()
        {
            string header = string.Empty;
            try
            {
                // open the file with share mode as sometimes the dwg may be openning by AutoCAD
                using (FileStream fs = new FileStream(this.tempDwgPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    StreamReader reader = new StreamReader(fs);
                    header = reader.ReadLine().ToUpper().Trim();
                    reader.Close();
                }
            }
            catch (Teigha.Runtime.Exception ex)
            {
                throw new NotImplementedException("获取" + this.tempDwgPath + "的第一行文字信息失败；" + ex.ToString());
            }

            // check file version
            // https://knowledge.autodesk.com/support/autocad/troubleshooting/caas/sfdcarticles/sfdcarticles/How-to-find-which-version-of-AutoCAD-was-used-to-create-save-a-DWG.html

            if (header.StartsWith("AC1015"))
                return CADFileVersion.R2000;
            else if (header.StartsWith("AC1018"))
                return CADFileVersion.R2004;
            else if (header.StartsWith("AC1021"))
                return CADFileVersion.R2007;
            else if (header.StartsWith("AC1024"))
                return CADFileVersion.R2010;
            else if (header.StartsWith("AC1027"))
                return CADFileVersion.R2013;
            else
                throw new NotImplementedException("CAD版本需要降低为为2000、2004、2007、2010、2013");

        }

        public DwgParser DwgParser { get; set; }

        public void ParseDwg()
        {
            GetDWGFileVersion();

            using (Services srv = new Services())
            {
                using (Database database = new Database(false, false))
                {
                    // ensure to open the dwg with share mode so that the dwg can be read when dwg is being opened by AutoCAD
                    database.ReadDwgFile(this.tempDwgPath, FileOpenMode.OpenForReadAndWriteNoShare, true, "");

                    this.DwgParser = new DwgParser(database);

                    // 是否可以外部调用，需要进一步测试
                    this.DwgParser.Parse();
                    this.DwgParser.Layers();
                }
            }
        }
    }
}