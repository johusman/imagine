using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Imagine.Library;

namespace Imagine.GUI
{
    public partial class MainForm : Form
    {
        ImagineFacade facade;

        public MainForm()
        {
            InitializeComponent();
            facade = new ImagineFacade();
        }

        private void openSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if(result == DialogResult.OK)
                facade.OpenSource(openFileDialog.FileName);
        }

        private void openDestinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();
            if(result == DialogResult.OK)
                facade.OpenDestination(saveFileDialog.FileName);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void doGenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            facade.Generate();
        }
    }
}