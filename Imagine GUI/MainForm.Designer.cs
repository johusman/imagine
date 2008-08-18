using Imagine.GUI.Properties;
namespace Imagine.GUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveGraphToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doGenerateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTooltipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showHelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webBrowser = new System.Windows.Forms.WebBrowser();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.lblTiming = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureDestinationPreview = new System.Windows.Forms.PictureBox();
            this.pictureSourcePreview = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.cobDestinations = new System.Windows.Forms.ComboBox();
            this.cobSources = new System.Windows.Forms.ComboBox();
            this.lblViewSourceFile = new System.Windows.Forms.LinkLabel();
            this.lblViewDestinationFile = new System.Windows.Forms.LinkLabel();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.graphArea1 = new Imagine.GUI.GraphArea();
            this.menuStrip1.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDestinationPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSourcePreview)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem,
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(714, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGraphToolStripMenuItem,
            this.openGraphToolStripMenuItem,
            this.saveGraphToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newGraphToolStripMenuItem
            // 
            this.newGraphToolStripMenuItem.Name = "newGraphToolStripMenuItem";
            this.newGraphToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.newGraphToolStripMenuItem.Text = "New Graph";
            this.newGraphToolStripMenuItem.Click += new System.EventHandler(this.newGraphToolStripMenuItem_Click);
            // 
            // openGraphToolStripMenuItem
            // 
            this.openGraphToolStripMenuItem.Name = "openGraphToolStripMenuItem";
            this.openGraphToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.openGraphToolStripMenuItem.Text = "Open Graph...";
            this.openGraphToolStripMenuItem.Click += new System.EventHandler(this.openGraphToolStripMenuItem_Click);
            // 
            // saveGraphToolStripMenuItem
            // 
            this.saveGraphToolStripMenuItem.Name = "saveGraphToolStripMenuItem";
            this.saveGraphToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.saveGraphToolStripMenuItem.Text = "Save Graph...";
            this.saveGraphToolStripMenuItem.Click += new System.EventHandler(this.saveGraphToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(141, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.doGenerateToolStripMenuItem});
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.generateToolStripMenuItem.Text = "Generate";
            // 
            // doGenerateToolStripMenuItem
            // 
            this.doGenerateToolStripMenuItem.Name = "doGenerateToolStripMenuItem";
            this.doGenerateToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.doGenerateToolStripMenuItem.Text = "Generate!";
            this.doGenerateToolStripMenuItem.Click += new System.EventHandler(this.doGenerateToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showTooltipsToolStripMenuItem,
            this.showPreviewToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // showTooltipsToolStripMenuItem
            // 
            this.showTooltipsToolStripMenuItem.Name = "showTooltipsToolStripMenuItem";
            this.showTooltipsToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.showTooltipsToolStripMenuItem.Text = "Show Tooltips";
            this.showTooltipsToolStripMenuItem.Click += new System.EventHandler(this.showTooltipsToolStripMenuItem_Click);
            // 
            // showPreviewToolStripMenuItem
            // 
            this.showPreviewToolStripMenuItem.Name = "showPreviewToolStripMenuItem";
            this.showPreviewToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.showPreviewToolStripMenuItem.Text = "Show Preview";
            this.showPreviewToolStripMenuItem.Click += new System.EventHandler(this.showPreviewToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showHelpToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // showHelpToolStripMenuItem
            // 
            this.showHelpToolStripMenuItem.Name = "showHelpToolStripMenuItem";
            this.showHelpToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.showHelpToolStripMenuItem.Text = "Show Help";
            this.showHelpToolStripMenuItem.Click += new System.EventHandler(this.showHelpToolStripMenuItem_Click);
            // 
            // webBrowser
            // 
            this.webBrowser.AllowNavigation = false;
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Right;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(423, 24);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.ScriptErrorsSuppressed = true;
            this.webBrowser.Size = new System.Drawing.Size(291, 404);
            this.webBrowser.TabIndex = 4;
            this.webBrowser.Visible = false;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // panelPreview
            // 
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPreview.Controls.Add(this.lblTiming);
            this.panelPreview.Controls.Add(this.label3);
            this.panelPreview.Controls.Add(this.label2);
            this.panelPreview.Controls.Add(this.label1);
            this.panelPreview.Controls.Add(this.pictureDestinationPreview);
            this.panelPreview.Controls.Add(this.pictureSourcePreview);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPreview.Location = new System.Drawing.Point(296, 24);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(127, 404);
            this.panelPreview.TabIndex = 3;
            this.panelPreview.Visible = false;
            // 
            // lblTiming
            // 
            this.lblTiming.AutoSize = true;
            this.lblTiming.Location = new System.Drawing.Point(20, 282);
            this.lblTiming.Name = "lblTiming";
            this.lblTiming.Size = new System.Drawing.Size(0, 13);
            this.lblTiming.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 269);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Timing:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Destination:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source:";
            // 
            // pictureDestinationPreview
            // 
            this.pictureDestinationPreview.Location = new System.Drawing.Point(14, 153);
            this.pictureDestinationPreview.Name = "pictureDestinationPreview";
            this.pictureDestinationPreview.Size = new System.Drawing.Size(100, 100);
            this.pictureDestinationPreview.TabIndex = 1;
            this.pictureDestinationPreview.TabStop = false;
            // 
            // pictureSourcePreview
            // 
            this.pictureSourcePreview.Location = new System.Drawing.Point(14, 25);
            this.pictureSourcePreview.Name = "pictureSourcePreview";
            this.pictureSourcePreview.Size = new System.Drawing.Size(100, 100);
            this.pictureSourcePreview.TabIndex = 0;
            this.pictureSourcePreview.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Imagine.GUI.Properties.Resources.gradient;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.btnGenerate);
            this.panel1.Controls.Add(this.cobDestinations);
            this.panel1.Controls.Add(this.cobSources);
            this.panel1.Controls.Add(this.lblViewSourceFile);
            this.panel1.Controls.Add(this.lblViewDestinationFile);
            this.panel1.Controls.Add(this.lblDestination);
            this.panel1.Controls.Add(this.lblSource);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 428);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(714, 57);
            this.panel1.TabIndex = 0;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Image = global::Imagine.GUI.Properties.Resources.generate;
            this.btnGenerate.Location = new System.Drawing.Point(3, 3);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(42, 51);
            this.btnGenerate.TabIndex = 6;
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // cobDestinations
            // 
            this.cobDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cobDestinations.DisplayMember = "Text";
            this.cobDestinations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobDestinations.FormattingEnabled = true;
            this.cobDestinations.Location = new System.Drawing.Point(138, 28);
            this.cobDestinations.Name = "cobDestinations";
            this.cobDestinations.Size = new System.Drawing.Size(509, 21);
            this.cobDestinations.TabIndex = 5;
            this.cobDestinations.ValueMember = "Text";
            this.cobDestinations.SelectedIndexChanged += new System.EventHandler(this.cobDestinations_SelectedIndexChanged);
            // 
            // cobSources
            // 
            this.cobSources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cobSources.DisplayMember = "Text";
            this.cobSources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cobSources.FormattingEnabled = true;
            this.cobSources.Location = new System.Drawing.Point(138, 6);
            this.cobSources.Name = "cobSources";
            this.cobSources.Size = new System.Drawing.Size(509, 21);
            this.cobSources.TabIndex = 4;
            this.cobSources.ValueMember = "Text";
            this.cobSources.SelectedIndexChanged += new System.EventHandler(this.cobSources_SelectedIndexChanged);
            // 
            // lblViewSourceFile
            // 
            this.lblViewSourceFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewSourceFile.AutoSize = true;
            this.lblViewSourceFile.BackColor = System.Drawing.Color.Transparent;
            this.lblViewSourceFile.Location = new System.Drawing.Point(653, 9);
            this.lblViewSourceFile.Name = "lblViewSourceFile";
            this.lblViewSourceFile.Size = new System.Drawing.Size(49, 13);
            this.lblViewSourceFile.TabIndex = 3;
            this.lblViewSourceFile.TabStop = true;
            this.lblViewSourceFile.Text = "View File";
            this.lblViewSourceFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.sourceLinkClicked);
            // 
            // lblViewDestinationFile
            // 
            this.lblViewDestinationFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblViewDestinationFile.AutoSize = true;
            this.lblViewDestinationFile.BackColor = System.Drawing.Color.Transparent;
            this.lblViewDestinationFile.Location = new System.Drawing.Point(653, 31);
            this.lblViewDestinationFile.Name = "lblViewDestinationFile";
            this.lblViewDestinationFile.Size = new System.Drawing.Size(49, 13);
            this.lblViewDestinationFile.TabIndex = 2;
            this.lblViewDestinationFile.TabStop = true;
            this.lblViewDestinationFile.Text = "View File";
            this.lblViewDestinationFile.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.destinationLinkClicked);
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.BackColor = System.Drawing.Color.Transparent;
            this.lblDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestination.Location = new System.Drawing.Point(51, 31);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(81, 13);
            this.lblDestination.TabIndex = 1;
            this.lblDestination.Text = "Destinations:";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.BackColor = System.Drawing.Color.Transparent;
            this.lblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSource.Location = new System.Drawing.Point(51, 9);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(57, 13);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Sources:";
            // 
            // graphArea1
            // 
            this.graphArea1.BackColor = System.Drawing.Color.OldLace;
            this.graphArea1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphArea1.Facade = null;
            this.graphArea1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.graphArea1.Graph = null;
            this.graphArea1.Location = new System.Drawing.Point(0, 24);
            this.graphArea1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.graphArea1.Name = "graphArea1";
            this.graphArea1.ShowTooltips = true;
            this.graphArea1.Size = new System.Drawing.Size(714, 404);
            this.graphArea1.TabIndex = 2;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 485);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.webBrowser);
            this.Controls.Add(this.graphArea1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Imagine";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            this.panelPreview.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDestinationPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSourcePreview)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doGenerateToolStripMenuItem;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.LinkLabel lblViewDestinationFile;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.LinkLabel lblViewSourceFile;
        private GraphArea graphArea1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTooltipsToolStripMenuItem;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.ToolStripMenuItem showPreviewToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureDestinationPreview;
        private System.Windows.Forms.PictureBox pictureSourcePreview;
        private System.Windows.Forms.Label lblTiming;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openGraphToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGraphToolStripMenuItem;
        private System.Windows.Forms.WebBrowser webBrowser;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showHelpToolStripMenuItem;
        private System.Windows.Forms.ComboBox cobDestinations;
        private System.Windows.Forms.ComboBox cobSources;
        private System.Windows.Forms.Button btnGenerate;
    }
}

