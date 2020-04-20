using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using wt_Common;

namespace LayoutParkingEffcient
{
    class ParkingAlgorithm
    {
        //在给字段赋值时，如果要对赋给字段的值加以限制，可以先判断值是否满足条件，如果满足条件则赋值，否则给字段赋默认值或进行其他操作。
        //定义字段 4个角点计算的车位数量
        public int LeftDown_XYZ_Obstacle_count { get; set; }
        public int LeftUp_XYZ_Obstacle_count { get; set; }
        public int RigthUp_XYZ_Obstacle_count { get; set; }
        public int RightDown_XYZ_Obstacle_count { get; set; }

        public int LeftDown_XYZ_CarRoad_count { get; set; }
        public int LeftUp_XYZ_CarRoad_count { get; set; }
        public int RigthUp_XYZ_CarRoad_count { get; set; }
        public int RightDown_XYZ_CarRoad_count { get; set; }

        public List<XYZ> LeftDown_tar_placeXYZs_Obstacle { get; set; }
        public List<XYZ> LeftUp_tar_placeXYZs_Obstacle { get; set; }
        public List<XYZ> RigthUp_tar_placeXYZs_Obstacle { get; set; }
        public List<XYZ> RightDown_tar_placeXYZs_Obstacle { get; set; }

        public List<XYZ> LeftDown_tar_placeXYZs_CarRoad { get; set; }
        public List<XYZ> LeftUp_tar_placeXYZs_CarRoad { get; set; }
        public List<XYZ> RigthUp_tar_placeXYZs_CarRoad { get; set; }
        public List<XYZ> RightDown_tar_placeXYZs_CarRoad { get; set; }

        //定义字段 4种方案中的最大值
        public int Max_XYZ_count { get; set; }

        public List<XYZ> Max_tar_placeXYZs { get; set; }

        //构造函数
        public ParkingAlgorithm()
        {

        }

