using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Imagine.Library;
using System.Diagnostics;
using System.IO;

namespace Imagine.GUI
{
    public partial class MainForm : Form
    {
        ImagineFacade facade;

        public MainForm()
        {
            InitializeComponent();

            CreateNewFacade();
            lblSourceFile.Text = "";
            lblDestinationFile.Text = "";
            
            showTooltipsToolStripMenuItem.Checked = graphArea1.ShowTooltips;
            showPreviewToolStripMenuItem.Checked = panelPreview.Visible;
        }

        private void CreateNewFacade()
        {
            facade = new ImagineFacade();
            facade.Disconnect(facade.SourceMachine, 0, facade.DestinationMachine, 0);

            facade.SourceChanged += new EventHandler(sourceChanged);
            facade.DestinationChanged += new EventHandler(destinationChanged);
            facade.GraphChanged += new EventHandler(graphChanged);

            graphArea1.Facade = facade;
        }

        private void openSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Images|*.jpg;*.png;*.gif;*.bmp|JPEG (*.jpg)|*.jpg|Ping (*.png)|*.png|GIF (*.gif)|*.gif|Bitmap (*.bmp)|*.bmp";
            DialogResult result = openFileDialog.ShowDialog();
            if(result == DialogResult.OK)
                facade.OpenSource(openFileDialog.FileName);
        }

        private void openDestinationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.Filter = "Ping (*.png)|*.png";
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
            ProgressWindow window = new ProgressWindow();
            window.Left = this.Left + (this.Width - window.Width) / 2;
            window.Top = this.Top + (this.Height - window.Height) / 2;
            window.Show(this);
            window.Refresh();

            Cursor lastCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            facade.SourceMachine.Preview = facade.DestinationMachine.Preview = false;
            facade.Generate(
                new ImagineFacade.ProgressCallback(
                    delegate(int machineIndex, int totalMachines, Machine currentMachine, int currentPercent)
                    {
                        double percent = currentPercent / 100.0;
                        double oneMachine = 1.0 / totalMachines;
                        window.SetPercent((int) ((machineIndex + percent) * oneMachine * 100.0));
                        window.SetText(currentMachine.ToString() + " [" + (machineIndex + 1) + "/" + totalMachines + "]");
                        window.Refresh();
                    }));
            facade.SourceMachine.Preview = facade.DestinationMachine.Preview = true;

            window.Close();

            Cursor.Current = lastCursor;
        }

        private void sourceChanged(object sender, EventArgs e)
        {
            lblSourceFile.Text = e.ToString();
            lblSourceFile.Links.Clear();
            lblSourceFile.Links.Add(new LinkLabel.Link(0, e.ToString().Length, e.ToString()));
            if (showPreviewToolStripMenuItem.Checked)
                DoPreview();
        }

        private void destinationChanged(object sender, EventArgs e)
        {
            lblDestinationFile.Text = e.ToString();
            lblDestinationFile.Links.Clear();
            lblDestinationFile.Links.Add(new LinkLabel.Link(0, e.ToString().Length, e.ToString()));
            if (showPreviewToolStripMenuItem.Checked)
                DoPreview();
        }

        private void graphChanged(object sender, EventArgs e)
        {
            if (showPreviewToolStripMenuItem.Checked)
                DoPreview();
        }

        private void DoPreview()
        {
            facade.SourceMachine.Preview = facade.DestinationMachine.Preview = true;
            long startTime = DateTime.Now.Ticks;
            facade.Generate();
            long stopTime = DateTime.Now.Ticks;
            facade.SourceMachine.Preview = facade.DestinationMachine.Preview = false;

            ImagineImage sourcePreview = facade.SourceMachine.LastPreviewImage;
            ImagineImage destinationPreview = facade.DestinationMachine.LastPreviewImage;
            pictureSourcePreview.Image = sourcePreview == null ? null : sourcePreview.GetBitmap();
            pictureDestinationPreview.Image = destinationPreview == null ? null : destinationPreview.GetBitmap();
            lblTiming.Text = String.Format("{0} ms", (stopTime - startTime) / 10000);
        }

        private void showTooltipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showTooltipsToolStripMenuItem.Checked = !showTooltipsToolStripMenuItem.Checked;
            graphArea1.ShowTooltips = showTooltipsToolStripMenuItem.Checked;
        }

        private void showPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPreviewToolStripMenuItem.Checked = !showPreviewToolStripMenuItem.Checked;
            panelPreview.Visible = showPreviewToolStripMenuItem.Checked;
            if(panelPreview.Visible)
                DoPreview();
        }

        private void linkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo process = new ProcessStartInfo((String) e.Link.LinkData);
            process.UseShellExecute = true;
            try
            {
                System.Diagnostics.Process.Start(process);
            }
            catch (Exception) { }
        }

        private void saveGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "imagine";
            saveFileDialog.Filter = "Imagine graphs (*.imagine)|*.imagine";
            DialogResult result = saveFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                using (TextWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.Write(facade.SerializeGraph());
                }
            }
        }

        private void openGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "imagine";
            openFileDialog.Filter = "Imagine graphs (*.imagine)|*.imagine";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                string data;
                using (TextReader reader = new StreamReader(openFileDialog.FileName))
                {
                    data = reader.ReadToEnd();
                }

                string source = facade.GetSourceFilename();
                string destination = facade.GetDestinationFilename();

                facade.DeserializeGraph(data);

                graphArea1.Facade = facade;
                if(source != null)
                    facade.OpenSource(source);
                if(destination != null)
                    facade.OpenDestination(destination);
                graphArea1.Refresh();

                if (showPreviewToolStripMenuItem.Checked)
                    DoPreview();
            }
        }

        private void newGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string source = facade.GetSourceFilename();
            string destination = facade.GetDestinationFilename();

            CreateNewFacade();

            if (source != null)
                facade.OpenSource(source);
            if (destination != null)
                facade.OpenDestination(destination);
            graphArea1.Refresh();

            if (showPreviewToolStripMenuItem.Checked)
                DoPreview();
        }
    }
}