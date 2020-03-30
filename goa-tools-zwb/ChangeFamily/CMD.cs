using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace FAMI_REPL
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class CMD : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //选择一个图元
            Reference refe = uiDoc.Selection.PickObject(ObjectType.Element, "选择要替换的类型中某一个图元.");
            Element elem = doc.GetElement(refe);
          
            //找到category
            Category category = elem.Category;
            //遍历category下所有的elementtype
            FilteredElementCollector collector_type = new FilteredElementCollector(doc);
            collector_type.OfCategoryId(category.Id).WhereElementIsElementType();

            //ui.showdialog
            List<Element> list = new List<Element>();
            UI ui = new UI();
            ui.listbox_ElementType.ItemsSource = collector_type.ToList();
            ui.ShowDialog();
            //获取选中的type
            ElementType type = ui.listbox_ElementType.SelectedItem as ElementType;

            FilteredElementCollector collector_elem = new FilteredElementCollector(doc);
            collector_elem.OfCategoryId(category.Id).WhereElementIsNotElementType();
            IEnumerable<Element> enu_elem = from temp in collector_elem
                                            where temp.GetTypeId() == elem.GetTypeId()
                                            select temp;
            List<Element> list_elem = enu_elem.ToList();

            using (Transaction changetypekeepparameter = new Transaction(doc))
            {
                changetypekeepparameter.Start("changetype");
                for (int i = list_elem.Count - 1; i >= 0; i--)
                {
                    ChangeTypeKeepParameter(list_elem[i], type);
                }
                changetypekeepparameter.Commit();
            }

            return Result.Succeeded;
        }



        public ElementId ChangeTypeKeepParameter(Element elem_old , ElementType type)
        {
            Document doc = elem_old.Document;
            ElementId id_newelement = null;
            //复制图元(偏移一段距离,跳过警告窗口)=>参数转移=>偏移回原位=>删除原有图元.
            ICollection<ElementId> ids_newelement = ElementTransformUtils.CopyElement(doc, elem_old.Id, new XYZ(1, 0, 0));
            if (ids_newelement.Count != 1) { return null; }
            id_newelement = ids_newelement.First();
            Element elem_new = doc.GetElement(id_newelement);
            elem_new.ChangeTypeId(type.Id);
            //parameter
            ParameterSet parameterSet = elem_old.Parameters;
            foreach (Parameter para_oldelem in parameterSet)
            {
                if (para_oldelem.IsReadOnly) { continue; }
                if (!para_oldelem.HasValue) { continue; }
                if (para_oldelem.StorageType == StorageType.None) { continue; }
                if (para_oldelem.StorageType == StorageType.ElementId) { continue; }

                Parameter para_newelem = elem_new.LookupParameter(para_oldelem.Definition.Name);
                if (para_newelem.IsReadOnly) { continue; }
                if (!para_newelem.HasValue) { continue; }
                if (para_newelem.StorageType == StorageType.None) { continue; }
                if (para_newelem.StorageType == StorageType.ElementId) { continue; }
                if (para_newelem.StorageType == StorageType.Double) { para_newelem.Set(para_oldelem.AsDouble()); }
                if (para_newelem.StorageType == StorageType.Integer) { para_newelem.Set(para_oldelem.AsInteger()); }
                if (para_newelem.StorageType == StorageType.String) { para_newelem.Set(para_oldelem.AsValueString()); }
            }
            doc.Delete(elem_old.Id);
            ElementTransformUtils.MoveElement(doc, id_newelement, new XYZ(-1, 0, 0));
            return id_newelement;

        }

        
    }
}
