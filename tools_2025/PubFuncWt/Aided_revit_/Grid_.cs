using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubFuncWt
{
    public static class Grid_
    {
        //手动创建进深方向的轴号1、2、3、4、5、6、7、8、9、10、……
        public static IList<string> GetGridName1_10()
        {
            IList<string> lst_gridVertical1_10 = new List<string>();
            for (int i = 1; i <= 1000; i++)
            {
                lst_gridVertical1_10.Add(i.ToString());
            }
            return lst_gridVertical1_10;
        }
        //手动创建进深方向的轴号A、B、C、D、E、F、G、……A1、B1、C1、D1、E1、F1、G1、……
        public static IList<string> GetGridNameA1(bool isContainZ)
        {
            IList<string> lst_gridVerticalA1_Z1 = new List<string>();
            for (char startChar = 'A'; startChar <= 'Z'; startChar++)
            {
                if (isContainZ)
                {
                    if (startChar != 'I' && startChar != 'O')
                    {
                        lst_gridVerticalA1_Z1.Add(startChar.ToString());
                    }
                }
                else
                {
                    if (startChar != 'I' && startChar != 'O' && startChar != 'Z')
                    {
                        lst_gridVerticalA1_Z1.Add(startChar.ToString());
                    }
                }


            }
            int fistCountA1 = lst_gridVerticalA1_Z1.Count();
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "1");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "2");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "3");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "4");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "5");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "6");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "7");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "8");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "9");
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add(lst_gridVerticalA1_Z1[i].ToString() + "10");
            }
            return lst_gridVerticalA1_Z1;
        }
        public static IList<string> GetGridName1A(bool isContainZ)
        {
            IList<string> lst_gridVerticalA1_Z1 = new List<string>();
            for (char startChar = 'A'; startChar <= 'Z'; startChar++)
            {
                if (isContainZ)
                {
                    if (startChar != 'I' && startChar != 'O')
                    {
                        lst_gridVerticalA1_Z1.Add(startChar.ToString());
                    }
                }
                else
                {
                    if (startChar != 'I' && startChar != 'O' && startChar != 'Z')
                    {
                        lst_gridVerticalA1_Z1.Add(startChar.ToString());
                    }
                }
            }
            int fistCountA1 = lst_gridVerticalA1_Z1.Count();
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("1" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("2" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("3" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("4" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("5" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("6" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("7" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("8" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("9" + lst_gridVerticalA1_Z1[i].ToString());
            }
            for (int i = 0; i < fistCountA1; i++)
            {
                lst_gridVerticalA1_Z1.Add("10" + lst_gridVerticalA1_Z1[i].ToString());
            }
            return lst_gridVerticalA1_Z1;
        }

        //手动创建进深方向的轴号A、B、C、D、E、F、G、……AA、BA、CA、DA、EA、FA、GA、……
        public static IList<string> GetGridNameAABA(bool isContainZ)
        {
            IList<string> lst_gridVerticalAA_ZZ = new List<string>();
            for (char startChar = 'A'; startChar <= 'Z'; startChar++)
            {
                if (isContainZ)
                {
                    if (startChar != 'I' && startChar != 'O')
                    {
                        lst_gridVerticalAA_ZZ.Add(startChar.ToString());
                    }
                }
                else
                {
                    if (startChar != 'I' && startChar != 'O' && startChar != 'Z')
                    {
                        lst_gridVerticalAA_ZZ.Add(startChar.ToString());
                    }
                }
            }
            int fistCountAA = lst_gridVerticalAA_ZZ.Count();
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "A");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "B");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "C");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "D");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "E");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "F");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "G");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "H");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "J");
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add(lst_gridVerticalAA_ZZ[i].ToString() + "K");
            }
            return lst_gridVerticalAA_ZZ;
        }
        public static IList<string> GetGridNameAAAB(bool isContainZ)
        {
            IList<string> lst_gridVerticalAA_ZZ = new List<string>();
            for (char startChar = 'A'; startChar <= 'Z'; startChar++)
            {
                if (isContainZ)
                {
                    if (startChar != 'I' && startChar != 'O')
                    {
                        lst_gridVerticalAA_ZZ.Add(startChar.ToString());
                    }
                }
                else
                {
                    if (startChar != 'I' && startChar != 'O' && startChar != 'Z')
                    {
                        lst_gridVerticalAA_ZZ.Add(startChar.ToString());
                    }
                }
            }
            int fistCountAA = lst_gridVerticalAA_ZZ.Count();
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("A" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("B" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("C" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("D" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("E" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("F" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("G" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("H" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("J" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            for (int i = 0; i < fistCountAA; i++)
            {
                lst_gridVerticalAA_ZZ.Add("K" + lst_gridVerticalAA_ZZ[i].ToString());
            }
            return lst_gridVerticalAA_ZZ;
        }

    }
}
