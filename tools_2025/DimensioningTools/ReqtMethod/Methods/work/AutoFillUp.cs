using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PubFuncWt;
using Autodesk.Revit.UI.Selection;
using System.Reflection;
using System.IO;
using TeighaNet;
using System.Diagnostics;
using Autodesk.Revit.DB.Structure;

using goa.Common;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB.Electrical;
using System.Text.RegularExpressions;
using goa.Revit.DirectContext3D;
using System.Windows;
using g3;
using goa.Common.Exceptions;
//using NetTopologySuite.Geometries;

namespace DimensioningTools
{
    internal class AutoFillUp : RequestMethod
    {
        internal AutoFillUp(UIApplication _uiApp) : base(_uiApp)
        {
        }

        internal override void Execute()
        {
            if (!validInputs())
            {
                TaskDialog.Show("错误", "请首先选择标高族。");
                return;
            }

            double startElevation = ViewModel.Instance.TextBox_startElevation;
            double levelHeight = ViewModel.Instance.TextBox_levelHeight;
            int numLevel = ViewModel.Instance.TextBox_numLevel;

            var pMap = getParameterMap(CMD.fi);

            var doc = goa.Common.APP.UIApp.ActiveUIDocument.Document;
            double currentHeight = startElevation;

            using (TransactionGroup tg = new TransactionGroup(doc, "自动填写标高"))
            {
                tg.Start();
                using (Transaction trans = new Transaction(doc, "clean up"))
                {
                    trans.Start();
                    //clean up existing parameter values
                    foreach (Parameter p in CMD.fi.Parameters)
                    {
                        if (p.Definition.Name.Contains("标高")
                            && p.StorageType == StorageType.String)
                        {
                            p.Set("");
                        }
                    }
                    trans.Commit();
                }

                using (Transaction trans = new Transaction(doc, "fill up"))
                {
                    trans.Start();
                    //fill up
                    for (int i = 1; i < numLevel + 1; i++)
                    {
                        if (pMap.ContainsKey(i) == false)
                            continue;
                        string h = String.Format("{0:0.00}", currentHeight);
                        if (currentHeight.IsAlmostEqualByDifference(0))
                        {
                            h = "±" + h;
                        }
                        if (i == 1)
                        {
                            h = "H = " + h;
                        }

                        var p = pMap[i];
                        p.Set(h);
                        currentHeight += levelHeight;
                    }
                    trans.Commit();
                }
                tg.Assimilate();
            }
        }

        private Dictionary<int, Parameter> getParameterMap(Element _e)
        {
            var map = new Dictionary<int, Parameter>();
            foreach (Parameter p in _e.Parameters)
            {
                if (p.Definition.Name.Contains("标高")
                    && p.StorageType == StorageType.String)
                {
                    string s = p.Definition.Name.RemoveAll("标高 ");
                    int i;
                    bool b = int.TryParse(s, out i);
                    if (b)
                    {
                        map[i] = p;
                    }
                }
            }
            return map;
        }
        private bool validInputs()
        {
            //double d;
            //int i;
            //bool b1 = double.TryParse(ViewModel.Instance.TextBox_levelHeight.Text, out d);
            //bool b2 = int.TryParse(this.textBox_numLevel.Text, out i);
            //bool b3 = double.TryParse(this.textBox_startElevation.Text, out d);

            return CMD.fi != null;
        }
    }
}
