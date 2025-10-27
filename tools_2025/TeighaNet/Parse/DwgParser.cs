using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teigha.DatabaseServices;
using Teigha.Geometry;

using PubFuncWt;

namespace TeighaNet
{
    /// <summary>
    /// dwg数据解析器
    /// </summary>
    public class DwgParser
    {
        public double CurrentScale = 1.0;
        public Database Database { get; set; }

        public DwgParser(Database database)
        {
            this.Database = database;
        }

        public void Layers()
        {
            this.LayerNames = new List<string>();
            using (var trans = this.Database.TransactionManager.StartTransaction())
            {
                using (LayerTable lt = (LayerTable)trans.GetObject(this.Database.LayerTableId, OpenMode.ForRead))
                {
                    foreach (ObjectId id in lt)
                    {
                        LayerTableRecord ltr = (LayerTableRecord)trans.GetObject(id, OpenMode.ForRead);
                        this.LayerNames.Add(ltr.Name);

                        //id.ToString().TaskDialogErrorMessage();
                    }
                }
                trans.Commit();
            }
        }

        public List<string> LayerNames { get; set; }

        public Dictionary<string, List<ArcInfo>> ArcLayerInfos { get; set; }
        public Dictionary<string, List<CircleInfo>> CircleLayerInfos { get; set; }
        public Dictionary<string, List<SplineInfo>> SplineLayerInfos { get; set; }
        public Dictionary<string, List<LineInfo>> LineLayerInfos { get; set; }
        public Dictionary<string, List<MlineInfo>> MlineLayerInfos { get; set; }
        public Dictionary<string, List<PolyLineInfo>> PolyLineLayerInfos { get; set; }
        public Dictionary<string, List<TextInfo>> TexLayertInfos { get; set; }
        public Dictionary<string, List<HatchInfo>> HatchLayerInfos { get; set; }
        /// <summary>
        /// 解析元素数量
        /// </summary>
        public int indent { get; set; }
        /// <summary>
        /// 解析
        /// </summary>
        public void Parse()
        {
            this.indent = 0;
            this.ArcLayerInfos = new Dictionary<string, List<ArcInfo>>();
            this.CircleLayerInfos = new Dictionary<string, List<CircleInfo>>();
            this.LineLayerInfos = new Dictionary<string, List<LineInfo>>();
            this.MlineLayerInfos = new Dictionary<string, List<MlineInfo>>();
            this.PolyLineLayerInfos = new Dictionary<string, List<PolyLineInfo>>();
            this.TexLayertInfos = new Dictionary<string, List<TextInfo>>();
            this.SplineLayerInfos = new Dictionary<string, List<SplineInfo>>();
            this.HatchLayerInfos = new Dictionary<string, List<HatchInfo>>();

            using (var trans = this.Database.TransactionManager.StartTransaction())
            {
                // 指向 BlockTable
                using (BlockTable blockTable = (BlockTable)this.Database.BlockTableId.GetObject(OpenMode.ForRead))
                {
                    // 循环 BlockTable
                    using (SymbolTableEnumerator enumerator = blockTable.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            using (BlockTableRecord blockTableRecord = (BlockTableRecord)enumerator.Current.GetObject(OpenMode.ForRead))
                            {
                                // 循环 BlockTableRecord
                                foreach (ObjectId objectId in blockTableRecord)
                                {
                                    if (objectId != null && !objectId.IsNull && !objectId.IsErased && objectId.IsValid)
                                    {
                                        this.indent++;
                                        ParseEntity(objectId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 解析单个元素
        /// </summary>
        /// <param name="objectId"></param>
        private void ParseEntity(ObjectId objectId)
        {
            using (Entity entity = (Entity)objectId.GetObject(OpenMode.ForRead, false, true)) //This method cannot work in RVT2014
            {
                // 仅考虑ModelSpace下的对象
                if (string.Compare(entity.BlockName, @"*Model_Space", true) == 0)
                {
                    switch (entity.GetRXClass().Name)
                    {
                        case "AcDb2LineAngularDimension":
                            break;
                        case "AcDb2dPolyline":
                            break;
                        case "AcDb3dPolyline":
                            break;
                        case "AcDb3PointAngularDimension":
                            break;
                        case "AcDbAlignedDimension":
                            break;
                        case "AcDbArc":
                            if (entity is Arc)
                                Parse((Arc)entity);
                            break;
                        case "AcDbArcDimension":
                            break;
                        case "AcDbBlockReference":
                            if (entity is BlockReference)
                                Parse((BlockReference)entity);
                            break;
                        case "AcDbBody":
                            break;
                        case "AcDbCircle":
                            if (entity is Circle)
                                Parse((Circle)entity);
                            break;
                        case "AcDbDgnReference":
                            break;
                        case "AcDbDiametricDimension":
                            break;
                        case "AcDbDwfReference":
                            break;
                        case "AcDbEllipse":
                            break;
                        case "AcDbFace":
                            break;
                        case "AcDbFcf":
                            break;
                        case "AcDbHatch":
                            if (entity is Hatch)
                                Parse((Hatch)entity);
                            break;
                        case "AcDbLeader":
                            break;
                        case "AcDbLine":
                            if (entity is Line)
                                Parse((Line)entity);
                            break;
                        case "AcDbMInsertBlock":
                            break;
                        case "AcDbMline":
                            if (entity is Mline)
                                Parse((Mline)entity);
                            break;
                        case "AcDbMText":
                            if (entity is MText)// what is it ？
                                Parse((MText)entity);
                            break;
                        case "AcDbOle2Frame":
                            break;
                        case "AcDbOrdinateDimension":
                            break;
                        case "AcDbText":
                            if (entity is DBText)
                                Parse((DBText)entity);
                            break;
                        case "AcDbPdfReference":
                            break;
                        case "AcDbPolyFaceMesh":
                            break;
                        case "AcDbPolygonMesh":
                            break;
                        case "AcDbPoint":
                            break;
                        case "AcDbPolyline":
                            if (entity is Polyline)
                                Parse((Polyline)entity);
                            break;
                        case "AcDbProxyEntity":
                            break;
                        case "AcDbRadialDimension":
                            break;
                        case "AcDbRasterImage":
                            break;
                        case "AcDbRay":
                            break;
                        case "AcDbRegion":
                            break;
                        case "AcDbRotatedDimension":
                            break;
                        case "AcDbShape":
                            break;
                        case "AcDb3dSolid":
                            break;
                        case "AcDbSpline":
                            if (entity is Spline)
                                Parse((Spline)entity);
                            break;
                        case "AcDbTable":
                            break;
                        case "AcDbTrace":
                            break;
                        case "AcDbViewport":
                            break;
                        case "AcDbWipeout":
                            break;
                        case "AcDbXline":
                            break;
                        default:
                            break;
                    }
                }
            }

            //throw new NotImplementedException();
        }
        private void Parse(BlockReference blockReference)
        {
            ObjectId objectId = blockReference.BlockTableRecord;
            if (objectId == null && objectId.IsNull) return;

            //using (BlockTableRecord blockTableRecord = (BlockTableRecord)objectId.GetObject(OpenMode.ForRead))
            //{
            //    int i = 0;

            //    AttributeCollection attributeCollection = blockReference.AttributeCollection;
            //    foreach (object obj in attributeCollection)
            //    {
            //        if (obj is AttributeReference)
            //        {
            //            if (obj is DBText)
            //            {
            //                Parse((DBText)obj);
            //            }
            //        }
            //        else if (obj is ObjectId objId)
            //        {
            //            using (AttributeReference attributeReference = (AttributeReference)objId.GetObject(OpenMode.ForRead))
            //            {
            //                if (attributeReference is DBText)
            //                {
            //                    Parse((DBText)attributeReference);
            //                }
            //            }
            //        }
            //        else
            //        {
            //        }
            //    }
            //}

            using (DBObjectCollection dBObjectCollection = new DBObjectCollection())
            {
                try
                {
                    blockReference.Explode(dBObjectCollection);
                }
                catch
                {
                    return;
                }

                foreach (Entity entity in dBObjectCollection)
                {
                    if (entity is BlockReference)
                    {
                        Parse((BlockReference)entity);
                    }
                    else if (entity is AttributeDefinition)
                    {
                        // ？？？已被dumpAttributeData
                    }
                    else if (entity is DBText)
                    {
                        Parse((DBText)entity);
                    }
                    else if (entity is MText)
                    {
                        Parse((MText)entity);
                    }
                    else if (entity is Mline)
                    {
                        Parse((Mline)entity);
                    }
                    else if (entity is Polyline)
                    {
                        Parse((Polyline)entity);
                    }
                    else if (entity is Line)
                    {
                        Parse((Line)entity);
                    }
                    else if (entity is Circle)
                    {
                        Parse((Circle)entity);
                    }
                    else if (entity is Hatch)
                    {
                        Parse((Hatch)entity);
                    }
                    else if (entity is Spline)
                    {
                        Parse((Spline)entity);
                    }
                    entity.Dispose();
                }
            }
        }

        void Parse(Arc arcEntity)
        {
            ArcInfo arcInfo = new ArcInfo(arcEntity);
            arcInfo.Parse();

            if (!this.ArcLayerInfos.ContainsKey(arcEntity.Layer))
            {
                List<ArcInfo> arcInfos = new List<ArcInfo>();
                this.ArcLayerInfos.Add(arcEntity.Layer, arcInfos);
            }
            this.ArcLayerInfos[arcEntity.Layer].Add(arcInfo);
        }


        private void Parse(Circle entity)
        {
            CircleInfo circleInfo = new CircleInfo(entity);
            circleInfo.Parse();
            if (!this.CircleLayerInfos.ContainsKey(entity.Layer))
            {
                List<CircleInfo> circleInfos = new List<CircleInfo>();
                this.CircleLayerInfos.Add(entity.Layer, circleInfos);
            }
            this.CircleLayerInfos[entity.Layer].Add(circleInfo);
            //throw new NotImplementedException();
        }
        private void Parse(Spline entity)
        {
            SplineInfo splineInfo = new SplineInfo(entity);
            splineInfo.Parse();
            if (!this.SplineLayerInfos.ContainsKey(entity.Layer))
            {
                List<SplineInfo> splineInfos = new List<SplineInfo>();
                this.SplineLayerInfos.Add(entity.Layer, splineInfos);
            }
            this.SplineLayerInfos[entity.Layer].Add(splineInfo);
            //throw new NotImplementedException();
        }

        void Parse(DBText dbEntityText)
        {
            TextInfo textInfo = new TextInfo(dbEntityText);
            textInfo.Parse();

            if (!this.TexLayertInfos.ContainsKey(dbEntityText.Layer))
            {
                List<TextInfo> textInfos = new List<TextInfo>();
                this.TexLayertInfos.Add(dbEntityText.Layer, textInfos);
            }
            // 追加
            this.TexLayertInfos[dbEntityText.Layer].Add(textInfo);
        }
        void Parse(Line lineEntity)
        {
            LineInfo lineInfo = new LineInfo(lineEntity);
            lineInfo.Parse();

            if (!this.LineLayerInfos.ContainsKey(lineEntity.Layer))
            {
                List<LineInfo> lineInfos = new List<LineInfo>();
                this.LineLayerInfos.Add(lineEntity.Layer, lineInfos);
            }

            this.LineLayerInfos[lineEntity.Layer].Add(lineInfo);
        }
        void Parse(MText mtextEntity)
        {
            using (DBObjectCollection dBObjectCollection = new DBObjectCollection())
            {
                try
                {
                    mtextEntity.Explode(dBObjectCollection);
                }
                catch
                {
                    return;
                }

                foreach (Entity entity in dBObjectCollection)
                {
                    if (entity is DBText)
                    {
                        Parse((DBText)entity);
                    }
                    entity.Dispose();
                }
            }
            //throw new NotImplementedException();
        }
        void Parse(Mline mlineEntity)
        {
            MlineInfo mlineInfo = new MlineInfo(mlineEntity);
            try
            {
                mlineInfo.Parse();
            }
            catch (Exception)
            {
                return;
                //throw;
            }

            if (!this.MlineLayerInfos.ContainsKey(mlineInfo.Layer))
            {
                List<MlineInfo> mlineInfos = new List<MlineInfo>();
                this.MlineLayerInfos.Add(mlineInfo.Layer, mlineInfos);
            }

            this.MlineLayerInfos[mlineInfo.Layer].Add(mlineInfo);
        }
        void Parse(Polyline polyLineEntity)
        {
            PolyLineInfo polyLineInfo = new PolyLineInfo(polyLineEntity);
            polyLineInfo.Parse();

            if (!this.PolyLineLayerInfos.ContainsKey(polyLineEntity.Layer))
            {
                List<PolyLineInfo> polyLineInfos = new List<PolyLineInfo>();
                this.PolyLineLayerInfos.Add(polyLineEntity.Layer, polyLineInfos);
            }

            this.PolyLineLayerInfos[polyLineEntity.Layer].Add(polyLineInfo);
        }
        void Parse(Hatch hatchEntity)
        {
            HatchInfo hatchInfo = new HatchInfo(hatchEntity);
            hatchInfo.Parse();

            if (!this.HatchLayerInfos.ContainsKey(hatchEntity.Layer))
            {
                List<HatchInfo> hatchInfos = new List<HatchInfo>();
                this.HatchLayerInfos.Add(hatchEntity.Layer, hatchInfos);
            }

            this.HatchLayerInfos[hatchEntity.Layer].Add(hatchInfo);
        }
    }

}
