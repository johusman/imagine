using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Imagine.GUI
{
    public partial class ProgressWindow : Form
    {
        public ProgressWindow()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            lblText.Text = text;
        }

        public void SetPercent(int percent)
        {
            progressBar.Value = percent;
        }
    }
}