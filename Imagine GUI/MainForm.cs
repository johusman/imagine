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

            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            Machine splitter = facade.NewMachine("Imagine.RGBSplitter");
            facade.Connect(facade.SourceMachine, 0, splitter, 0);

            Machine inverter = facade.NewMachine("Imagine.Inverter");
            facade.Connect(splitter, 1, inverter, 0);
            facade.Connect(inverter, 0, facade.DestinationMachine, 0);

            SinkMachine newOutput = new SinkMachine();
            newOutput.Filename = "c:\\temp\\blue.png";
            facade.AddMachine(newOutput);
            facade.Connect(splitter, 2, newOutput, 0);

            facade.SourceChanged += new EventHandler(sourceChanged);
            facade.DestinationChanged += new EventHandler(destinationChanged);
            lblSourceFile.Text = "";
            lblDestinationFile.Text = "";
            graphArea1.Graph = facade.Graph;
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

        private void sourceChanged(object sender, EventArgs e)
        {
            lblSourceFile.Text = e.ToString();
        }

        private void destinationChanged(object sender, EventArgs e)
        {
            lblDestinationFile.Text = e.ToString();
        }
    }
}