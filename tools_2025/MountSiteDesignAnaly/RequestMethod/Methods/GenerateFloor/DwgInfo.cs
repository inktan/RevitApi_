using Autodesk.Revit.DB;
using goa.Common;
using PubFuncWt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using g3;
using System.IO;
using System.Reflection;
using Autodesk.Revit.UI;
using TeighaNet;

namespace MountSiteDesignAnaly
{
    class DwgInfo
    {
        internal ImportInstance importInstance;
        internal XYZ PositonMoveDis { get; set; }

        public DwgInfo(ImportInstance _importInstance)
        {
            importInstance = _importInstance;

            this.PositonMoveDis = importInstance.GetTotalTransform().Origin;

        }

        TeighaServices teighaServices { get; set; }
        internal void Excute(UIApplication uiApp)
        {
            CADLinkType type = CMD.Doc.GetElement(importInstance.GetTypeId()) as CADLinkType;
            string pathfile = ModelPathUtils.ConvertModelPathToUserVisiblePath(type.GetExternalFileReference().GetAbsolutePath());

            this.teighaServices = new TeighaServices(uiApp);
            teighaServices.TempDwgPath = pathfile;
            teighaServices.ParseDwg();
        }

        internal List<string> LayerNames => this.teighaServices.DwgParser.LayerNames;

        internal string floorPolylingLayer { get; set; }
        public string heightTextLayer { get; internal set; }

        internal IEnumerable<PolyLineInfo> PolyLinesByLayerName()
        {

            List<TextInfo> textInfos = new List<TextInfo>();
            // 楼板多段线图层包含的文字
            if (this.teighaServices.DwgParser.TexLayertInfos.ContainsKey(floorPolylingLayer))
            {
                textInfos = this.teighaServices.DwgParser.TexLayertInfos[floorPolylingLayer];
            }
            // 单独的文字图层
            if (this.teighaServices.DwgParser.TexLayertInfos.ContainsKey(heightTextLayer))
            {
                textInfos.AddRange(this.teighaServices.DwgParser.TexLayertInfos[heightTextLayer]);
            }

            //foreach (var item in textInfos)
            //{
            //    XYZ xYZ = new XYZ(item.MaxPoint.X.MilliMeterToFeet(), item.MaxPoint.Y.MilliMeterToFeet(), 0);
            //    Line line = Line.CreateBound(xYZ, xYZ + new XYZ(5, 5, 0));
            //    List<GeometryObject> geometryObjects = new List<GeometryObject>() { line };
            //    CMD.Doc.CreateDirectShapeWithNewTransaction(geometryObjects);
            //}

            if (this.teighaServices.DwgParser.PolyLineLayerInfos.ContainsKey(floorPolylingLayer))
            {
                foreach (var item in this.teighaServices.DwgParser.PolyLineLayerInfos[floorPolylingLayer])
                {
                    Polygon2d polygon2d = item.PolyLine.ToPolygon2d();
                    // 计算楼板高程点
                    double zValue = 0.0;
                    foreach (var dwgParserText in textInfos)
                    {
                        var position = dwgParserText.Center;

                        Vector2d textNowPosition = new Vector2d(position.X + this.PositonMoveDis.X, position.Y + this.PositonMoveDis.Y);

                        // 提取的文字信息为实际显示信息，需要进行单位转换
                        if (polygon2d.Contains(textNowPosition))
                        {
                            zValue = dwgParserText.Value;
                            break;
                        }
                    }
                    // 单位要进行换算 默认计算单位为 米
                    yield return new PolyLineInfo(item.PolyLine, zValue * 1000);
                }
            }
        }
    }
}
