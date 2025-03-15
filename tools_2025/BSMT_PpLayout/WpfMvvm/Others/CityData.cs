using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BSMT_PpLayout
{
    class CityData
    {
        internal string CityName;
        internal double pSHeight;// 停车位 族类型 高度
        internal double pSWidth;// 停车位 族类型 宽度

        internal double miniPSHeight;// 微型
        internal double miniPSWidth;// 微型

        internal double pSHeight_Hor; // 平行式停车
        internal double pSWidth_Hor; // 平行式停车    
        internal double Wd_pri;// 停车位 主车道 宽度
        internal double Wd_sec;// 停车位 次车道 宽度

        internal Dictionary<string, List<CityData>> GetCityData()
        {
            // 获取插件所在位置
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            path = Path.GetDirectoryName(path);

            //// 通过插件所在位置提取数据库
            path = @"W:\BIM_ARCH\03.插件\goa tools 当前版本\Content\goa_tools";
            StreamReader sr = new StreamReader(path + @"\Resources\地库强排各地基础规范数据.csv", Encoding.Default);

            String line;
            Dictionary<string, List<CityData>> keyValuePairs = new Dictionary<string, List<CityData>>();
            string key_province = "";
            List<CityData> vlue_citys = new List<CityData>();

            while ((line = sr.ReadLine()) != null)
            {
                if (line.Substring(0, 1) == "【")
                {
                    string[] strs = line.Split(',');
                    string strProvince = strs[0];
                    int strProvinceCount = strProvince.Count();
                    key_province = line.Substring(1, strProvinceCount - 3);// 获取省份

                    vlue_citys = new List<CityData>();
                    keyValuePairs[key_province] = vlue_citys;// 添加空的键值对

                }
                else if (line.Substring(0, 2) == "城市")
                {
                    string[] strs = line.Split(',');
                    string tempStr = strs[1];// 城市名

                    string cityName = tempStr;

                    #region 将城市名的前面的省份字符串进行删除
                    int provinceNameCount = key_province.Count();
                    if (cityName.Substring(0, provinceNameCount) == key_province)
                    {
                        cityName = cityName.Substring(provinceNameCount, cityName.Count() - provinceNameCount);
                    }

                    #endregion

                    tempStr = strs[2];// 垂直式停车 长
                    double pSHeight;
                    double.TryParse(tempStr, out pSHeight);
                    tempStr = strs[3];// 垂直式停车 宽
                    double pSWidth;
                    double.TryParse(tempStr, out pSWidth);

                    tempStr = strs[4];// 微型停车 长
                    double miniPSHeight;
                    double.TryParse(tempStr, out miniPSHeight);
                    tempStr = strs[5];// 微型停车 宽
                    double miniPSWidth;
                    double.TryParse(tempStr, out miniPSWidth);

                    tempStr = strs[6];// 平行式停车 长
                    double pSHeight_Hor;
                    double.TryParse(tempStr, out pSHeight_Hor);
                    tempStr = strs[7];// 平行式停车 宽
                    double pSWidth_Hor;
                    double.TryParse(tempStr, out pSWidth_Hor);

                    tempStr = strs[8];// 主通车道 长
                    double Wd_pri;
                    double.TryParse(tempStr, out Wd_pri);
                    tempStr = strs[9];// 次通车道 宽
                    double Wd_sec;
                    double.TryParse(tempStr, out Wd_sec);

                    CityData cityData = new CityData();
                    cityData.CityName = cityName;
                    cityData.pSHeight = pSHeight;
                    cityData.pSWidth = pSWidth;
                    cityData.miniPSHeight = miniPSHeight;
                    cityData.miniPSWidth = miniPSWidth;

                    cityData.pSHeight_Hor = pSHeight_Hor;
                    cityData.pSWidth_Hor = pSWidth_Hor;
                    cityData.Wd_pri = Wd_pri;
                    cityData.Wd_sec = Wd_sec;
                    vlue_citys.Add(cityData);
                }
            }
            sr.Close();
            return keyValuePairs;
        }
    }
}

