using Autodesk.Revit.DB;
using goa.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class FamilySymbol_
    {
        /// <summary>
        /// 使用该函数时候，判断族类型名是否符合要求，不符合要求，则复制一个族类型，修改对应的族参数
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="faPath"></param>
        /// <param name="fsName"></param>
        /// <param name="faName"></param>
        /// <returns></returns>
        public static FamilySymbol FamilySymbolByPath(this Document doc, string faPath, string fsName, string faName)
        {
            FamilySymbol fs = doc.FamilySymbolByName(fsName, faName);

            if (fs != null)
            {
                return fs;
            }

            Family fa = ReLoadFamily(doc, faPath);
            if (fa == null)
            {
                ("族路径：" + faPath + ",不存在，请联系BIM协调员。").TaskDialogErrorMessage();
            }

            return doc.FamilySymbolByName(fsName, faName);
        }
        /// <summary>
        /// 通过加载路径
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fsName"></param>
        /// <param name="faName"></param>
        /// <returns></returns>
        public static FamilySymbol FamilySymbolByName(this Document doc, string fsName, string faName)
        {
            FamilySymbol fs = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().Where(p => p.Name == fsName && p.FamilyName == faName).FirstOrDefault();
            if (fs != null)
            {
                return fs;
            }

            Family fa = doc.FamilyByName(faName);

            if (fa != null)// 
            {
                fs = fa.FamilySymbolByName(fsName);
                if (fs != null)
                {
                    return fs;
                }
                int count = fa.GetFamilySymbolIds().Count;
                if (count > 0)
                {
                    using (Transaction duplicateFs = new Transaction(doc))
                    {
                        duplicateFs.Start("duplicateFs");
                        fs = fa.GetSymbols().First().Duplicate(fsName) as FamilySymbol;
                        duplicateFs.Commit();
                    }
                    return fs;
                }
                else
                {
                    // 通过 族编辑器创建一个与族名相同的族类型
                    Document familyDoc = doc.EditFamily(fa);
                    FamilyManager familyManager = familyDoc.FamilyManager;

                    FamilyTypeSetIterator familyTypeSetIterator = familyManager.Types.ForwardIterator();
                    while (familyTypeSetIterator.MoveNext())
                    {
                        if ((familyTypeSetIterator.Current as FamilyType).Name == faName)
                        {
                            using (Transaction loadFamily = new Transaction(familyDoc))
                            {
                                loadFamily.Start("loadFamily");
                                projectFamLoadOption pjflo = new projectFamLoadOption();
                                fa = familyDoc.LoadFamily(doc, pjflo);
                                loadFamily.Commit();
                            }
                            return fa.FamilySymbolByName(faName);
                        }
                    }

                    using (Transaction creatFamilyType = new Transaction(familyDoc))
                    {
                        creatFamilyType.Start("creatFamilyType");
                        FamilyType newFamilyType = familyManager.NewType(faName);
                        creatFamilyType.Commit();
                    }
                    using (Transaction loadFamily = new Transaction(familyDoc))
                    {
                        loadFamily.Start("loadFamily");
                        projectFamLoadOption pjflo = new projectFamLoadOption();
                        fa = familyDoc.LoadFamily(doc, pjflo);
                        loadFamily.Commit();
                    }
                    return fa.FamilySymbolByName(faName);
                }
            }
            return null;
        }

        public static FamilySymbol FamilySymbolByName(this Family _fam, string fsName)
        {
            var doc = _fam.Document;
            return _fam.GetFamilySymbolIds().Select(x => doc.GetElement(x) as FamilySymbol).Where(x => x.Name == fsName).FirstOrDefault();
        }
        public static Family FamilyByName(this Document doc, string faName)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(Family)).Cast<Family>().Where(p => p.Name == faName).FirstOrDefault();
        }

        public static Family ReLoadFamily(this Document doc, string FamilyPath)
        {
            Family family = null;
            using (Transaction loadFamily = new Transaction(doc))
            {
                loadFamily.Start("loadFamily");
                projectFamLoadOption pjflo = new projectFamLoadOption();
                doc.LoadFamily(FamilyPath, pjflo, out family);//经过测试
                loadFamily.Commit();
            }
            return family;
        }

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
