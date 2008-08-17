using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Imagine.StandardMachines
{
    public partial class ScriptInput : Form
    {
        public ScriptInput()
        {
            InitializeComponent();
        }

        public string Value
        {
            get { return txtScript.Text; }
            set { txtScript.Text = value; }
        }

        public string Errors
        {
            get { return txtErrors.Text; }
            set { txtErrors.Text = value; }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }


    }
}