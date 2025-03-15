using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeighaNet;
using PubFuncWt;

namespace InfoStrucFormwork

{
    class ConBeamType
    {
        public TextInfo TextInfo { get; internal set; }
        public string beamTypeName { get; internal set; }

        // 1 矩形梁 2 变截面梁 5 工字钢梁
        public int typeInt { get; set; }

        public int Width = 0;// 工字钢为U
        public int Height1 = 0;// 高低梁的低端
        public int Height = 0;// 工字钢为H

        public int B = 0;
        public int H = 0;
        public int U = 0;
        public int T = 0;
        public int D = 0;
        public int F = 0;

        /// <summary>
        /// 存在加腋的可能性
        /// </summary>
        internal List<ConBeamType> AxillaryBeamTypes { get; set; }

        public ConBeamType(TextInfo textInfo)
        {
            this.TextInfo = textInfo;
        }

        internal void Excute()
        {
            int count = this.TextInfo.Text.SubstringCount("*");
            char temp = '*';
            if (count == 0)
            {
                count = this.TextInfo.Text.SubstringCount("x");
                temp = 'x';

            }

            // 矩形梁
            if (count == 1)
            {
                typeInt = 1;

                Excute01(temp);
                Excute02(temp);
                Excute03(temp);
            }
            // 变截面梁
            else if (count == 2)
            {
                typeInt = 2;
            }
            // 钢梁
            else if (count == 5)
            {
                typeInt = 5;
                Excute05(temp);
            }
        }
        /// <summary>
        /// 普通梁类型提取
        /// </summary>
        internal void Excute01(char temp)
        {
            //if (!TextInfo.Text.Contains(@"/")) return;

            int idnexMark = this.TextInfo.Text.IndexOf(temp);// 具有绝对意义
            string widthTmp = "";
            int i = 0;
            // 基于*号，向左寻找数字
            while (true)
            {
                i++;
                char widthChar = this.TextInfo.Text.ElementAtOrDefault(idnexMark - i);

                if (char.IsNumber(widthChar))
                {
                    widthTmp += widthChar;
                }
                if (!char.IsNumber(widthChar))
                {
                    break;
                }
            }

            string width = "";
            foreach (var charTmp in widthTmp.Reverse())
            {
                width += charTmp;
            }

            string heght = "";
            i = 0;
            // 基于*号，向右寻找数字
            while (true)
            {
                i++;
                char heightChar = this.TextInfo.Text.ElementAtOrDefault(idnexMark + i);

                if (char.IsNumber(heightChar))
                {
                    heght += heightChar;
                }

                if (!char.IsNumber(heightChar))
                {
                    break;
                }
            }

            int.TryParse(width, out this.Width);
            int.TryParse(heght, out this.Height);

            this.beamTypeName = width + temp + heght;
        }

        /// <summary>
        /// 异形托梁类型提取
        /// </summary>
        internal void Excute02(char temp)
        {
        }
        /// <summary>
        /// 钢梁类型提取
        /// </summary>
        internal void Excute05(char temp)
        {
            char[] tmep = { temp };
            String[] dimensionInfos = this.TextInfo.Text.Split(tmep);
            for (int index = 0; index < 6; index++)
            {
                if (index == 0)
                {
                    string str = "";
                    int i = 0;
                    // 基于*号，向左寻找数字
                    while (true)
                    {
                        i++;
                        char tempChar = dimensionInfos[index].ElementAtOrDefault(dimensionInfos[index].Length - i);

                        if (char.IsNumber(tempChar))
                        {
                            str += tempChar;
                        }
                        else
                        {
                            break;
                        }
                    }
                    str = new string(str.ToCharArray().Reverse<char>().ToArray<char>());
                    int.TryParse(str, out this.B);
                }
                else
                {
                    string str = "";
                    int i = 0;
                    while (true)
                    {
                        i++;
                        char tempChar = dimensionInfos[index].ElementAtOrDefault(i - 1);

                        if (char.IsNumber(tempChar))
                        {
                            str += tempChar;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (index == 1)
                    {
                        int.TryParse(str, out this.H);
                    }
                    else if (index == 2)
                    {
                        int.TryParse(str, out this.U);
                    }
                    else if (index == 3)
                    {
                        int.TryParse(str, out this.T);
                    }
                    else if (index == 4)
                    {
                        int.TryParse(str, out this.D);
                    }
                    else if (index == 5)
                    {
                        int.TryParse(str, out this.F);
                    }
                }
            }
            this.beamTypeName = this.B + temp + this.H + temp + this.U + temp + this.T + temp + this.D + temp + this.F+"";
        }

        /// <summary>
        /// 加腋类型提取，需要二次确认分割符号
        /// </summary>
        internal void Excute03(char temp)
        {
            if (!TextInfo.Text.Contains("竖向加腋")) return;

            int idnexMark = this.TextInfo.Text.IndexOf('H');// 具有绝对意义
            string heigthDifference = "";
            int i = 1;
            // 基于*号，向右寻找数字
            while (true)
            {
                i++;
                char heightChar = this.TextInfo.Text.ElementAtOrDefault(idnexMark + i);

                if (char.IsNumber(heightChar))
                {
                    heigthDifference += heightChar;
                }

                if (!char.IsNumber(heightChar))
                {
                    break;
                }
            }
            int heigthDifference_dou = 0;
            int.TryParse(heigthDifference, out heigthDifference_dou);
            if (heigthDifference_dou == 0) return;

            this.AxillaryBeamTypes = new List<ConBeamType>();
            ConBeamType conBeamType = new ConBeamType(this.TextInfo);
            conBeamType.Width = this.Width;
            conBeamType.Height1 = this.Height + heigthDifference_dou;
            conBeamType.beamTypeName = conBeamType.Width + "x" + conBeamType.Height1 + "-" + conBeamType.Height;
            this.AxillaryBeamTypes.Add(conBeamType);

        }

    }
}
