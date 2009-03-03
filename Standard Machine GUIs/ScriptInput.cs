using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

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

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                string data;
                using (TextReader reader = new StreamReader(openFileDialog.FileName))
                {
                    data = reader.ReadToEnd();
                }

                txtScript.Text = data;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                using (TextWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.Write(txtScript.Text);
                }
            }
        }
    }
}