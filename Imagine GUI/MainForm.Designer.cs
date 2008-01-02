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
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblSourceFile = new System.Windows.Forms.LinkLabel();
            this.lblDestinationFile = new System.Windows.Forms.LinkLabel();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDestinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doGenerateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showTooltipsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelPreview = new System.Windows.Forms.Panel();
            this.showPreviewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.graphArea1 = new Imagine.GUI.GraphArea();
            this.pictureSourcePreview = new System.Windows.Forms.PictureBox();
            this.pictureDestinationPreview = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panelPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureSourcePreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDestinationPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Imagine.GUI.Properties.Resources.gradient;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.Controls.Add(this.lblSourceFile);
            this.panel1.Controls.Add(this.lblDestinationFile);
            this.panel1.Controls.Add(this.lblDestination);
            this.panel1.Controls.Add(this.lblSource);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 450);
            this.panel1.Margin = new System.Windows.Forms.Padding(1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(714, 35);
            this.panel1.TabIndex = 0;
            // 
            // lblSourceFile
            // 
            this.lblSourceFile.AutoSize = true;
            this.lblSourceFile.BackColor = System.Drawing.Color.Transparent;
            this.lblSourceFile.Location = new System.Drawing.Point(107, 4);
            this.lblSourceFile.Name = "lblSourceFile";
            this.lblSourceFile.Size = new System.Drawing.Size(67, 13);
            this.lblSourceFile.TabIndex = 3;
            this.lblSourceFile.Text = "lblSourceFile";
            // 
            // lblDestinationFile
            // 
            this.lblDestinationFile.AutoSize = true;
            this.lblDestinationFile.BackColor = System.Drawing.Color.Transparent;
            this.lblDestinationFile.Location = new System.Drawing.Point(107, 17);
            this.lblDestinationFile.Name = "lblDestinationFile";
            this.lblDestinationFile.Size = new System.Drawing.Size(86, 13);
            this.lblDestinationFile.TabIndex = 2;
            this.lblDestinationFile.Text = "lblDestinationFile";
            // 
            // lblDestination
            // 
            this.lblDestination.AutoSize = true;
            this.lblDestination.BackColor = System.Drawing.Color.Transparent;
            this.lblDestination.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestination.Location = new System.Drawing.Point(4, 17);
            this.lblDestination.Name = "lblDestination";
            this.lblDestination.Size = new System.Drawing.Size(96, 13);
            this.lblDestination.TabIndex = 1;
            this.lblDestination.Text = "Destination file:";
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.BackColor = System.Drawing.Color.Transparent;
            this.lblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSource.Location = new System.Drawing.Point(4, 4);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(72, 13);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Source file:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(714, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openSourceToolStripMenuItem,
            this.openDestinationToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openSourceToolStripMenuItem
            // 
            this.openSourceToolStripMenuItem.Name = "openSourceToolStripMenuItem";
            this.openSourceToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.openSourceToolStripMenuItem.Text = "Open source...";
            this.openSourceToolStripMenuItem.Click += new System.EventHandler(this.openSourceToolStripMenuItem_Click);
            // 
            // openDestinationToolStripMenuItem
            // 
            this.openDestinationToolStripMenuItem.Name = "openDestinationToolStripMenuItem";
            this.openDestinationToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.openDestinationToolStripMenuItem.Text = "Open destination...";
            this.openDestinationToolStripMenuItem.Click += new System.EventHandler(this.openDestinationToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
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
            this.doGenerateToolStripMenuItem.Size = new System.Drawing.Size(134, 22);
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
            this.showTooltipsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showTooltipsToolStripMenuItem.Text = "Show Tooltips";
            this.showTooltipsToolStripMenuItem.Click += new System.EventHandler(this.showTooltipsToolStripMenuItem_Click);
            // 
            // panelPreview
            // 
            this.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPreview.Controls.Add(this.pictureDestinationPreview);
            this.panelPreview.Controls.Add(this.pictureSourcePreview);
            this.panelPreview.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelPreview.Location = new System.Drawing.Point(587, 24);
            this.panelPreview.Name = "panelPreview";
            this.panelPreview.Size = new System.Drawing.Size(127, 426);
            this.panelPreview.TabIndex = 3;
            this.panelPreview.Visible = false;
            // 
            // showPreviewToolStripMenuItem
            // 
            this.showPreviewToolStripMenuItem.Name = "showPreviewToolStripMenuItem";
            this.showPreviewToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.showPreviewToolStripMenuItem.Text = "Show Preview";
            this.showPreviewToolStripMenuItem.Click += new System.EventHandler(this.showPreviewToolStripMenuItem_Click);
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
            this.graphArea1.Size = new System.Drawing.Size(714, 426);
            this.graphArea1.TabIndex = 2;
            // 
            // pictureSourcePreview
            // 
            this.pictureSourcePreview.Location = new System.Drawing.Point(14, 12);
            this.pictureSourcePreview.Name = "pictureSourcePreview";
            this.pictureSourcePreview.Size = new System.Drawing.Size(100, 100);
            this.pictureSourcePreview.TabIndex = 0;
            this.pictureSourcePreview.TabStop = false;
            // 
            // pictureDestinationPreview
            // 
            this.pictureDestinationPreview.Location = new System.Drawing.Point(14, 118);
            this.pictureDestinationPreview.Name = "pictureDestinationPreview";
            this.pictureDestinationPreview.Size = new System.Drawing.Size(100, 100);
            this.pictureDestinationPreview.TabIndex = 1;
            this.pictureDestinationPreview.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 485);
            this.Controls.Add(this.panelPreview);
            this.Controls.Add(this.graphArea1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.Text = "Imagine";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panelPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureSourcePreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDestinationPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openSourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDestinationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem doGenerateToolStripMenuItem;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.LinkLabel lblDestinationFile;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.LinkLabel lblSourceFile;
        private GraphArea graphArea1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showTooltipsToolStripMenuItem;
        private System.Windows.Forms.Panel panelPreview;
        private System.Windows.Forms.ToolStripMenuItem showPreviewToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureDestinationPreview;
        private System.Windows.Forms.PictureBox pictureSourcePreview;
    }
}

