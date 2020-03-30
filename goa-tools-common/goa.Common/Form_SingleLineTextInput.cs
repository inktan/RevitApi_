using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace goa.Common
{
    public partial class Form_SingleLineTextInput : Form
    {
        public string Input { get { return this.textBox1.Text; } }

        public Form_SingleLineTextInput(string _instruction, string _text)
        {
            InitializeComponent();
            this.label1.Text = _instruction;
            this.textBox1.Text = _text;
        }
    }
}
