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
using Imagine.GUI.Properties;
using System.Threading;

namespace Imagine.GUI
{
    public partial class MainForm : Form
    {
        private ImagineFacade facade;
        private string workingDirectory;
        private Cursor lastCursor;

        public MainForm()
        {
            InitializeComponent();

            workingDirectory = Environment.CurrentDirectory;

            CreateNewFacade();
            
            showTooltipsToolStripMenuItem.Checked = graphArea1.ShowTooltips;
            showPreviewToolStripMenuItem.Checked = panelPreview.Visible;

            webBrowser.DocumentText = Resources.help;

            RefreshSourcesAndDestinations(null);    
        }

        private void CreateNewFacade()
        {
            facade = new ImagineFacade(workingDirectory);
            facade.GraphChanged += graphChanged;
            graphArea1.Initialize(facade);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void doGenerateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Generate();
        }

        private void Generate()
        {
            ProgressWindow window = SetupForGeneration();

            Thread genThread = new Thread(new ParameterizedThreadStart(GenerateJob));
            genThread.Start(window);
        }

        private ProgressWindow SetupForGeneration()
        {
            ProgressWindow window = new ProgressWindow();
            window.Left = this.Left + (this.Width - window.Width) / 2;
            window.Top = this.Top + (this.Height - window.Height) / 2;
            window.Show(this);
            window.Refresh();

            this.Enabled = false;

            lastCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            TogglePreview(false);

            return window;
        }

        public void CleanupAfterGeneration(ProgressWindow window)
        {
            TogglePreview(true);

            window.Close();

            Cursor.Current = lastCursor;

            this.Enabled = true;
        }

        private void GenerateJob(object obj)
        {
            ProgressWindow window = (ProgressWindow)obj;

            GenerationCallbackObject callbackObject = new GenerationCallbackObject(window, this);

            facade.Generate(
                new ImagineFacade.TotalProgressCallback(
                    delegate(int machineIndex, int totalMachines, Machine currentMachine, int currentPercent)
                    {
                        double percent = currentPercent / 100.0;
                        double oneMachine = 1.0 / totalMachines;
                        
                        callbackObject.SetPercent((int)((machineIndex + percent) * oneMachine * 100.0));
                        callbackObject.SetText(currentMachine.ToString() + " [" + (machineIndex + 1) + "/" + totalMachines + "]");
                        callbackObject.Refresh();
                    }));

            callbackObject.Cleanup();
        }

        private void TogglePreview(bool preview)
        {
            foreach (SourceMachine machine in facade.Sources)
                machine.Preview = preview;
            foreach (SinkMachine machine in facade.Destinations)
                machine.Preview = preview;
        }

        private void graphChanged(object sender, EventArgs e)
        {
            RefreshSourcesAndDestinations(sender);
            
            if (showPreviewToolStripMenuItem.Checked)
                DoPreview();
        }

        private void RefreshSourcesAndDestinations(object sender)
        {
            object selectedItem = null;
            SourceMachine lastSourceMachine = cobSources.SelectedItem != null ? ((SourceMachineItem)cobSources.SelectedItem).Machine : null;
            SinkMachine lastDestinationMachine = cobDestinations.SelectedItem != null ? ((DestinationMachineItem)cobDestinations.SelectedItem).Machine : null;
            object lastSourceItem = null;
            object lastDestinationItem = null;

            cobSources.Items.Clear();

            foreach (SourceMachine machine in facade.Sources)
            {
                MachineItem item = new SourceMachineItem(machine);
                cobSources.Items.Add(item);
                if (machine == sender)
                    selectedItem = item;
                if (machine == lastSourceMachine)
                    lastSourceItem = item;
            }

            if (selectedItem != null)
                cobSources.SelectedItem = selectedItem;
            else
                cobSources.SelectedItem = lastSourceItem;

            if (cobSources.SelectedItem == null && cobSources.Items.Count > 0)
                cobSources.SelectedIndex = 0;

            selectedItem = null;


            cobDestinations.Items.Clear();

            foreach (SinkMachine machine in facade.Destinations)
            {
                MachineItem item = new DestinationMachineItem(machine);
                cobDestinations.Items.Add(item);
                if (machine == sender)
                    selectedItem = item;
                if (machine == lastDestinationMachine)
                    lastDestinationItem = item;
            }

            if (selectedItem != null)
                cobDestinations.SelectedItem = selectedItem;
            else
                cobDestinations.SelectedItem = lastDestinationItem;

            if (cobDestinations.SelectedItem == null && cobDestinations.Items.Count > 0)
                cobDestinations.SelectedIndex = 0;

        }

        private void DoPreview()
        {
            TogglePreview(true);
            long startTime = DateTime.Now.Ticks;
            facade.Generate();
            long stopTime = DateTime.Now.Ticks;
            TogglePreview(false);

            ShowPreviews();

            lblTiming.Text = String.Format("{0} ms", (stopTime - startTime) / 10000);
        }

