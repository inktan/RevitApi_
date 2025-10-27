using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SHET_INFO
{
    public partial class Form_Dropdown : Form
    {
        public ComboBox ComboBox { get { return this.comboBox1; } }

        public Form_Dropdown(List<string> _names)
        {
            InitializeComponent();
            this.comboBox1.DataSource = _names;
        }
    }
}
