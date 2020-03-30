using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Architecture;

namespace CADToRevit_RoomTag
{

    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;
            View view = uidoc.ActiveView;


            bool iscontinue = true;

            do
            {
                TextNote textnote = null;
                Room room = null;
                try
                {
                    textnote = SelectText(uidoc);
                    room = SelectRoom(uidoc);
                    if(room == null || textnote == null) { TaskDialog.Show("wrong", "貌似获取到的房间或文字有点问题,请重新获取.");continue; }
                }
                catch
                {
                    iscontinue = false;
                    continue;
                }

                //transaction

                using (Transaction transaction = new Transaction(doc))
                {
                    transaction.Start("更改房间标记");

                    string text = textnote.Text;

                    room.Name = text;

                    transaction.Commit();
                }

            }
            while (iscontinue);





            return Result.Succeeded;
        }


        public TextNote SelectText(UIDocument uidoc)
        {
            Reference refe = uidoc.Selection.PickObject(ObjectType.Element, new TextNoteFilter(), "点选文字.");
            TextNote text = uidoc.Document.GetElement(refe) as TextNote;
            return text;
        }
        public Room SelectRoom(UIDocument uidoc)
        {
            Reference refe = uidoc.Selection.PickObject(ObjectType.Element, new RoomFilter(), "点选房间.");
            Room room = uidoc.Document.GetElement(refe) as Room;
            return room;
        }

    }


    public class TextNoteFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if ((BuiltInCategory)elem.Category.Id.IntegerValue == BuiltInCategory.OST_TextNotes)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class RoomFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if ((BuiltInCategory)elem.Category.Id.IntegerValue == BuiltInCategory.OST_Rooms)
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

}
