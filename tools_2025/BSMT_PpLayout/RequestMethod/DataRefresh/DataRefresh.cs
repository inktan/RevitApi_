
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using g3;
using System.Diagnostics;
using PubFuncWt;
using System.Windows.Media;
using goa.Common;
using System.Data;
using System.IO;

namespace BSMT_PpLayout
{
    class DataRefresh : RequestMethod
    {

        public DataRefresh(UIApplication uiapp) : base(uiapp)
        {
        }
        internal override void Execute()
        {
            UpData(new ElementId(-1));
        }
        internal void Execute(ElementId elementId)
        {
            UpData(elementId);
        }

        /// <summary>
        /// 刷新统计数据
        /// </summary>
        private void UpData(ElementId elementId)
        {

            //清除无效详图组类型
            this.doc.DeleteInvalidDetailGroupTypes();

            List<ElementId> selBaseWallIds;
            List<ElemsViewLevel> elemsViewLevels;
            if (elementId != null && elementId.IntegerValue != -1)
            {
                Element ele = doc.GetElement(doc.GetElement(elementId).OwnerViewId);
                selBaseWallIds = new List<ElementId>() { elementId }; // UI获取所有地库填充区域id
                elemsViewLevels = new List<ElemsViewLevel>() { new ElemsViewLevel(ele) }; //一个视图对应一个层级
            }
            else
            {
                InitialUIinter initialUIinter = new InitialUIinter(this.uiApp);
                selBaseWallIds = initialUIinter.SelBsmtWallIds(); // UI获取所有地库填充区域id
                elemsViewLevels = initialUIinter.ElemsViewLevels(); //一个视图对应一个层级
            }

            double L_ = 0;// 地下室外墙周长之和
            double S_ = 0;// 地下室建筑面积
            double S0_ = 0;// 塔楼投影面积之和
            double S2_ = 0;// 主楼非停车区域面积
            double S3_ = 0;// 大型设备用房面积
            double S4_ = 0;// 非机动车库面积-不含夹层
            double S5_ = 0;// 非机动车库面积-夹层
            double S6_ = 0;// 工具间
            double S7_ = 0;// 公变所
            double S8_ = 0;// 储藏间
            double S9_ = 0;// 人防分区储藏间
            double S10_ = 0;// 核心筒
            double S11_ = 0;// 采光井
            double S12_ = 0;// 下沉庭院
            double S13_ = 0;// 单元门厅

            double N_ = 0;// 平层停车数量
            double MeN_ = 0;// 机械车位数量
            double N_Allocated = GlobalData.Instance.numberCarsAllocated_num;
            foreach (ElemsViewLevel _elemsViewLevel in elemsViewLevels)
            {
                List<DataCollection> dataCollections = new List<DataCollection>();

                string title = "";
                List<ElementId> selElementIds = new List<ElementId>();

                /*
                 * 多个方案，则把方案放置于多个方案中间的位置
                 * 
                 */

                List<XYZ> xYZs = new List<XYZ>();

                List<RevitElePS> allBoundElePses = new List<RevitElePS>(); // 该收集器，用于统计不同类型的车位族实例
                foreach (Bsmt bsmt in _elemsViewLevel.Bsmts)
                {
                    Polygon2d bsmntPolygon2d = bsmt.Polygon2dInward;
                    // 判断地库是否属于选择集
                    if (!selBaseWallIds.Contains(bsmt.BsmtBound.Id))
                        continue;

                    bsmt.Computer_Ps_Fr_Col_SubExit_Area();

                    bsmt.CalStatistics();
                    dataCollections.Add(bsmt.DataCollection);// 添加表格基础数据

                    xYZs.Add((bsmt.Polygon2dInward.LUpOfBox2d() - new Vector2d(100.0, -200.0)).ToXYZ());

                    selElementIds.Add(bsmt.BsmtBound.Id);
                    string door_Number = bsmt.BsmtBound.Ele.get_Parameter(BuiltInParameter.DOOR_NUMBER).AsString();
                    title += @"方案:" + door_Number + @", ElementId:" + bsmt.BsmtBound.Id.ToString() + "\n";

                    List<RevitElePS> inBoundElePses = bsmt.InBoundElePses;
                    allBoundElePses.AddRange(inBoundElePses);
                    #region 统计车位数量

                    foreach (var item in inBoundElePses)// 微型车位统计数据在自身属性上已经给过系数0.7
                    {
                        if (item.EleProperty == EleProperty.MechanicalPP)// 机械车位需要单独进行统计
                        {
                            Parameter parameter = item.Ele.GetParaByName_("机械车位数");
                            int count = parameter.AsInteger();

                            N_ += count;
                            MeN_ += count;
                        }
                        else
                        {
                            if (item.EleProperty == EleProperty.EndPP)// 如果是回车类型，则不进行统计
                            {
                                // N_ -= item.Count();
                            }
                            else
                            {
                                N_ += item.Count();
                            }
                        }
                    }

                    #endregion

                    #region 统计各项面积、周长……

                    List<BoundO> revitEleFRs = bsmt.InBoundEleFRes;

                    L_ += bsmt.BsmtBound.Length;
                    S_ += bsmt.BsmtBound.Area;

                    foreach (var item in revitEleFRs)
                    {
                        // 塔楼投影
                        if (item.EleProperty == EleProperty.ResidenStruRegion)
                        {
                            S0_ += item.Area;
                        }
                        // 主楼非停车区域
                        else if (item.EleProperty == EleProperty.MainBuildingNonParkingArea)
                        {
                            S2_ += item.Area;
                        }
                        // 设备用房
                        else if (item.EleProperty == EleProperty.EquRoom)
                        {
                            S3_ += item.Area;
                        }
                        // 非机动车库（不含夹层）
                        else if (item.EleProperty == EleProperty.NonVehicleGarage)
                        {
                            S4_ += item.Area;
                        }
                        // 非机动车库（夹层）
                        else if (item.EleProperty == EleProperty.NonVehicleGarage_Mezzanine)
                        {
                            S5_ += item.Area;
                        }
                        // 工具间
                        else if (item.EleProperty == EleProperty.ToolRoom)
                        {
                            S6_ += item.Area;
                        }
                        // 住宅公用变电所
                        else if (item.EleProperty == EleProperty.ResidentialUtilitySubstation)
                        {
                            S7_ += item.Area;
                        }
                        // 储藏间
                        else if (item.EleProperty == EleProperty.Storeroom)
                        {
                            S8_ += item.Area;
                        }
                        // 人防分区
                        else if (item.EleProperty == EleProperty.AirDefenseDivision)
                        {
                            S9_ += item.Area;
                        }
                        // 核心筒
                        else if (item.EleProperty == EleProperty.CoreTube)
                        {
                            S2_ += item.Area;
                            S10_ += item.Area;// 单独计算核心筒数据
                        }
                        // 采光井
                        else if (item.EleProperty == EleProperty.LightWell)
                        {
                            S11_ += item.Area;
                        }
                        // 下沉庭院
                        else if (item.EleProperty == EleProperty.SinkingCourtyard)
                        {
                            S12_ += item.Area;
                        }
                        // 单元门厅
                        else if (item.EleProperty == EleProperty.UnitFoyer)
                        {
                            S2_ += item.Area;
                            S13_ += item.Area;
                        }

                    }
                    #endregion

                    // 判断指标是否满足，不满足给予颜色提示
                    // 该功能暂时取消
                    if (MainWindow.Instance.judeData.IsChecked == true)
                    {
                        SetSubstandard(bsmt, GlobalData.Instance.tarParkingEfficiency_num);
                    }
                }

                //【】创建 DataTable

                DataTable dt = new DataTable();
                dt.Columns.Add("01", typeof(String));//添加列
                dt.Columns.Add("02", typeof(String));
                dt.Columns.Add("03", typeof(String));
                dt.Columns.Add("04", typeof(String));
                dt.Columns.Add("05", typeof(String));
                dt.Columns.Add("06", typeof(String));
                dt.Columns.Add("07", typeof(String));
                dt.Columns.Add("08", typeof(String));
                dt.Columns.Add("09", typeof(String));
                dt.Columns.Add("10", typeof(String));
                dt.Rows.Add(new object[] { @"附表-地库强排数据收集" });

                string strUpData = UpdateData(L_, S_, S0_, S2_, S3_, S4_, S5_, S6_, S7_, S8_, S9_, S10_, S11_, S12_, S13_, N_, MeN_, N_Allocated, dt);

                // 将数据写出到csv
                string filePath = this.doc.PathName;
                if (filePath.IsNullOrEmpty())
                {
                    //"由于该文件尚未保存，本次运算不会生成指标统计的Excel文件，请保存Revit文件后，再次点击-指标刷新-功能。".TaskDialogErrorMessage();
                }
                else
                {
                    int length = filePath.Length;
                    filePath = filePath.Substring(0, length - 4) + @"_地库强排数据.csv";
                    bool delete = FileUtils.DeleteFile(filePath);
                    if (delete)
                    {
                        // 表格数据添加生成时间
                        dt.Rows.Add(new object[] { @"计算时间", DateTime.Now, "", "", "", "" });

                        // 删除成功，才允许写出文件
                        dt.ToCSV(filePath);
                    }
                }

                string typesPPstatistic = CalTypesPPstatistic(allBoundElePses, N_);

                //【】创建注释文字
                string dataString = "";
                dataString += @"图面统计数据汇总" + "\n";
                dataString += title + "\n";
                dataString += strUpData + "\n";// 基本统计数据
                dataString += typesPPstatistic + "\n";// 不同型号车位，单项统计
                dataString += @"计算时间：" + DateTime.Now.ToString();

                XYZ xYZ = XYZ.Zero;
                xYZs.ForEach(p =>
                {
                    xYZ += p;
                });
                XYZ textLocation = xYZ / (xYZs.Count);

                UpDataToTextNote(this.doc, this.view, textLocation, dataString, selElementIds);

                CompareNumbPs(N_, N_Allocated);

                //【】表格处理
                if (GlobalData.Instance.wheGenerateTable)// 绘制表格的开关
                {
                    TableHandle tableHandle = new TableHandle(this.doc);
                    tableHandle.Execute(dataCollections);// 输入地库外墙线的Id，为了给表格group命名
                }
            }

            sw.Stop();
            sw.Restart();
        }
        /// <summary>
        /// 低于指定停车效率进行填充区域红色提示
        /// </summary>
        internal void SetSubstandard(Bsmt bsmt, double referenceIndex)
        {
            bsmt.SubPsAreaEexists.ForEach(p =>
            {
                p.Computer();

                using (Transaction transDelete = new Transaction(this.doc, "判断地库-子区域的停车效率"))
                {
                    transDelete.Start();
                    p.SetSubstandard(referenceIndex);
                    transDelete.Commit();
                }

            });
        }
        /// <summary>
        /// 判断图面停车位与应停车位数量的关系 红色是超过百分比，需要减少，蓝色则低于这个百分比
        /// </summary>
        internal void CompareNumbPs(double _n, double _N_Allocated)
        {
            MainWindow.Instance.NumberParkingOnLevel.Foreground = new SolidColorBrush(Colors.Black);
            double highNum = _N_Allocated * (1 + GlobalData.Instance.perOfDamagedPs_num / 100);
            if (_n < _N_Allocated)
            {
                MainWindow.Instance.NumberParkingOnLevel.Foreground = new SolidColorBrush(Colors.Blue);
            }
            else if (_n > highNum)// 红色提示高于超过百分比
            {
                MainWindow.Instance.NumberParkingOnLevel.Foreground = new SolidColorBrush(Colors.Red);
            }
        }
        /// <summary>
        /// 分类型统计车位数量
        /// </summary>
        /// <param name="revitElePs"></param>
        /// <returns></returns>
        internal string CalTypesPPstatistic(List<RevitElePS> revitElePs, double N_)
        {
            string str = "各类型停车单项统计数据如下：" + "\n";
            List<string> fiNames = revitElePs.Select(p => p.Ele.Name).Distinct().ToList();

            foreach (var item in fiNames)
            {
                int count = 0;
                if (item.Contains("机械车位_"))
                {
                    foreach (var elePS in revitElePs.Where(p => p.Ele.Name == item))
                    {
                        Parameter parameter = elePS.Ele.GetParaByName_("机械车位数");
                        count += parameter.AsInteger();
                    }
                }
                else
                {
                    count = revitElePs.Where(p => p.Ele.Name == item).Count();
                }

                str += item + " ==> 数量: " + count.ToString() + " ==> 占比: " + (count / N_).NumDecimal(4).ToPercent(2) + "\n";
            }
            return str;
        }

