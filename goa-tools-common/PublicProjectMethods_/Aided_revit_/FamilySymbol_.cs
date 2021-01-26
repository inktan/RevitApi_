using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicProjectMethods_
{
    public static class FamilySymbol_
    {
        public static FamilySymbol FindFamilySymbol(this Document doc, string FiamlySymbolName, string FamilyName, string FamilyFilePath, BuiltInCategory builtInCategory)
        {
            FamilySymbol parkingType = null;
            Family parkFamily = null;
            bool symbolFound = FindFamilySymbol(doc, FiamlySymbolName, FamilyName, out parkingType, builtInCategory);//寻找目标停车位族类型            
            if (!symbolFound)
            {
                ReLoadFamily(doc, FamilyFilePath, out parkFamily);
                symbolFound = FindFamilySymbol(doc, FiamlySymbolName, FamilyName, out parkingType, builtInCategory);//寻找目标停车位族类型            

                //throw new NotImplementedException("找不到族：" + "\n" + "族路径：" + FamilyFilePath + "\n" + "族名：" + FamilyName + "\n" + "族类型名：" + FiamlySymbolName);
            }
            return parkingType;
        }
        public static bool FindFamilySymbol(Document doc, string FiamlySymbolName, string FamilyName, out FamilySymbol targetFamilySymbal, BuiltInCategory builtInCategory)
        {
            List<FamilySymbol> symbols = new FilteredElementCollector(doc).OfCategory(builtInCategory).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
            bool symbolFound = false;
            targetFamilySymbal = null;
            foreach (FamilySymbol element in symbols)
            {
                // 族名称与族类型名称，二者需要同时满足       
                if (element.FamilyName == FamilyName && element.Name == FiamlySymbolName)
                {
                    symbolFound = true;
                    targetFamilySymbal = element;
                    break;
                }
            }
            return symbolFound;
        }

        /// <summary>
        /// 通过两层方法(如果当前文档不存在目标name族，则载入族文件)，确定当前文档，存在停车位族
        /// </summary>
        public static FamilySymbol FamilySymbolByNamePath(this Document doc, string faPath, string fsNmae, string faName)
        {
            FamilySymbol fs = null;
            fs = doc.FamilySymbolByName(fsNmae, faName);//寻找目标停车位族类型
            if (fs == null)
            {
                Family fa = null;
                bool loadFamily = ReLoadFamily(doc, faPath, out fa);
                fs = doc.FamilySymbolByName(fsNmae, faName);//寻找目标停车位族类型
            }
            return fs;
        }
        /// <summary>
        /// 通过name进行全文档索引需要的族类型
        /// </summary>
        public static FamilySymbol FamilySymbolByName(this Document doc, string fsName, string faName)
        {
            FilteredElementCollector parkingSymbols = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol));
            FamilySymbol fs = null;
            foreach (Element element in parkingSymbols)
            {
                if (element is FamilySymbol)
                {
                    if (element.Name == fsName)
                    {
                        FamilySymbol familySymbol = element as FamilySymbol;
                        if (familySymbol.FamilyName == faName)
                        {
                            fs = element as FamilySymbol;
                            break;
                        }
                    }
                }
            }
            return fs;
        }
        /// <summary>
        /// 通过文件路径字符串，将族载入到当前文档 如果族已经存在 则载入失败
        /// </summary>
        public static bool ReLoadFamily(this Document doc, string FamilyPath, out Family family)
        {
            family = null;
            bool loadSuccess = false;
            using (Transaction loadFamily = new Transaction(doc))
            {
                loadFamily.Start("loadFamily");
                projectFamLoadOption pjflo = new projectFamLoadOption();
                loadSuccess = doc.LoadFamily(FamilyPath, pjflo, out family);//经过测试
                if (loadSuccess)
                {
                    foreach (ElementId parkingTypeId in family.GetValidTypes())//该函数无效，获取不出一个family的族类型；
                    {
                        FamilySymbol parkingTypeName = doc.GetElement(parkingTypeId) as FamilySymbol;
                        if (parkingTypeName != null)
                        {
                            //CMD.TestList.Add(parkingTypeName.Name);
                        }
                    }
                }
                loadFamily.Commit();
            }
            return loadSuccess;
        }
        /// <summary>
        /// 载入族提示是否要覆盖族参数
        /// </summary>
        internal class projectFamLoadOption : IFamilyLoadOptions
        {
            bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
                // throw new NotImplementedException();
            }

            bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Project;
                overwriteParameterValues = true;
                return true;
                // throw new NotImplementedException();
            }
        }
    }
}
