using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Imagine.StandardMachines
{
    public partial class NumberInput : Form
    {
        public NumberInput()
        {
            InitializeComponent();
        }

        public double Value
        {
            get { return (double)numericBox.Value; }
            set { numericBox.Value = (decimal)value; }
        }

        public string Caption
        {
            get { return this.Text; }
            set { this.Text = value; }
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