        private void ShowPreviews()
        {
            ImagineImage sourcePreview = ((SourceMachineItem)cobSources.SelectedItem).Machine.LastPreviewImage;
            ImagineImage destinationPreview = ((DestinationMachineItem)cobDestinations.SelectedItem).Machine.LastPreviewImage;
            pictureSourcePreview.Image = sourcePreview == null ? null : sourcePreview.GetBitmap();
            pictureDestinationPreview.Image = destinationPreview == null ? null : destinationPreview.GetBitmap();
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

        private void sourceLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(cobSources.SelectedItem != null)
                ViewImage(((MachineItem) cobSources.SelectedItem).Filename);
        }

        private void destinationLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (cobDestinations.SelectedItem != null)
                ViewImage(((MachineItem)cobDestinations.SelectedItem).Filename);
        }

        private static void ViewImage(string file)
        {
            ProcessStartInfo process = new ProcessStartInfo(file);
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
                string graphData = facade.SerializeGraph();
                string layoutData = graphArea1.SerializeLayout();
                using (TextWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    writer.Write(graphData + "\n\n" + layoutData);
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

                string[] sources = new string[facade.Sources.Count];
                for(int i = 0; i < sources.Length; i++)
                    sources[i] = facade.Sources[i].Filename;
                string[] destinations = new string[facade.Destinations.Count];
                for (int i = 0; i < destinations.Length; i++)
                    destinations[i] = facade.Destinations[i].Filename;

                List<string> unrecognizedTypes = facade.DeserializeGraph(data);

                graphArea1.Initialize(facade);
                graphArea1.DeserializeLayout(data);
                for (int i = 0; i < sources.Length && i < facade.Sources.Count; i++)
                    facade.Sources[i].Filename = sources[i];
                for (int i = 0; i < destinations.Length && i < facade.Destinations.Count; i++)
                    facade.Destinations[i].Filename = destinations[i];

                if (unrecognizedTypes.Count > 0)
                {
                    string types = "";
                    foreach (string type in unrecognizedTypes)
                        types += "\n\t" + type;
                    MessageBox.Show(this, "The following machine types where not recognized\nand have been left out of the graph:\n"
                        + types +
                        "\n\nPerhaps you are missing a required library,\n" +
                        "or the name of the types may have changed\n" +
                        "in this version of Imagine or the library.",
                        "Problem during load", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                if (showPreviewToolStripMenuItem.Checked)
                    DoPreview();
            }
        }

        private void newGraphToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string source = facade.Sources[0].Filename;
            string destination = facade.Destinations[0].Filename;

            CreateNewFacade();

            if (source != null)
                facade.Sources[0].Filename = source;
            if (destination != null)
                facade.Destinations[0].Filename = destination;
            graphArea1.Refresh();

            if (showPreviewToolStripMenuItem.Checked)
                DoPreview();
        }

        private void showHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showHelpToolStripMenuItem.Checked = !showHelpToolStripMenuItem.Checked;
            webBrowser.Visible = showHelpToolStripMenuItem.Checked;
        }

        private void cobSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (showPreviewToolStripMenuItem.Checked)
                ShowPreviews();
        }

        private void cobDestinations_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (showPreviewToolStripMenuItem.Checked)
                ShowPreviews();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Generate();
        }
    }

    class MachineItem
    {
        protected string filename;
        protected string text;

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    class SourceMachineItem : MachineItem
    {
        private SourceMachine machine;

        public SourceMachine Machine
        {
            get { return machine; }
            set { machine = value; }
        }

        public SourceMachineItem(SourceMachine machine)
        {
            this.machine = machine;
            text = filename = machine.Filename;
            if (filename == null)
                text = "[source]";
        }
    }

    class DestinationMachineItem : MachineItem
    {
        private SinkMachine machine;

        public SinkMachine Machine
        {
            get { return machine; }
            set { machine = value; }
        }

        public DestinationMachineItem(SinkMachine machine)
        {
            this.machine = machine;
            text = filename = machine.Filename;
            if (filename == null)
                text = "[destination]";
        }
    }

    class GenerationCallbackObject
    {
        private delegate void SetPercentDel(int myInt);
        private delegate void SetTextDel(string myString);
        private delegate void RefreshDel();
        private delegate void CleanupDel(ProgressWindow window);

        private SetPercentDel setPercent;
        private SetTextDel setText;
        private RefreshDel refresh;
        private CleanupDel cleanup;

        private ProgressWindow window;
        private MainForm mainForm;

        public GenerationCallbackObject(ProgressWindow window, MainForm mainForm)
        {
            this.window = window;
            this.mainForm = mainForm;

            setPercent = new SetPercentDel(window.SetPercent);
            setText = new SetTextDel(window.SetText);
            refresh = new RefreshDel(window.Refresh);
            cleanup = new CleanupDel(mainForm.CleanupAfterGeneration);
        }

        public void SetPercent(int percent)
        {
            window.Invoke(setPercent, percent);
        }

        public void SetText(string text)
        {
            window.Invoke(setText, text);
        }

        public void Refresh()
        {
            window.Invoke(refresh);
        }

        public void Cleanup()
        {
            mainForm.Invoke(cleanup, window);
        }
    }
}