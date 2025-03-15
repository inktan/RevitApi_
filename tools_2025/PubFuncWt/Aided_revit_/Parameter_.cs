using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Parameter_
    {
        #region Parameter
        public static Parameter ApiGuid(this FamilySymbol familySymbol)
        {
            Parameter parameter = familySymbol.get_Parameter(new Guid("679a258b-764c-481b-bd7d-0f543c298849"));
            return parameter;
        }
        /// <summary>
        /// 基于指定字符串，寻找参数
        /// </summary>
        public static Parameter GetParaByName_(this Element _elem, string _name)
        {
            if (_elem is FamilyInstance)
            {
                FamilyInstance fi = _elem as FamilyInstance;
                FamilySymbol fs = fi.Symbol;
                foreach (Parameter p in fi.Parameters)
                {
                    if (p.Definition.Name == _name)// 先找实例参数
                        return p;
                }
                foreach (Parameter p in fs.Parameters)// 再找类型参数
                {
                    if (p.Definition.Name == _name)
                        return p;
                }
            }
            else if (_elem is Wall)
            {
                Wall fi = _elem as Wall;
                WallType fs = fi.WallType;
                foreach (Parameter p in fi.Parameters)// 先找实例参数
                {
                    if (p.Definition.Name == _name)
                        return p;
                }
                foreach (Parameter p in fs.Parameters)// 再找类型参数
                {
                    if (p.Definition.Name == _name)
                        return p;
                }
            }

            return null;
        }
        /// <summary>
        /// 基于指定字符串，寻找参数 
        /// --异常显示 族样板中的已存在默认类别尺寸参数，将其修改为实例后，通过BuiltInParameter方式，仍可以获取非null的Parameter，但是该参数存储数据为空值
        /// </summary>
        /// <returns></returns>
        public static Parameter GetParameterByBuiltInParameter(this Element _elem, BuiltInParameter builtInParameter)
        {
            if (_elem is FamilyInstance)
            {
                if (_elem.Id == new ElementId(336349))
                {
                    Line line = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(0, 0, 0));
                    string test = "成功进入";
                    test.TaskDialogErrorMessage();
                }
                FamilyInstance fi = _elem as FamilyInstance;
                FamilySymbol fs = fi.Symbol;

                Parameter parameter = fs.get_Parameter(builtInParameter);
                if (parameter == null)
                {
                    return fi.get_Parameter(builtInParameter);
                }
                else
                {
                    return parameter;
                }

            }
            else if (_elem is Wall)
            {
                Wall fi = _elem as Wall;
                WallType fs = fi.WallType;

                Parameter parameter = fs.get_Parameter(builtInParameter);
                if (parameter == null)
                {
                    return fi.get_Parameter(builtInParameter);
                }
                else
                {
                    return parameter;
                }
            }

            return null;
        }

        #endregion
    }
}