        //以下为 class 中的各种 method ———————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        /// <summary>
        /// 排布车位 计算目标区域内的 停车位 中心点
        /// </summary>
        /// <param name="uiapp"></param>
        public List<XYZ> LayoutParking(List<Line> _single_canPlacedRegion, List<List<Line>> _Obstacle_Line)//全部转移到clipper中进行处理
        {
            double distance_x = 0;
            double distance_y = 0;
            //从角点方向 开始排车位 4个方面
            //从起点相邻的是通道，还是障碍物 2个方面
            //基于逻辑判断，共8分支

            XYZ _LeftDownXYZ = _Methods.GetLeftDownXYZfromLines(_single_canPlacedRegion, out distance_x, out distance_y);//目标区域所在矩形的左下角坐标，以及X方向长度以及Y方向长度
            XYZ _LeftUpXYZ = _Methods.GetLeftUpXYZfromLines(_single_canPlacedRegion, out distance_x, out distance_y);//目标区域所在矩形的左上角坐标，以及X方向长度以及Y方向长度
            XYZ _RigthUpXYZ = _Methods.GetRigthUpXYZfromLines(_single_canPlacedRegion, out distance_x, out distance_y);//目标区域所在矩形的右上角坐标，以及X方向长度以及Y方向长度
            XYZ _RightDownXYZ = _Methods.GetRightDownXYZfromLines(_single_canPlacedRegion, out distance_x, out distance_y);//目标区域所在矩形的右下角坐标，以及X方向长度以及Y方向长度


            //通过判断求出落在目标区域内的点
            if (CMD.layoutMethod == "垂直式_0°")
            {
                LeftDown_tar_placeXYZs_Obstacle = GetTarPoints_vertical_0(_LeftDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "LeftDown", "Obstacle");
                LeftUp_tar_placeXYZs_Obstacle = GetTarPoints_vertical_0(_LeftUpXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "LeftUp", "Obstacle");
                RigthUp_tar_placeXYZs_Obstacle = GetTarPoints_vertical_0(_RigthUpXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "RightUp", "Obstacle");
                RightDown_tar_placeXYZs_Obstacle = GetTarPoints_vertical_0(_RightDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "RightDown", "Obstacle");

                LeftDown_tar_placeXYZs_CarRoad = GetTarPoints_vertical_0(_LeftDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "LeftDown", "CarRoad");
                LeftUp_tar_placeXYZs_CarRoad = GetTarPoints_vertical_0(_LeftUpXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "LeftUp", "CarRoad");
                RigthUp_tar_placeXYZs_CarRoad = GetTarPoints_vertical_0(_RigthUpXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "RightUp", "CarRoad");
                RightDown_tar_placeXYZs_CarRoad = GetTarPoints_vertical_0(_RightDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line, "RightDown", "CarRoad");

                LeftDown_XYZ_Obstacle_count = LeftDown_tar_placeXYZs_Obstacle.Count;
                LeftUp_XYZ_Obstacle_count = LeftUp_tar_placeXYZs_Obstacle.Count;
                RigthUp_XYZ_Obstacle_count = RigthUp_tar_placeXYZs_Obstacle.Count;
                RightDown_XYZ_Obstacle_count = RightDown_tar_placeXYZs_Obstacle.Count;

                LeftDown_XYZ_CarRoad_count = LeftDown_tar_placeXYZs_CarRoad.Count;
                LeftUp_XYZ_CarRoad_count = LeftUp_tar_placeXYZs_CarRoad.Count;
                RigthUp_XYZ_CarRoad_count = RigthUp_tar_placeXYZs_CarRoad.Count;
                RightDown_XYZ_CarRoad_count = RightDown_tar_placeXYZs_CarRoad.Count;

                //求出8种方案的最大值
                List<List<XYZ>> EightMethodsCalcutor_xyzlist = new List<List<XYZ>>()
                { LeftDown_tar_placeXYZs_Obstacle, LeftUp_tar_placeXYZs_Obstacle, RigthUp_tar_placeXYZs_Obstacle, RightDown_tar_placeXYZs_Obstacle,
                 LeftDown_tar_placeXYZs_CarRoad, LeftUp_tar_placeXYZs_CarRoad, RigthUp_tar_placeXYZs_CarRoad, RightDown_tar_placeXYZs_CarRoad };

                List<int> EightMethodsCalcutor_count = new List<int>()
                { LeftDown_XYZ_Obstacle_count, LeftUp_XYZ_Obstacle_count, RigthUp_XYZ_Obstacle_count, RightDown_XYZ_Obstacle_count,
                 LeftDown_XYZ_CarRoad_count, LeftUp_XYZ_CarRoad_count, RigthUp_XYZ_CarRoad_count, RightDown_XYZ_CarRoad_count };

                Max_XYZ_count = EightMethodsCalcutor_count.Max();
                int maxIndex = EightMethodsCalcutor_count.IndexOf(Max_XYZ_count);
                Max_tar_placeXYZs = EightMethodsCalcutor_xyzlist[maxIndex];
            }
            else if (CMD.layoutMethod == "垂直式_90°")
            {
                //tar_placeXYZs = GetTarPoints_vertical_90(_LeftDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line);//通过判断求出落在目标区域内的点
            }
            else if (CMD.layoutMethod == "平行式_0°")
            {
                //tar_placeXYZs = GetTarPoints_parallel_0(_LeftDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line);//通过判断求出落在目标区域内的点
            }
            else if (CMD.layoutMethod == "平行式_90°")
            {
                //tar_placeXYZs = GetTarPoints_parallel_90(_LeftDownXYZ, _single_canPlacedRegion, distance_x, distance_y, _Obstacle_Line);//通过判断求出落在目标区域内的点
            }
            else if (CMD.layoutMethod == "斜列式-倾角30°")
            {

            }
            else if (CMD.layoutMethod == "斜列式-倾角45°")
            {

            }
            else if (CMD.layoutMethod == "斜列式-倾角60°")
            {

            }
            else
            {
                TaskDialog.Show("error", "未选择停车方式");
            }
            return Max_tar_placeXYZs;
        }
        /// <summary>
        /// 获取落在目标区域内的车位中心点，需要设定车位排布规则 停车方式为平行式
        /// </summary>
        /// <param name="_origin_leftdown_XYZ"></param>
        /// <param name="_Lines"></param>
        /// <param name="distance_x"></param>
        /// <param name="distance_y"></param>
        /// <returns></returns>
        private List<XYZ> GetTarPoints_parallel_0(XYZ _origin_leftdown_XYZ, IList<Line> _Lines, double distance_x, double distance_y, List<List<Line>> SelObstacleAreas)
        {
            List<XYZ> tar_placeXYZs = new List<XYZ>();
            double _originX = _origin_leftdown_XYZ.X;
            double _originY = _origin_leftdown_XYZ.Y;
            XYZ _newXYZ = new XYZ(0, 0, 0);

            //计算目标区域所在矩形满排车位的最大值，并进行缓冲值处理
            double _parkingPlaceWight = CMD.parkingPlaceWight;
            double _parkingPlaceHeight = CMD.parkingPlaceHeight;
            double _Wd = CMD.Wd;
            double _columnWidth = CMD.columnWidth;

            int x_count_temp = Convert.ToInt32(distance_x / _parkingPlaceWight + 10);
            int y_count_temp = Convert.ToInt32(distance_y / _parkingPlaceHeight + 10);

            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;

                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }

                    bool _isNeeded = FilterRegionPoints(_newXYZ, _Lines, SelObstacleAreas);//对点进行过滤
                    if (_isNeeded)
                    {
                        tar_placeXYZs.Add(_newXYZ);
                    }
                }
            }
            return tar_placeXYZs;
        }
        private List<XYZ> GetTarPoints_parallel_90(XYZ _origin_leftdown_XYZ, IList<Line> _Lines, double distance_x, double distance_y, List<List<Line>> SelObstacleAreas)
        {
            List<XYZ> tar_placeXYZs = new List<XYZ>();
            double _originX = _origin_leftdown_XYZ.X;
            double _originY = _origin_leftdown_XYZ.Y;
            XYZ _newXYZ = new XYZ(0, 0, 0);

            //计算目标区域所在矩形满排车位的最大值，并进行缓冲值处理
            double _parkingPlaceWight = CMD.parkingPlaceWight;
            double _parkingPlaceHeight = CMD.parkingPlaceHeight;
            double _Wd = CMD.Wd;
            double _columnWidth = CMD.columnWidth;

            int x_count_temp = Convert.ToInt32(distance_x / _parkingPlaceWight + 10);
            int y_count_temp = Convert.ToInt32(distance_y / _parkingPlaceHeight + 10);

            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;

                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }

                    bool _isNeeded = FilterRegionPoints(_newXYZ, _Lines, SelObstacleAreas);//对点进行过滤
                    if (_isNeeded)
                    {
                        tar_placeXYZs.Add(_newXYZ);
                    }
                }
            }
            return tar_placeXYZs;
        }
        /// <summary>
        /// 获取落在目标区域内的车位中心点，需要设定车位排布规则 停车方式为垂直式
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <returns></returns>
        private List<XYZ> GetTarPoints_vertical_0(XYZ _origin_leftdown_XYZ, IList<Line> _Lines, double distance_x, double distance_y, List<List<Line>> SelObstacleAreas, string Fourcornerpoints, string neighborOption)
        {
            //计算目标区域所在矩形满排车位的最大值，并进行缓冲值处理
            double _parkingPlaceWight = CMD.parkingPlaceWight;//停车位宽度
            double _parkingPlaceHeight = CMD.parkingPlaceHeight;//停车位高度
            double _Wd = CMD.Wd;//通车道宽度
            double _columnWidth = CMD.columnWidth;//柱子宽度

            int x_count_temp = Convert.ToInt32(distance_x / _parkingPlaceWight);
            int y_count_temp = Convert.ToInt32(distance_y / _parkingPlaceHeight);

            //从起点相邻的是通道，还是障碍物 2个方面
            //基于逻辑判断，共8分支

            //起点坐标 需要考虑 停车位与边界线的关系 关系到 布置的第一个停车位 是紧挨障碍物，还是车道
            double _originX = _origin_leftdown_XYZ.X; //车位布置起点坐标个增加一般长宽
            double _originY = _origin_leftdown_XYZ.Y;

            // 1 得到  均布目标区域所在平面矩形的停车中心点
            // 1 从四个角点进行排布车位，算法不同 同时，还需要判断 起点相邻的是车道，还是障碍物
            List<XYZ> tar_placeXYZs = new List<XYZ>();
            if (Fourcornerpoints == "LeftDown")
            {
                if (neighborOption == "Obstacle")
                {
                    tar_placeXYZs = GetAllPoints_LeftDown_neighbor_Obstacle(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
                else if (neighborOption == "CarRoad")
                {
                    tar_placeXYZs = GetAllPoints_LeftDown_neighbor_CarRoad(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
            }
            else if (Fourcornerpoints == "LeftUp")
            {
                if (neighborOption == "Obstacle")
                {
                    tar_placeXYZs = GetAllPoints_LeftUp_neighbor_Obstacle(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
                else if (neighborOption == "CarRoad")
                {
                    tar_placeXYZs = GetAllPoints_LeftUp_neighbor_CarRoad(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
            }
            else if (Fourcornerpoints == "RightUp")
            {
                if (neighborOption == "Obstacle")
                {
                    tar_placeXYZs = GetAllPoints_RightUp_neighbor_Obstacle(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
                else if (neighborOption == "CarRoad")
                {
                    tar_placeXYZs = GetAllPoints_RightUp_neighbor_CarRoad(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
            }
            else if (Fourcornerpoints == "RightDown")
            {
                if (neighborOption == "Obstacle")
                {
                    tar_placeXYZs = GetAllPoints_RightDown_neighbor_Obstacle(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
                else if (neighborOption == "CarRoad")
                {
                    tar_placeXYZs = GetAllPoints_RightDown_neighbor_CarRoad(x_count_temp, y_count_temp, _originX, _originY, _parkingPlaceWight, _parkingPlaceHeight, _Wd, _columnWidth);
                }
            }

            // 2 过滤 落在停车区域外边界线 与 障碍物边界 之间的停车位中心点
            // 2 源于 上一步获取的点，均为停车位的中心点 因此，需要通过车身长和宽 剔除 与边界线重合的停车位 这里需要改进，需要计算停车位四个角点与边界线的关系 不再进行改进
            tar_placeXYZs = FilterPointsByRegion_vertical(tar_placeXYZs, _Lines, SelObstacleAreas);

            return tar_placeXYZs;
        }
        private List<XYZ> GetTarPoints_vertical_90(XYZ _origin_leftdown_XYZ, IList<Line> _Lines, double distance_x, double distance_y, List<List<Line>> SelObstacleAreas)
        {
            List<XYZ> tar_placeXYZs = new List<XYZ>();
            double _originX = _origin_leftdown_XYZ.X + CMD.parkingPlaceHeight / 2;
            double _originY = _origin_leftdown_XYZ.Y + CMD.parkingPlaceWight / 2;
            XYZ _newXYZ = new XYZ(0, 0, 0);

            //计算目标区域所在矩形满排车位的最大值，并进行缓冲值处理
            double _parkingPlaceWight = CMD.parkingPlaceWight;
            double _parkingPlaceHeight = CMD.parkingPlaceHeight;
            double _Wd = CMD.Wd;
            double _columnWidth = CMD.columnWidth;

            int x_count_temp = Convert.ToInt32(distance_x / _parkingPlaceHeight + 10);
            int y_count_temp = Convert.ToInt32(distance_y / _parkingPlaceWight + 10);

            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = j / 3;//计算需要添加通车道宽度的数量
                    double _tarY = _originY + j * _parkingPlaceWight + _columnWidth * _column_count;
                    if (i % 2 != 0)
                    {
                        int _Wd_count = i / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarX = _originX + i * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);

                    }
                    else if (i % 2 == 0)
                    {
                        int _Wd_count = i / 2;//计算需要添加通车道宽度的数量
                        double _tarX = _originX + i * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZ = new XYZ(_tarX, _tarY, 0);
                    }
                    bool _isNeeded = FilterRegionPoints(_newXYZ, _Lines, SelObstacleAreas);//对点进行过滤
                    if (_isNeeded)//首次过滤，得到车库边界线与障碍物 之间的空间
                    {
                        bool _2_isNeeded = FilterDistancePoints_vetical_90(_newXYZ, _Lines);
                        if (_2_isNeeded)//判断车位族中心点 与 车库边界线之间的关系
                        {
                            bool _3_isNeeded = FilterDistancePoints_vetical_List_90(_newXYZ, SelObstacleAreas);
                            if (_3_isNeeded)//二次判断 车位中心点 与 障碍物边界的问题
                            {
                                tar_placeXYZs.Add(_newXYZ);
                            }
                        }
                    }
                }
            }
            return tar_placeXYZs;
        }
        /// <summary>
        /// 剔除与边界线重合的 停车位 这里需要改进，需要计算停车位四个角点与边界线的关系
        /// </summary>
        /// <param name="temp_tar_placeXYZs"></param>
        /// <param name="_Lines"></param>
        /// <param name="SelObstacleAreas"></param>
        /// <returns></returns>
        public List<XYZ> FilterPointsByRegion_vertical(List<XYZ> temp_tar_placeXYZs, IList<Line> _Lines, List<List<Line>> SelObstacleAreas)
        {
            List<XYZ> tar_placeXYZs = new List<XYZ>();
            foreach (XYZ _xyz in temp_tar_placeXYZs)
            {
                bool _isNeeded = FilterRegionPoints(_xyz, _Lines, SelObstacleAreas);//对点进行首次过滤 得到落在车库边界线与障碍物之间的所有停车位中心点
                if (_isNeeded)
                {
                    bool _2_isNeeded = FilterDistancePoints_vetical(_xyz, _Lines);//判断 车位族中心点 与 车库边界线之间的关系
                    if (_2_isNeeded)
                    {
                        bool _3_isNeeded = FilterDistancePoints_vetical_List(_xyz, SelObstacleAreas);//二次判断 车位中心点 与 障碍物边界的问题
                        if (_3_isNeeded)
                        {
                            tar_placeXYZs.Add(_xyz);
                        }
                    }
                }
            }
            return tar_placeXYZs;
        }
        /// <summary>
        /// 左下角为起点 起点紧邻障碍物边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_LeftDown_neighbor_Obstacle(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX + CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY + CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;
                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 左下角为起点 起点紧邻道路边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_LeftDown_neighbor_CarRoad(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX + CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY + CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;
                    int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                    double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                    _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 左上角为起点 起点紧邻障碍物边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_LeftUp_neighbor_Obstacle(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX + CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY - CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;
                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY - j * _parkingPlaceHeight - _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY - j * _parkingPlaceHeight - _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 左上角为起点 起点紧邻道路边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_LeftUp_neighbor_CarRoad(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX + CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY - CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX + i * _parkingPlaceWight + _columnWidth * _column_count;
                    int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                    double _tarY = _originY - j * _parkingPlaceHeight - _Wd * _Wd_count;
                    _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 右上角为起点 起点紧邻障碍物边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_RightUp_neighbor_Obstacle(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX - CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY - CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX - i * _parkingPlaceWight - _columnWidth * _column_count;
                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY - j * _parkingPlaceHeight - _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY - j * _parkingPlaceHeight - _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 右上角为起点 起点紧邻道路边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_RightUp_neighbor_CarRoad(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX - CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY - CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX - i * _parkingPlaceWight - _columnWidth * _column_count;
                    int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                    double _tarY = _originY - j * _parkingPlaceHeight - _Wd * _Wd_count;
                    _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 右下角为起点 起点紧邻障碍物边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_RightDown_neighbor_Obstacle(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX - CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY + CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX - i * _parkingPlaceWight - _columnWidth * _column_count;
                    if (j % 2 != 0)
                    {
                        int _Wd_count = j / 2 + 1;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                    else if (j % 2 == 0)
                    {
                        int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                        double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                        _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                    }
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 右下角为起点 起点紧邻道路边界 均布目标区域所在平面矩形的停车中心点
        /// </summary>
        /// <param name="x_count_temp"></param>
        /// <param name="y_count_temp"></param>
        /// <param name="_originX"></param>
        /// <param name="_originY"></param>
        /// <param name="_parkingPlaceWight"></param>
        /// <param name="_parkingPlaceHeight"></param>
        /// <param name="_Wd"></param>
        /// <param name="_columnWidth"></param>
        /// <returns></returns>
        private List<XYZ> GetAllPoints_RightDown_neighbor_CarRoad(int x_count_temp, int y_count_temp, double _originX, double _originY, double _parkingPlaceWight, double _parkingPlaceHeight, double _Wd, double _columnWidth)
        {
            _originX = _originX - CMD.parkingPlaceWight / 2; //车位布置起点坐标个增加一般长宽
            _originY = _originY + CMD.parkingPlaceHeight / 2;
            List<XYZ> _newXYZs = new List<XYZ>();
            for (int i = 0; i < x_count_temp; i++)
            {
                for (int j = 0; j < y_count_temp; j++)
                {
                    int _column_count = i / 3;//计算需要添加通车道宽度的数量
                    double _tarX = _originX - i * _parkingPlaceWight - _columnWidth * _column_count;
                    int _Wd_count = j / 2;//计算需要添加通车道宽度的数量
                    double _tarY = _originY + j * _parkingPlaceHeight + _Wd * _Wd_count;
                    _newXYZs.Add(new XYZ(_tarX, _tarY, 0));
                }
            }
            return _newXYZs;
        }
        /// <summary>
        /// 对障碍的边界关系进行处理，障碍物边界为矩阵列表 即对单列表边界关系进行处理
        /// </summary>
        /// <param name="_newXYZ"></param>
        /// <param name="SelObstacleAreas"></param>
        /// <returns></returns>
        private bool FilterDistancePoints_vetical_List(XYZ _newXYZ, List<List<Line>> SelObstacleAreas)
        {
            bool isNeeded = true;
            foreach (List<Line> _Lines in SelObstacleAreas)
            {
                isNeeded = FilterDistancePoints_vetical(_newXYZ, _Lines);
                if (isNeeded == false)
                {
                    break;
                }
            }
            return isNeeded;
        }
        /// <summary>
        /// 对单列表边界关系进行处理 判断停车位边界线与边界线之间的关系 未采用停车位四角点与边界的关系进行判断
        /// </summary>
        /// <param name="_newXYZ"></param>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        private bool FilterDistancePoints_vetical(XYZ _newXYZ, IList<Line> _Lines)
        {
            bool isNeeded = true;
            double distance_x = CMD.parkingPlaceWight / 2 - 0.001;//做一个极限值，非精确值
            double distance_y = CMD.parkingPlaceHeight / 2 - 0.001;
            foreach (Line _Line in _Lines)//区分水平、竖直方向的线
            {
                XYZ _direction = _Line.Direction;
                double direction_x = Math.Abs(_direction.X);
                double direction_y = Math.Abs(_direction.Y);

                double _distance = _Line.Distance(_newXYZ);

                if (direction_x > 0.999999 || direction_y < 0.000001)//水平线
                {
                    if (_distance < distance_y)//注意x y轴 与 车位长宽的关系
                    {
                        isNeeded = false;
                        break;
                    }
                }
                else if (direction_y > 0.999999 || direction_x < 0.000001)//垂直线
                {
                    if (_distance < distance_x)//不能精确比较，需要做一个缓冲值空间
                    {
                        isNeeded = false;
                        break;
                    }
                }
                else
                {
                    if (_distance < distance_y)//不能精确比较，需要做一个缓冲值空间
                    {
                        isNeeded = false;
                        break;
                    }
                }
            }
            return isNeeded;
        }
        /// <summary>
        /// 对障碍的边界关系进行处理，障碍物边界为矩阵列表 即对单列表边界关系进行处理
        /// </summary>
        /// <param name="_newXYZ"></param>
        /// <param name="SelObstacleAreas"></param>
        /// <returns></returns>
        private bool FilterDistancePoints_vetical_List_90(XYZ _newXYZ, List<List<Line>> SelObstacleAreas)
        {
            bool isNeeded = true;
            foreach (List<Line> _Lines in SelObstacleAreas)
            {
                isNeeded = FilterDistancePoints_vetical_90(_newXYZ, _Lines);
                if (isNeeded == false)
                {
                    break;
                }
            }
            return isNeeded;
        }
        /// <summary>
        /// 对单列表边界关系进行处理
        /// </summary>
        /// <param name="_newXYZ"></param>
        /// <param name="_Lines"></param>
        /// <returns></returns>
        private bool FilterDistancePoints_vetical_90(XYZ _newXYZ, IList<Line> _Lines)
        {
            bool isNeeded = true;
            //double distance_x = CMD.parkingPlaceWight / 2 + Methods.MilliMeterToFeet(0);//做一个极限值，非精确值
            //double distance_y = CMD.parkingPlaceHeight / 2 + Methods.MilliMeterToFeet(0);
            double distance_x = CMD.parkingPlaceWight / 2 - 0.01;//做一个极限值，非精确值
            double distance_y = CMD.parkingPlaceHeight / 2 - 0.01;
            foreach (Line _Line in _Lines)//区分水平、竖直方向的线
            {
                XYZ _direction = _Line.Direction;
                double direction_x = Math.Abs(_direction.X);
                double direction_y = Math.Abs(_direction.Y);

                double _distance = _Line.Distance(_newXYZ);

                if (direction_x > 0.999999 || direction_y < 0.000001)//水平线
                {
                    if (_distance < distance_x)//注意x y轴 与 车位长宽的关系
                    {
                        isNeeded = false;
                        break;
                    }
                }
                else if (direction_y > 0.999999 || direction_x < 0.000001)//垂直线
                {
                    if (_distance < distance_y)//不能精确比较，需要做一个缓冲值空间
                    {
                        isNeeded = false;
                        break;
                    }
                }
                else
                {
                    if (_distance < distance_y)//不能精确比较，需要做一个缓冲值空间
                    {
                        isNeeded = false;
                        break;
                    }
                }
            }
            return isNeeded;
        }
        /// <summary>
        /// 对车位中心点进行过滤 得到计算区域外边界与障碍边界之间空间的点集
        /// </summary>
        /// <param name="_newXYZ"></param>
        /// <param name="_Lines"></param>
        /// <param name="SelObstacleAreas"></param>
        /// <returns></returns>
        private bool FilterRegionPoints(XYZ _newXYZ, IList<Line> _Lines, List<List<Line>> SelObstacleAreas)
        {
            bool _isNeeded = false;
            bool _isInLine = isInLine(_newXYZ, _Lines);//第一步，该函数判断点在不在边界线上，在为true，不在为false；
            bool _isInRgion = isInRegion(_newXYZ, _Lines);//第二步，判断点在总区域内部还是外部，在为true，不在为false；
            if (_isInRgion && !_isInLine)//双重判断，不在线上，在选择区域内
            {
                if (SelObstacleAreas.Count > 0)//需要对障碍区域进行过滤判断
                {
                    foreach (List<Line> _2_Lines in SelObstacleAreas)
                    {
                        bool _2_isInLine = isInLine(_newXYZ, _2_Lines);//第一步，该函数判断点在不在边界线上，在为true，不在为false；
                        bool _2_isInRgion = isInRegion(_newXYZ, _2_Lines);//第二步，判断点在区域内部还是外部，在为true，不在为false；
                        if (_2_isInLine || _2_isInRgion)//双重判断，不在线上，也不在选择区域内
                        {
                            _isNeeded = false;
                            break;
                        }
                        else
                        {
                            _isNeeded = true;
                        }
                    }
                }
                else
                {
                    _isNeeded = true;
                }
            }
            return _isNeeded;
        }
        /// <summary>
        /// 使用射线法，求一个点是不是在区域内
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <param name="_selRegionBoundings"></param>
        /// <returns></returns>
        private bool isInRegion(XYZ _XYZ, IList<Line> _selRegionBoundings)
        {
            bool _isInRegion = false;
            int intersectCount = 0;

            Line _LInebound = Line.CreateBound(_XYZ, new XYZ(_XYZ.X + 10000000, 0, 0));//求一个点的射线，不存在射线，给一个极大值
            foreach (Line _Line in _selRegionBoundings)
            {
                IntersectionResultArray results;
                SetComparisonResult result = _LInebound.Intersect(_Line, out results);
                if (result == SetComparisonResult.Overlap)//判断基准线是否与轴网相交
                {
                    if (results != null)
                    {
                        XYZ _LineendPoint_0 = _Line.GetEndPoint(0);
                        XYZ _LineendPoint_1 = _Line.GetEndPoint(1);
                        //根据上面的假设，射线连续经过的两个顶点显然都位于射线以上的一侧，因此这种情况看作没有发生穿越就可以了。
                        if ((_LineendPoint_0.Y < _XYZ.Y && _LineendPoint_1.Y >= _XYZ.Y) || (_LineendPoint_0.Y > _XYZ.Y && _LineendPoint_1.Y <= _XYZ.Y))//判断是不是顶点穿越
                        {
                            intersectCount += results.Size;
                        }
                    }
                }
            }
            //TaskDialog.Show("error", intersectCount.ToString());
            if (intersectCount % 2 != 0)//判断交点的数量是否为奇数或者偶数，奇数为内true，偶数为外false
            {
                _isInRegion = true;
            }
            return _isInRegion;
        }
        /// <summary>
        /// 判断一个点是不是一个列表集合线段上
        /// </summary>
        /// <param name="_XYZ"></param>
        /// <param name="_selRegionBoundings"></param>
        /// <returns></returns>
        private bool isInLine(XYZ _XYZ, IList<Line> _selRegionBoundings)
        {
            bool _isInLien = false;
            foreach (Line _Line in _selRegionBoundings)
            {
                if (_Line.Distance(_XYZ) < 0.003)
                {
                    _isInLien = true;
                    break;
                }
            }
            return _isInLien;
        }

    }
}
