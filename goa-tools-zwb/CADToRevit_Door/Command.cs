using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;

namespace CADToRevit_Door
{

    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]

    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            View activeview = uidoc.ActiveView;
            Level level = activeview.GenLevel;

            if (activeview.UpDirection.X != 0 || activeview.UpDirection.Z != 0) { TaskDialog.Show("wrong", "请在平面视图内使用"); }

            ExternalEventHandler handler = new ExternalEventHandler();
            ExternalEvent exevent = ExternalEvent.Create(handler);

            bool iscontinue = true;

            do
            {
                DoorInformation doorinfo = GetDoorInfo(uidoc);
                if(doorinfo == null) { iscontinue = false; }
                else
                {
                    Wall hostwall = GetHostWall(uidoc, doorinfo);
                    if (hostwall == null) { TaskDialog.Show("wrong", "在找墙的时候遇到错误."); return Result.Failed; }

                    //creat door
                    FamilyInstance door = null;
                    using (Transaction transaction = new Transaction(doc))
                    {
                        transaction.Start("create the door");

                        door = doc.Create.NewFamilyInstance(doorinfo.door_location, doorinfo.door_symbol, hostwall, level, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);

                        transaction.Commit();
                    }
                }
            } while (iscontinue);




            return Result.Succeeded;
        }

        public DoorInformation GetDoorInfo(UIDocument uidoc)
        {
            Document doc = uidoc.Document;

            //selection
            DetaiLineFilter detaillinefilter = new DetaiLineFilter();
            TextFilter textfilter = new TextFilter();
            Reference refdetailline1 = null;
            Reference refdetailline2 = null;
            Reference reftext = null;

            try
            {
                refdetailline1 = uidoc.Selection.PickObject(ObjectType.Element, detaillinefilter, "选择第一根边线.");
                refdetailline2 = uidoc.Selection.PickObject(ObjectType.Element, detaillinefilter, "选择第二根边线.");
                reftext = uidoc.Selection.PickObject(ObjectType.Element, textfilter, "选择门标记.");
            }
            catch
            {
                return null;
            }

            List<Line> lines = new List<Line>();
            lines.Add((doc.GetElement(refdetailline1) as DetailLine).GeometryCurve as Line);
            lines.Add((doc.GetElement(refdetailline2) as DetailLine).GeometryCurve as Line);
            TextNote text = doc.GetElement(reftext) as TextNote;
            
            //get family and symbol
            string str = text.Text;
            str = ProcessString(str);
            Family doorfamily = GetDoorFamily(doc,str);
            FamilySymbol doorsymbol = GetDoorSymbol(doc, doorfamily, str);
            XYZ doorlocation = GetLoaction(lines);
            DoorInformation doorinfo = new DoorInformation(doorsymbol,doorlocation);
            return doorinfo;

        }
        public Wall GetHostWall(UIDocument uidoc , DoorInformation doorinfo)
        {
            Document doc = uidoc.Document;
            View activeview = uidoc.ActiveView;
            XYZ point = doorinfo.door_location;
            FilteredElementCollector wallcollector = new FilteredElementCollector(doc, activeview.Id);
            wallcollector.OfCategory(BuiltInCategory.OST_Walls).OfClass(typeof(Wall)).WhereElementIsNotElementType();
            IEnumerable<Wall> selected = from ele in wallcollector
                                         let wall = ele as Wall
                                         let locationcurve = (ele as Wall).Location as LocationCurve
                                         let curve = locationcurve.Curve
                                         let line = curve as Line
                                         let newline = Line.CreateBound(new XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, point.Z), new XYZ(line.GetEndPoint(1).X, line.GetEndPoint(1).Y, point.Z))
                                         let intersectionresulttmp = newline.Project(point)
                                         let distance = intersectionresulttmp.Distance
                                         where distance <= wall.Width
                                         select wall;
            if (selected.Count() == 0) { TaskDialog.Show("wrong", "未找到墙."); return null; }
            if (selected.Count() > 1) { TaskDialog.Show("wrong", "找到多个墙."); return null; }
            Wall hostwall = selected.First();
            return hostwall;
        }


        public string ProcessString(string str)
        {
            str = str.Trim();
            switch (str[0])
            {
                case 'M':
                    str = str.Insert(1, "MM");
                    break;
                case 'J':
                case 'Z':
                case 'F':
                    str = str.Insert(2, "M");
                    break;
                case 'T':
                    break;
                default:
                    TaskDialog.Show("wrong", "有未知类型");
                    return null;
            }
            return str;
        }
        public int GetDoorWidth(string str)
        {
            int temp = Convert.ToInt32(str.Substring(3, 2)) * 100;
            if (str.Contains("a") || str.Contains("c")) { temp += 50;}
            return temp;       
        }
        public int GetDoorHeight(string str)
        {
            int temp = Convert.ToInt32(str.Substring(5, 2)) * 100;
            if (str.Contains("b") || str.Contains("c")) { temp += 50; }
            return temp;
        }
        public DoorLeaf GetDoorLeaf(int width)
        {
            if (width <= 1000) { return DoorLeaf.SNGL; }
            //else if(width>=1200 && width <= 1300) { return DoorLeaf.UNQL; }
            else { return DoorLeaf.DUBL; }
        }
        public FamilySymbol GetDoorSymbol(Document doc, Family doorfamily, string str)
        {
            int width = GetDoorWidth(str);
            int height = GetDoorHeight(str);

            FamilySymbol doortype = FindSymbol(doc, doorfamily, width, height , str);
            if (doortype == null)
            {
                doortype = CreatNewSymbol(doc, doorfamily, width, height, str);
            }

            return doortype;

        }
        public Family GetDoorFamily(Document doc , string str)
        {
            string familyname = null;
            DoorType type = DoorType.NONE;
            int width = 0;
            DoorLeaf leaf = DoorLeaf.NONE;
            switch (str[0])
            {
                case 'M':
                case 'F':
                    type = DoorType.M;
                    width = GetDoorWidth(str);
                    leaf = GetDoorLeaf(width);
                    if(leaf == DoorLeaf.SNGL) { familyname = "A-DOOR-SNGL"; }
                    if(leaf == DoorLeaf.DUBL) { familyname = "A-DOOR-DUBL"; }
                    break;
                case 'J':
                    type = DoorType.JM;
                    width = GetDoorWidth(str);
                    leaf = GetDoorLeaf(width);
                    if (leaf == DoorLeaf.SNGL) { familyname = "A-DOOR-ACES-SNGL"; }
                    if (leaf == DoorLeaf.DUBL) { familyname = "A-DOOR-ACES-DUBL"; }
                    break;
                case 'Z':
                    type = DoorType.ZM;
                    width = GetDoorWidth(str);
                    familyname = "A-DOOR-UNQL";
                    break;
                case 'T':
                    type = DoorType.TLM;
                    width = GetDoorWidth(str);
                    familyname = "A-DOOR-SLID";
                    break;



                default:
                    TaskDialog.Show("wrong", "有未知类型");
                    return null;
            }

            FilteredElementCollector doorfamilycollector = new FilteredElementCollector(doc);
            doorfamilycollector.OfClass(typeof(Family));
            IEnumerable<Family> doorfamilys = from element in doorfamilycollector
                                              let family = element as Family
                                              where family.Name.Equals(familyname)
                                              select family;    
            if(doorfamilys.Count() == 0) { TaskDialog.Show("wrong", "未找到族,可能是因为未载入族文件."); return null; }
            return doorfamilys.First() as Family;
        }
        public FamilySymbol FindSymbol(Document doc , Family doorfamily,int width,int height,string str)
        {
            FamilySymbol doortype = null;
            List<ElementId> doorfamilysymbolids = doorfamily.GetFamilySymbolIds().ToList();
            foreach (ElementId id in doorfamilysymbolids)
            {
                FamilySymbol symbol = doc.GetElement(id) as FamilySymbol;
                Parameter para_width = symbol.get_Parameter(BuiltInParameter.DOOR_WIDTH);
                Parameter para_height = symbol.get_Parameter(BuiltInParameter.GENERIC_HEIGHT);
                bool bool_width = para_width.AsValueString().Equals(width.ToString());
                bool bool_height = para_height.AsValueString().Equals(height.ToString());
                if (bool_height && bool_width)
                {
                    if((symbol.Name)[0] == str[0])
                    {
                        doortype = symbol;
                        break;
                    }
                }
            }
            return doortype;
        }
        public class LoadOptions : IFamilyLoadOptions
        {
            public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
            {
                overwriteParameterValues = true;
                return true;
            }

            public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
            {
                source = FamilySource.Project;
                overwriteParameterValues = true;
                return true;
            }
        }
        public FamilySymbol CreatNewSymbol(Document doc , Family doorfamily , int doorwidth, int doorheight , string str )
        {
            Document familydoc = doc.EditFamily(doorfamily);
            FamilyManager familymanager = familydoc.FamilyManager;
            string symbolname = str;
            using (Transaction creatnewsymbol = new Transaction(familydoc))
            {
                creatnewsymbol.Start("新建族类型");
                FamilyType familytype = familymanager.NewType(symbolname);
                FamilyParameter para_width = familymanager.get_Parameter(BuiltInParameter.DOOR_WIDTH);
                FamilyParameter para_height = familymanager.get_Parameter(BuiltInParameter.GENERIC_HEIGHT);
                familymanager.Set(para_width, doorwidth / 304.8);
                familymanager.Set(para_height, doorheight / 304.8);
                creatnewsymbol.Commit();
            }
            LoadOptions loadoptions = new LoadOptions();
            doorfamily = familydoc.LoadFamily(doc, loadoptions);
            FamilySymbol newsymbol = null;
            using (Transaction activesymbol = new Transaction(doc))
            {
                activesymbol.Start("激活族类型");
                ISet<ElementId> idstmp = doorfamily.GetFamilySymbolIds();
                foreach (ElementId id in idstmp)
                {
                    FamilySymbol symbol = doc.GetElement(id) as FamilySymbol;
                    if (symbol.Name.Equals(symbolname))
                    {
                        symbol.Activate();
                        newsymbol = symbol;
                    }
                }
                activesymbol.Commit();
            }

            return newsymbol;
        }
        public XYZ GetLoaction(List<Line> lines)
        {
            XYZ p1 = lines[0].GetEndPoint(0);
            XYZ p2 = lines[0].GetEndPoint(1);
            XYZ p3 = lines[1].GetEndPoint(0);
            XYZ p4 = lines[1].GetEndPoint(1);
            double x = (p1.X + p2.X + p3.X + p4.X)/4;
            double y = (p1.Y + p2.Y + p3.Y + p4.Y)/4;
            double z = (p1.Z + p2.Z + p3.Z + p4.Z)/4;
            return new XYZ(x, y, z);

        }







    }


    public enum DoorType
    {
        NONE = 0,
        M = 1,
        ZM = 2,
        JM =3,
        TLM =4
    }
    //字母门由字符串直接判断
    public enum DoorLeaf
    {
        NONE = 0,
        SNGL = 1,
        //UNQL = 2,
        DUBL = 2
    }

    public class DetaiLineFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            DetailLine line = elem as DetailLine;
            if (line != null)
            {
                Line geoline = line.GeometryCurve as Line;
                if (geoline.Length < (501 / 304.8) && geoline.Length > (49 / 304.8)) { return true; }
                else { return false; }
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class TextFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            TextNote text = elem as TextNote;
            if (text != null){return true; }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }


    public class DoorInformation
    {
        public DoorInformation(FamilySymbol symbol , XYZ location)
        {
            this.door_symbol = symbol;
            this.door_location = location;
        }

        //public Family door_family = null;
        public FamilySymbol door_symbol = null;
        public XYZ door_location = null;
        //public Wall door_host = null;

    }








    public class ExternalEventHandler : IExternalEventHandler
    {

        public void Execute(UIApplication app)
        {
            throw new NotImplementedException();
        }

        public string GetName()
        {
            throw new NotImplementedException();
        }
    }



}
