using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PubFuncWt
{
    public static class Color_
    {

        /// <summary>
        /// 彩虹色梯度渐变 00
        /// </summary>
        public static Color[] RainbowColors =
        {
            Color.Magenta,
            Color.Blue ,
            Color.Cyan ,
            Color.Lime ,
            Color.Yellow ,
            Color.Red
        };
        /// <summary>
        /// 浅绿到深绿 01
        /// </summary>
        public static Color[] GreenYellowToGreen =
        {
            Color.GreenYellow,
            Color.Green
        };
        /// <summary>
        /// 黑白梯度渐变 02
        /// </summary>
        public static Color[] WhiteToBlack =
        {
            Color.White,
            Color.Black
        };
        /// <summary>
        /// 桃核梯度渐变 03
        /// </summary>
        public static Color[] PeachPuffGradient =
        {
            Color.PapayaWhip,
            Color.PeachPuff,
            Color.Peru
        };
        /// <summary>
        /// 红绿渐变 04
        /// </summary>
        public static Color[] GreenRedGradient =
        {
            Color.Green,
            Color.Yellow ,
            Color.Red
        };

        /// <summary>
        /// 获得某一颜色区间的颜色集合
        /// </summary>
        /// <param name="sourceColor">起始颜色</param>
        /// <param name="destColor">终止颜色</param>
        /// <param name="count">分度数</param>
        /// <returns>返回颜色集合</returns>
        public static List<Color> GetSingleColorList(Color srcColor, Color desColor, int count)
        {
            List<Color> colorFactorList = new List<Color>();
            int redSpan = desColor.R - srcColor.R;
            int greenSpan = desColor.G - srcColor.G;
            int blueSpan = desColor.B - srcColor.B;
            for (int i = 0; i < count; i++)
            {
                Color color = Color.FromArgb(
                  srcColor.R + (int)((double)i / count * redSpan),
                  srcColor.G + (int)((double)i / count * greenSpan),
                  srcColor.B + (int)((double)i / count * blueSpan)
                );
                colorFactorList.Add(color);
            }
            return colorFactorList;
        }

        /// <summary>
        /// 获取颜色梯度渐变
        /// </summary>
        /// <param name="totalCount">梯度数</param>
        /// <param name="redToPurple">反向</param>
        /// <returns></returns>
        public static List<Color> GetColorListRedToMagenta(Color[] colors, int totalCount = 100, bool redToPurple = true)
        {
            List<Color> colorList = new List<Color>();

            int count = colors.Count();

            if (totalCount > 0)
            {
                if (redToPurple)
                {
                    for (int i = 0; i < count - 1; i++)
                    {
                        colorList.AddRange(GetSingleColorList(colors[i], colors[(i + 1) % count], totalCount / (count - 1) + (totalCount % (count - 1) > i ? 1 : 0)));
                    }
                }
                else
                {
                    for (int i = count - 1; i >= 1; i--)
                    {
                        colorList.AddRange(GetSingleColorList(colors[i], colors[(i - 1) % count], totalCount / (count - 1) + (totalCount % (count - 1) > i ? 1 : 0)));
                    }
                }
            }
            return colorList;
        }

        public static Autodesk.Revit.DB.Color ToRevitColor(this Color color)
        {
            return new Autodesk.Revit.DB.Color(color.R, color.G, color.B);
        }


    }
}
