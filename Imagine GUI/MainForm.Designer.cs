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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openSourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDestinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.doGenerateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblSource = new System.Windows.Forms.Label();
            this.lblDestination = new System.Windows.Forms.Label();
            this.lblDestinationFile = new System.Windows.Forms.Label();
            this.lblSourceFile = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackgroundImage = global::Imagine_GUI.Properties.Resources.gradient;
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.generateToolStripMenuItem});
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
            this.openSourceToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openSourceToolStripMenuItem.Text = "Open source...";
            this.openSourceToolStripMenuItem.Click += new System.EventHandler(this.openSourceToolStripMenuItem_Click);
            // 
            // openDestinationToolStripMenuItem
            // 
            this.openDestinationToolStripMenuItem.Name = "openDestinationToolStripMenuItem";
            this.openDestinationToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openDestinationToolStripMenuItem.Text = "Open destination...";
            this.openDestinationToolStripMenuItem.Click += new System.EventHandler(this.openDestinationToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
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
            this.doGenerateToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.doGenerateToolStripMenuItem.Text = "Generate!";
            this.doGenerateToolStripMenuItem.Click += new System.EventHandler(this.doGenerateToolStripMenuItem_Click);
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
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 485);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "MainForm";
            this.Text = "Imagine";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
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
        private System.Windows.Forms.Label lblDestinationFile;
        private System.Windows.Forms.Label lblDestination;
        private System.Windows.Forms.Label lblSourceFile;
    }
}

