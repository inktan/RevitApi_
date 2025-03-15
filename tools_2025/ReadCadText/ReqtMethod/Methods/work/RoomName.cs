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
using Autodesk.Revit.DB.Architecture;
using System.Windows.Controls;
using g3;

namespace ReadCadText
{
    internal class RoomName : RequestMethod
    {
        internal RoomName(UIApplication _uiApp) : base(_uiApp)
        {
        }

        public string PathName { get; set; }

        internal override void Execute()
        {
            FilteredElementCollector collector = new FilteredElementCollector(this.doc, this.view.Id).OfCategory(BuiltInCategory.OST_TextNotes);
            List<TextNote> textNotes = collector.ToElements().Where(p => p is TextNote).Cast<TextNote>().ToList();

            collector = new FilteredElementCollector(this.doc, this.view.Id).OfCategory(BuiltInCategory.OST_Rooms);
            foreach (var item in collector.ToElements())
            {
                if (item is Room room)
                {
                    Polygon2d polygon2d = room.get_BoundingBox(this.view).ToPolygon2d();
                    foreach (var textNote in textNotes)
                    {
                        if (polygon2d.Contains(textNote.Coord.ToVector2d()))
                        {
                            using (Transaction trans = new Transaction(this.doc))
                            {
                                trans.Start("---");
                                room.get_Parameter(BuiltInParameter.ROOM_NAME).Set(textNote.Text);
                                trans.Commit();
                            }
                            break;
                        }

                    }
                }

            }

        }
    }
}
