using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TransGroupParas
{
    public static class ParameterExtension_temp
    {
        /// <summary>
        /// get the meaningful value from parameter.
        /// return Element if storage type is ElementId
        /// return "error" text if any exception.
        /// </summary>
        /// <param name="_p"></param>
        /// <returns></returns>
        public static object GetMeaningfulValue_temp(this Parameter _p)
        {
            if (_p == null || !_p.HasValue)
            {
                //family type parameter could return -1 id and Not has value
                if (_p.StorageType == StorageType.ElementId)
                    return ElementId.InvalidElementId;
                else
                    return null;
            }

            switch (_p.StorageType)
            {
                case StorageType.None:
                    {
                        return null;
                    }
                case StorageType.ElementId:
                    {
                        var doc = _p.Element.Document;
                        var id = _p.AsElementId();//此处为空值
                        //some id parameter does not return value
                        if (id == null)
                            return _p.AsValueString();
                        //key parameter can return -1 id and has value
                        //showing "(none)" on UI
                        if (id.IntegerValue == -1)
                            return id;
                        else
                        {
                            var elem = doc.GetElement(id);
                            if (elem == null)
                                return _p.AsValueString();
                            else return elem;
                        }
                    }
                case StorageType.Integer:
                    {
                        return _p.AsInteger();
                    }
                case StorageType.String:
                    {
                        return _p.AsString();
                    }
                case StorageType.Double:
                    {
                        return _p.AsDouble(); //value with unit if applicable
                    }
                default:
                    {
                        return "< Error >";
                    }
            }
        }

        public static void SetValue_temp(this Parameter p, object value)
        {
            //special case for key parameter
            if (p.StorageType == StorageType.ElementId
                && value is ElementId)
            {
                var id = ((ElementId)value).IntegerValue;
                if (id == -1)
                {
                    p.Set((ElementId)value);
                    return;
                }
            }

            if (value == null)            
                value = ""; //此处需要特别注意

            if (value.GetType().Equals(typeof(string)))
            {
                if (p.SetValueString(value as string))
                    return;
            }

            switch (p.StorageType)
            {
                case StorageType.None:
                    break;
                case StorageType.Double:
                    if (value is string)
                    {
                        var valueString = value as string;
                        if (string.IsNullOrEmpty(valueString))
                            p.Set(0.0);
                        else
                            p.Set(double.Parse(valueString));
                    }
                    else
                    {
                        p.Set(Convert.ToDouble(value));
                    }
                    break;
                case StorageType.Integer:
                    if (value is string)
                    {
                        var valueString = value as string;
                        if (string.IsNullOrEmpty(valueString))
                            p.Set(0);
                        else
                            p.Set(int.Parse(valueString));
                    }
                    else
                    {
                        p.Set(Convert.ToInt32(value));
                    }
                    break;
                case StorageType.ElementId:
                    if (value is ElementId)
                    {
                        p.Set(value as ElementId);
                    }
                    else if (value is string)
                    {
                        var valueString = value as string;
                        if (string.IsNullOrEmpty(valueString))
                            p.Set(0);
                        else
                            p.SetValueString(valueString);//此处需要注意
                    }
                    else if (value is Element)
                    {
                        p.Set(((Element)value).Id);
                    }
                    else
                    {
                        p.Set(new ElementId(Convert.ToInt32(value)));
                    }
                    break;
                case StorageType.String:
                    p.Set(value.ToString());
                    break;
            }
        }

        public static bool ToBoolean(this string _string)
        {
            var trueList = new string[] { "True", "Yes", "1", "是" };
            var falseList = new string[] { "False", "No", "0", "否" };
            if (trueList.Contains(_string, StringComparer.InvariantCultureIgnoreCase))
                return true;
            else if (falseList.Contains(_string, StringComparer.InvariantCultureIgnoreCase))
                return false;
            else
                throw new FormatException("Failed to convert \"" + _string + "\" to boolean value.");
        }

        public static string GetId(this Parameter p)
        {
            string id = p.GetUniqueId();
            if (id == null)
                id = p.Id.ToString();
            return id;
        }

        public static string GetId(this FamilyParameter p)
        {
            string id = p.GetUniqueId();
            if (id == null)
                id = p.Id.ToString();
            return id;
        }

        /// <summary>
        /// one method for all types of parameters. 
        /// Project parameter does not have a unique identity, return null;
        /// </summary>
        public static string GetUniqueId(this Parameter p)
        {
            //shared parameter
            if (p.IsShared)
                return p.GUID.ToString();
            var iDef = p.Definition as InternalDefinition;
            //built-in parameter
            if (iDef.BuiltInParameter != BuiltInParameter.INVALID)
                return p.Id.ToString();
            //project parameter dose not have a unique identifier
            else
                return null;
        }

        /// <summary>
        /// family parameter, the same as parameter.
        /// </summary>
        public static string GetUniqueId(this FamilyParameter p)
        {
            //shared parameter
            if (p.IsShared)
                return p.GUID.ToString();
            var iDef = p.Definition as InternalDefinition;
            //built-in parameter
            if (iDef.BuiltInParameter != BuiltInParameter.INVALID)
                return p.Id.ToString();
            //project parameter dose not have a unique identifier
            else
                return null;
        }
    }
}
