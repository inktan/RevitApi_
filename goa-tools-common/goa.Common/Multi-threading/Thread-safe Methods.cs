using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace goa.Common
{
    static public class TSMethods
    {
        delegate void Del_SetLabelText(Label _label, string _text);

        static public void SetLabelText(Label _label, string _text)
        {
            if (_label.InvokeRequired)
            {
                var d = new Del_SetLabelText(SetLabelText);
                _label.Invoke(d, new object[] { _label, _text });
            }
            else
            {
                _label.Text = _text;
            }
        }

        delegate void Del_CloseForm(Form _form);

        static public void CloseForm(Form _form)
        {
            if (_form.InvokeRequired)
            {
                var d = new Del_CloseForm(CloseForm);
                _form.Invoke(d, new object[] { _form });
            }
            else
            {
                _form.Close();
            }
        }

        delegate void Del_BringToFront(Form _form);

        static public void BringToFront(Form _form)
        {
            if (_form.InvokeRequired)
            {
                var d = new Del_BringToFront(BringToFront);
                _form.Invoke(d, new object[] { _form });
            }
            else
            {
                _form.BringToFront();
            }
        }

        delegate void Del_HideForm(Form _form);

        static public void HideForm(Form _form)
        {
            if (_form.InvokeRequired)
            {
                var d = new Del_HideForm(HideForm);
                _form.Invoke(d, new object[] { _form });
            }
            else
            {
                _form.Hide();
            }
        }

        delegate void Del_ShowDialog(Form _form);

        static public void ShowDialog(Form _form)
        {
            if (_form.InvokeRequired)
            {
                var d = new Del_ShowDialog(ShowDialog);
                _form.Invoke(d, new object[] { _form });
            }
            else
            {
                _form.ShowDialog();
            }
        }
    }
}