        /// <summary>
        /// 数据更新 参量说明：_s 地下室建筑面积,_s2 主楼非停车区建筑面积,_s3 大型设备用房面积,_s4 非机动车库面积
        /// 并输出文本
        /// </summary>
        internal string UpdateData(double _l, double _s, double _s0, double _s2, double _s3, double _s4, double _s5, double _s6, double _s7, double _s8, double _s9, double _s10, double _s11, double _s12, double _s13, double _n, double _men, double _N_Allocated, DataTable dt)
        {
            string strUpData = "";
            // 单位转换
            _l = _l.FeetToMilliMeter();
            _s = _s.SQUARE_FEETtoSQUARE_METERS();
            _s0 = _s0.SQUARE_FEETtoSQUARE_METERS();
            _s2 = _s2.SQUARE_FEETtoSQUARE_METERS();
            _s3 = _s3.SQUARE_FEETtoSQUARE_METERS();
            _s4 = _s4.SQUARE_FEETtoSQUARE_METERS();
            _s5 = _s5.SQUARE_FEETtoSQUARE_METERS();
            _s6 = _s6.SQUARE_FEETtoSQUARE_METERS();
            _s7 = _s7.SQUARE_FEETtoSQUARE_METERS();
            _s8 = _s8.SQUARE_FEETtoSQUARE_METERS();
            _s9 = _s9.SQUARE_FEETtoSQUARE_METERS();
            _s10 = _s10.SQUARE_FEETtoSQUARE_METERS();
            _s11 = _s11.SQUARE_FEETtoSQUARE_METERS();
            _s12 = _s12.SQUARE_FEETtoSQUARE_METERS();
            _s13 = _s13.SQUARE_FEETtoSQUARE_METERS();

            dt.Rows.Add(new object[] { @"项目年份", "", "", "", @"设计年份" });
            dt.Rows.Add(new object[] { @"项目所在地", "", "", "", @"县市级别" });
            dt.Rows.Add(new object[] { @"地上类型", "", "", "", @"住宅类型，如高层、洋房、合院等" });
            dt.Rows.Add(new object[] { @"用地面积", "", "", "", @"按实填写" });
            dt.Rows.Add(new object[] { @"建筑密度", "", "", "", @"按实填写" });
            dt.Rows.Add(new object[] { @"容积率", "", "", "", @"按实填写" });

            // 地下室外墙周长之和
            GlobalData.Instance.L = (_l / 1000.0).NumDecimal(1).ToString();// 
            strUpData += @"L-地下室外墙周长之和：" + GlobalData.Instance.L + @"m" + "\n";
            dt.Rows.Add(new object[] { @"地下室外墙周长之和", @"L", GlobalData.Instance.L, @"m", @"地下室外围结构墙体墙体的长度" });
            // 地下室建筑面积
            GlobalData.Instance.S = _s.NumDecimal(1).ToString();// 不含夹层面积
            strUpData += @"S-地下室建筑面积(不含夹层面积)：" + GlobalData.Instance.S + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"地下室建筑面积(不含夹层面积)", @"S", GlobalData.Instance.S, @"㎡", @"不含夹层面积" });
            // 地下室周长系数
            double k2 = (_l / 1000.0) / _s;
            GlobalData.Instance.K2 = k2.NumDecimal(2).ToString();// 
            strUpData += @"K2-地下室周长系数：" + GlobalData.Instance.K2 + @"m/㎡" + "\n";
            dt.Rows.Add(new object[] { @"地下室周长系数", @"K2", GlobalData.Instance.K2, @"㎡", @"地下室外墙周长之和与地下建筑面积的比值" });
            // 塔楼投影面积之和
            GlobalData.Instance.S0 = _s0.NumDecimal(1).ToString();// 
            strUpData += @"S0-塔楼投影面积之和：" + GlobalData.Instance.S0 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"塔楼投影面积之和", @"S0", GlobalData.Instance.S0, @"㎡", @"地下室外墙范围内，地面塔影面积的统计和" });
            // 机动车库面积
            double S1_ = _s - _s2 - _s3 - _s4 - _s6;
            GlobalData.Instance.S1 = S1_.NumDecimal(1).ToString(); // S1=S-S2-S3-S4不含门厅、核心筒、主楼内储藏、主楼内设备、大型设备房、非机动车库的纯车库面积，可以包含主楼外车库内的风机房等小型设备房
            strUpData += @"S1-机动车库面积-：" + GlobalData.Instance.S1 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"机动车库面积", @"S1", GlobalData.Instance.S1, @"㎡", @"S1=S-S2-S3-S4 不含门厅、核心筒、主楼内储藏、主楼内设备、大型设备房、非机动车库的纯车库面积，可以包含主楼外车库内的风机房等小型设备。" });
            // 主楼非停车区建筑面积
            GlobalData.Instance.S2 = _s2.NumDecimal(1).ToString();// 主楼轮廓内停车区以外的面积，主要包含门厅、核心筒、主楼内储藏等
            strUpData += @"S2-主楼非停车区建筑面积：" + GlobalData.Instance.S2 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"S2-主楼非停车区建筑面积", @"S2", GlobalData.Instance.S2, @"㎡", @"主楼轮廓内停车区以外的面积，主要包含门厅、核心筒、主楼内储藏等" });
            // 大型设备用房面积
            GlobalData.Instance.S3 = _s3.NumDecimal(1).ToString();// 变电房、水泵房、换热站等较大设备房
            strUpData += @"S3-大型设备用房面积：" + GlobalData.Instance.S3 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"大型设备用房面积", @"S3", GlobalData.Instance.S3, @"㎡", @"变电房、水泵房、换热站等较大设备用房" });
            // 非机动车库面积（不含夹层）
            GlobalData.Instance.S4 = _s4.NumDecimal(1).ToString();// 不含夹层非机动车库
            strUpData += @"S4-非机动车库面积（不含夹层）：" + GlobalData.Instance.S4 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"非机动车库面积（不含夹层）", @"S4", GlobalData.Instance.S4, @"㎡", @"不含夹层非机动车库" });
            // 非机动车库面积（夹层）
            GlobalData.Instance.S5 = _s5.NumDecimal(1).ToString();// 
            strUpData += @"S5-非机动车库面积（夹层）：" + GlobalData.Instance.S5 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"非机动车库面积（夹层）", @"S5", GlobalData.Instance.S5, @"㎡", @"含夹层非机动车库" });
            // 工具间
            GlobalData.Instance.S6 = _s6.NumDecimal(1).ToString();// 
            strUpData += @"S6-工具间：" + GlobalData.Instance.S6 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"工具间", @"S6", GlobalData.Instance.S6, @"㎡", @"工具间面积和" });
            //住宅公用变电所
            GlobalData.Instance.S7 = _s7.NumDecimal(1).ToString();// 
            strUpData += @"S7-住宅公共变电所：" + GlobalData.Instance.S7 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"住宅公共变电所", @"S7", GlobalData.Instance.S7, @"㎡", @"住宅公共变电所面积和" });
            //储藏间
            GlobalData.Instance.S8 = _s8.NumDecimal(1).ToString();// 
            strUpData += @"S8-储藏间：" + GlobalData.Instance.S8 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"储藏间", @"S8", GlobalData.Instance.S8, @"㎡", @"储藏间面积" });
            //人防分区
            GlobalData.Instance.S9 = _s9.NumDecimal(1).ToString();// 
            strUpData += @"S9-人防分区：" + GlobalData.Instance.S9 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"人防分区", @"S9", GlobalData.Instance.S9, @"㎡", @"人防分区面积" });
            //核心筒
            GlobalData.Instance.S10 = _s10.NumDecimal(1);// 
            strUpData += @"S10-核心筒：" + GlobalData.Instance.S10 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"核心筒", @"S10", GlobalData.Instance.S10, @"㎡", @"核心筒" });
            //采光井
            GlobalData.Instance.S11 = _s11.NumDecimal(1);// 
            strUpData += @"S11-采光井：" + GlobalData.Instance.S11 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"采光井", @"S11", GlobalData.Instance.S11, @"㎡", @"采光井" });
            //下沉庭院
            GlobalData.Instance.S12 = _s12.NumDecimal(1);// 
            strUpData += @"S12-下沉庭院：" + GlobalData.Instance.S12 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"下沉庭院", @"S12", GlobalData.Instance.S12, @"㎡", @"下沉庭院" });
            //单元门厅
            GlobalData.Instance.S13 = _s13.NumDecimal(1);// 
            strUpData += @"S13-单元门厅：" + GlobalData.Instance.S13 + @"㎡" + "\n";
            dt.Rows.Add(new object[] { @"单元门厅", @"S12", GlobalData.Instance.S13, @"㎡", @"单元门厅" });

            // 设备用房面积占比
            GlobalData.Instance.Pe = (_s3 / _s).NumDecimal(4).ToPercent(2);// S3/S*100%
            strUpData += @"Pe-设备用房面积占比：" + GlobalData.Instance.Pe + "\n";
            dt.Rows.Add(new object[] { @"设备用房面积占比", @"Pe", GlobalData.Instance.Pe, @"", @"S3/S * 100%" });
            // 平层停车数量
            GlobalData.Instance.N = _n.NumDecimal(1).ToString();// 立体车位需区分是否按单层考虑停车数
            strUpData += @"N-平层停车数量（包含机械车位）：" + GlobalData.Instance.N + @"辆" + "\n";
            dt.Rows.Add(new object[] { @"平层停车数量（包含机械车位）", @"N", GlobalData.Instance.N, @"辆", @"立体车位需区分是否按单层考虑停车数" });
            // 机械车位数量
            GlobalData.Instance.MeN = _men.ToString();// 立体车位需区分是否按单层考虑停车数
            strUpData += @"MeN-机械车位数量：" + GlobalData.Instance.MeN + @"辆" + "\n";
            dt.Rows.Add(new object[] { @"机械车位数量", @"MeN", GlobalData.Instance.MeN, @"辆" });
            // 毛车地比
            GlobalData.Instance.P = (_s / _n).NumDecimal(1).ToString();// S/N*100%按地下室总面积计算的车地比
            GlobalData.Instance.PAllocated = (_s / _N_Allocated).NumDecimal(1).ToString();// S/N*100%按地下室总面积计算的车地比
            strUpData += @"P-毛车地比：" + GlobalData.Instance.P + "\n";
            dt.Rows.Add(new object[] { @"毛车地比", @"P", GlobalData.Instance.P, "", @"S/N * 100% 按地下室总面积计算的车地比" });
            // 含设备及主楼车地比
            GlobalData.Instance.P1 = ((_s - _s4) / _n).NumDecimal(1).ToString();//（S-S4）/N*100%|仅扣除非机动车库的车地比
            GlobalData.Instance.P1Allocated = ((_s - _s4) / _N_Allocated).NumDecimal(1).ToString();//（S-S4）/N*100%|仅扣除非机动车库的车地比
            strUpData += @"P1-含设备及主楼车地比：" + GlobalData.Instance.P1 + "\n";
            dt.Rows.Add(new object[] { @"含设备及主楼车地比", @"P1", GlobalData.Instance.P1, "", @"(S-S4)/N * 100 仅扣除非机动车库的车地比" });
            // 含设备车地比
            GlobalData.Instance.P2 = ((_s - _s2 - _s4) / _n).NumDecimal(1).ToString();//（S-S2-S4）/N*100%|扣除主楼非停车区和非机动车库的车地比
            GlobalData.Instance.P2Allocated = ((_s - _s2 - _s4) / _N_Allocated).NumDecimal(1).ToString();//（S-S2-S4）/N*100%|扣除主楼非停车区和非机动车库的车地比
            strUpData += @"P2-含设备车地比：" + GlobalData.Instance.P2 + "\n";
            dt.Rows.Add(new object[] { @"含设备车地比", @"P2", GlobalData.Instance.P2, "", @"(S-S2-S4)/N * 100% 扣除主楼非停车区和非机动车库的车地" });
            // 仅含主楼车地比
            GlobalData.Instance.P3 = ((_s - _s3 - _s4) / _n).NumDecimal(1).ToString();//（S-S3-S4/N*100%扣除大型设备房和非机动车库的车地比
            GlobalData.Instance.P3Allocated = ((_s - _s3 - _s4) / _N_Allocated).NumDecimal(1).ToString();//（S-S3-S4/N*100%扣除大型设备房和非机动车库的车地比
            strUpData += @"P3-仅含主楼车地比：" + GlobalData.Instance.P3 + "\n";
            dt.Rows.Add(new object[] { @"仅含主楼车地比", @"P3", GlobalData.Instance.P3, "", @"(S-S3-S4)/N * 100% 扣除大型设备房和非机动车库的车地比" });
            // 净车地比
            GlobalData.Instance.P4 = (S1_ / _n).NumDecimal(1).ToString();// S1/N*100%|纯车库的车地比
            GlobalData.Instance.P4Allocated = (S1_ / _N_Allocated).NumDecimal(1).ToString();// S1/N*100%|纯车库的车地比
            strUpData += @"P4-净车地比：" + GlobalData.Instance.P4 + "\n";
            dt.Rows.Add(new object[] { @"净车地比", @"P4", GlobalData.Instance.P4, "", @"S1/N * 100% 纯车库的车地比" });

            dt.Rows.Add(new object[] { @"地下室层高及层数", "", "", "", @"室内外高差、覆土厚度及各层层高" });

            string str_Time = @"计算时间：" + DateTime.Now.ToString();
            string parkingplace_count = @"停车位数辆：" + GlobalData.Instance.N.ToString() + @" 辆；";
            string str_gross_Area = @"地下室建筑面积：" + GlobalData.Instance.S.ToString() + @" ㎡；";
            string str_gross_Length = @"地下室外墙周长之和：" + GlobalData.Instance.L.ToString() + @" m；";
            string str_net_ParkingEfficiency = @"毛车地比：" + GlobalData.Instance.P + @" ㎡/车；";
            string str_net_Area = @"机动车库面积：" + GlobalData.Instance.S1.ToString() + @" ㎡；";
            string str_gross_ParkingEfficiency = @"净车地比：" + GlobalData.Instance.P4 + @" ㎡/车；";

            return strUpData;
        }
        /// <summary>
        /// 数据以文字形式显示在界面上
        /// </summary>
        internal void UpDataToTextNote(Document doc, View nowView, XYZ xYZlocation, string dataString, List<ElementId> selElementIds)
        {
            List<TextNoteType> textNoteTypes = (new FilteredElementCollector(doc)).OfClass(typeof(TextNoteType)).WhereElementIsElementType().Cast<TextNoteType>().ToList();
            string textNoteTypeName = @"明细表默认 3.0mm";
            TextNoteType textNoteType = textNoteTypes.Where(p => p.Name == textNoteTypeName).FirstOrDefault();

            using (Transaction duplicateTextNoteType = new Transaction(doc, "duplicateTextNoteType"))
            {
                duplicateTextNoteType.Start();
                if (textNoteType == null)
                {
                    textNoteType = textNoteTypes.First().Duplicate(textNoteTypeName) as TextNoteType;
                }
                textNoteType.get_Parameter(BuiltInParameter.TEXT_SIZE).Set(3.0.MilliMeterToFeet());
                duplicateTextNoteType.Commit();
            }

            List<TextNote> textNotes = (new FilteredElementCollector(doc, nowView.Id)).OfCategory(BuiltInCategory.OST_TextNotes)?.WhereElementIsNotElementType()?.Cast<TextNote>().ToList();

            bool whetherCreatText = true;
            if (textNotes.Count > 0)
            {
                List<string> tarStrings = selElementIds.Select(p => "ElementId:" + p.ToString()).ToList();
                foreach (TextNote textNote in textNotes)
                {
                    if (textNote.Text.ContainAllListString(tarStrings))
                    {
                        using (Transaction modifyTrans = new Transaction(doc, "modifyText"))
                        {
                            modifyTrans.Start();
                            textNote.Text = dataString;
                            textNote.HorizontalAlignment = HorizontalTextAlignment.Right;
                            modifyTrans.Commit();
                        }
                        whetherCreatText = false;
                        break;
                    }
                }
            }

            if (whetherCreatText)
            {
                using (Transaction trans = new Transaction(doc, "creatText"))
                {
                    trans.Start();
                    TextNote textNote = TextNote.Create(doc, nowView.Id, xYZlocation, dataString, textNoteType.Id);
                    textNote.HorizontalAlignment = HorizontalTextAlignment.Right;
                    trans.Commit();
                }
            }
        }




    }//
}//
