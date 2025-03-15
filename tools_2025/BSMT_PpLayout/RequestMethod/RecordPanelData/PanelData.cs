using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    class PanelData
    {

        internal List<string> AllData { get; set; }

        internal PanelData()
        {
            AllData = new List<string>();
        }
        /// <summary>
        /// 读取所有标记属性的值
        /// </summary>
        internal void Read()
        {
            AllData = new List<string>();

            Type type = GlobalData.Instance.GetType();
            var members = type.GetProperties();
            var markedMembers = members.Where(m => m.GetCustomAttributes(typeof(PanelDataAttri), false).Length != 0);

            foreach (var markedMember in markedMembers)
            {
                var value = markedMember.GetValue(GlobalData.Instance);
                //GlobalData.Instance.strBackgroundMonitorDta += "\n" + markedMember.PropertyType;
                if (!ReferenceEquals(value, null))
                {
                    //GlobalData.Instance.strBackgroundMonitorDta += ":" + value;
                    AllData.Add(value.ToString());
                }
                else
                {
                    //GlobalData.Instance.strBackgroundMonitorDta += ":" + "---";
                    AllData.Add("---");
                }
            }
        }
        /// <summary>
        /// 将值写入标记属性
        /// </summary>
        internal void Write()
        {
            Type type = GlobalData.Instance.GetType();
            var members = type.GetProperties();
            var markedMembers = members.Where(m => m.GetCustomAttributes(typeof(PanelDataAttri), false).Length != 0).ToList();

            int count = AllData.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    string temp = AllData[i];
                    PropertyInfo propertyInfo = markedMembers[i];
                    string properType = propertyInfo.PropertyType.ToString();

                    if (properType == "System.Double")
                    {

                    }
                    else if (properType == "System.String")
                    {
                        if (temp == "---")
                        {
                            propertyInfo.SetValue(GlobalData.Instance, null);
                        }
                        else
                        {
                            propertyInfo.SetValue(GlobalData.Instance, temp);
                        }
                    }
                    else if (properType == "System.Boolean")
                    {
                        if (temp == "---")
                        {
                            propertyInfo.SetValue(GlobalData.Instance, null);
                        }
                        else
                        {
                            if (temp == "True")
                            {
                                propertyInfo.SetValue(GlobalData.Instance, true);

                            }
                            else if (temp == "False")
                            {
                                propertyInfo.SetValue(GlobalData.Instance, false);

                            }

                        }
                    }
                }
            }
        }
    }
}
