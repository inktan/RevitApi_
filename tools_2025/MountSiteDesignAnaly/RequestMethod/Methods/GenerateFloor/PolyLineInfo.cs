using Autodesk.Revit.DB;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MountSiteDesignAnaly
{
    internal class PolyLineInfo
    {
        internal PolyLine polyLine { get; set; }
        internal double zValue = 0.0;

        public PolyLineInfo(PolyLine _polyLine, double _zValue)
        {
            polyLine = _polyLine;
            zValue = _zValue;
        }

        internal void CreatFloor(FloorType floorType, Level level)
        {
            using (Transaction transaction = new Transaction(CMD.Doc, "创建楼板"))
            {
                transaction.Start();
                transaction.DeleteErrOrWaringTaskDialog();
                CurveArray curveArray = ToCurveArray(this.polyLine);
                if (curveArray.Size > 2)
                {
                    try
                    {
                        Floor floor = CMD.Doc.Create.NewFloor(curveArray, floorType, level, false);
                        floor.get_Parameter(BuiltInParameter.FLOOR_HEIGHTABOVELEVEL_PARAM).Set(zValue);
                    }
                    catch (Exception)
                    {
                        //throw;
                    }
                }
                transaction.Commit();
            }
        }

        internal IEnumerable<CurveArray> ToCurveArrArray(IEnumerable<PolyLine> polyLines)
        {
            foreach (var item in polyLines)
            {
                CurveArray curveArray = ToCurveArray(item);
                if (curveArray.Size > 2)
                {
                    yield return curveArray;
                }
            }
        }

        internal CurveArray ToCurveArray(PolyLine polyLine)
        {
            CurveArray curveArray = new CurveArray();
            var points = polyLine.GetCoordinates().DelAlmostEqualPoint();
            if (points.Count() < 3)
            {
                return curveArray;
            }

            for (int i = 0; i < points.Count(); ++i)
                curveArray.Append(Line.CreateBound(points.ElementAt(i), points.ElementAt((i + 1) % points.Count())));

            return curveArray;
        }


    }
}
