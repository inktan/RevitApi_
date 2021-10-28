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
    public partial class Form_Error : Form
    {
        public Form_Error(string _errorMessage)
        {
            InitializeComponent();
            this.textBox_errorMessage.Text = _errorMessage;
        }

        public Form_Error(string _mainInstruction, string _errorMessage)
        {
            InitializeComponent();
            this.label_message.Text = _mainInstruction;
            this.textBox_errorMessage.Text = _errorMessage;
        }
    }
}
