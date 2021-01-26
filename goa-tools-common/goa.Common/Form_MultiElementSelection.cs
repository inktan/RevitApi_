using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.Revit.DB;

namespace goa.Common
{
    public partial class Form_MultiElementSelection : System.Windows.Forms.Form
    {
        public Form_MultiElementSelection(IEnumerable<ElementInfoForUI> _elems, IEnumerable<ElementId> _checked, string _caption = null)
        {
            InitializeComponent();

            this.listBox1.DisplayMember = "DisplayName";

            if (_caption != null)
                this.Text = _caption;

            foreach (var elem in _elems)
            {
                this.listBox1.Items.Add(elem);
                int index = this.listBox1.Items.IndexOf(elem);
                listBox1.SetSelected(index, _checked.Contains(elem.Id));
            }
        }
        public IEnumerable<Element> SelectedElements
        {
            get
            {
                var items = this.listBox1.SelectedItems;
                return items.Cast<ElementInfoForUI>().Select(x => x.Elem);
            }
        }

        private void setCheckStateToAll(bool _state)
        {
            var size = this.listBox1.Items.Count;
            for (int i = 0; i < size; i++)
            {
                this.listBox1.SetSelected(i, _state);
            }
        }

        private void button_selAll_Click(object sender, EventArgs e)
        {
            setCheckStateToAll(true);
        }

        private void button_selNone_Click(object sender, EventArgs e)
        {
            setCheckStateToAll(false);
        }

        private void button_reverseSel_Click(object sender, EventArgs e)
        {
            var size = this.listBox1.Items.Count;
            for (int i = 0; i < size; i++)
            {
                var state = this.listBox1.GetSelected(i);
                this.listBox1.SetSelected(i, !state);
            }
        }
    }
}
