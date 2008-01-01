namespace Imagine.GUI
{
    partial class GraphArea
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.destinationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.inverterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rgbSplitterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.adder4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.composerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.halverToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brightnessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alphaMultiplier4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sourceToolStripMenuItem,
            this.destinationToolStripMenuItem,
            this.forkToolStripMenuItem,
            this.adder4ToolStripMenuItem,
            this.inverterToolStripMenuItem,
            this.halverToolStripMenuItem,
            this.rgbSplitterToolStripMenuItem,
            this.composerToolStripMenuItem,
            this.brightnessToolStripMenuItem,
            this.hueToolStripMenuItem,
            this.alphaMultiplier4ToolStripMenuItem});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // sourceToolStripMenuItem
            // 
            this.sourceToolStripMenuItem.Name = "sourceToolStripMenuItem";
            this.sourceToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.sourceToolStripMenuItem.Tag = "Imagine.Source";
            this.sourceToolStripMenuItem.Text = "Source";
            this.sourceToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // destinationToolStripMenuItem
            // 
            this.destinationToolStripMenuItem.Name = "destinationToolStripMenuItem";
            this.destinationToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.destinationToolStripMenuItem.Tag = "Imagine.Destination";
            this.destinationToolStripMenuItem.Text = "Destination";
            this.destinationToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // inverterToolStripMenuItem
            // 
            this.inverterToolStripMenuItem.Name = "inverterToolStripMenuItem";
            this.inverterToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.inverterToolStripMenuItem.Tag = "Imagine.Inverter";
            this.inverterToolStripMenuItem.Text = "Inverter";
            this.inverterToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // rgbSplitterToolStripMenuItem
            // 
            this.rgbSplitterToolStripMenuItem.Name = "rgbSplitterToolStripMenuItem";
            this.rgbSplitterToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.rgbSplitterToolStripMenuItem.Tag = "Imagine.RGBSplitter";
            this.rgbSplitterToolStripMenuItem.Text = "RGB Splitter";
            this.rgbSplitterToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // adder4ToolStripMenuItem
            // 
            this.adder4ToolStripMenuItem.Name = "adder4ToolStripMenuItem";
            this.adder4ToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.adder4ToolStripMenuItem.Tag = "Imagine.Adder4";
            this.adder4ToolStripMenuItem.Text = "Adder4";
            this.adder4ToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // forkToolStripMenuItem
            // 
            this.forkToolStripMenuItem.Name = "forkToolStripMenuItem";
            this.forkToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.forkToolStripMenuItem.Tag = "Imagine.Branch4";
            this.forkToolStripMenuItem.Text = "Branch4";
            this.forkToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // composerToolStripMenuItem
            // 
            this.composerToolStripMenuItem.Name = "composerToolStripMenuItem";
            this.composerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.composerToolStripMenuItem.Tag = "Imagine.RGBJoiner";
            this.composerToolStripMenuItem.Text = "RGB Joiner";
            this.composerToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // halverToolStripMenuItem
            // 
            this.halverToolStripMenuItem.Name = "halverToolStripMenuItem";
            this.halverToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.halverToolStripMenuItem.Tag = "Imagine.Halver";
            this.halverToolStripMenuItem.Text = "Halver";
            this.halverToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // brightnessToolStripMenuItem
            // 
            this.brightnessToolStripMenuItem.Name = "brightnessToolStripMenuItem";
            this.brightnessToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.brightnessToolStripMenuItem.Tag = "Imagine.HSLSplitter";
            this.brightnessToolStripMenuItem.Text = "HSL Splitter";
            this.brightnessToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // hueToolStripMenuItem
            // 
            this.hueToolStripMenuItem.Name = "hueToolStripMenuItem";
            this.hueToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hueToolStripMenuItem.Tag = "Imagine.HSLJoiner";
            this.hueToolStripMenuItem.Text = "HSL Joiner";
            this.hueToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // alphaMultiplier4ToolStripMenuItem
            // 
            this.alphaMultiplier4ToolStripMenuItem.Name = "alphaMultiplier4ToolStripMenuItem";
            this.alphaMultiplier4ToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this.alphaMultiplier4ToolStripMenuItem.Tag = "Imagine.AlphaMultiplier4";
            this.alphaMultiplier4ToolStripMenuItem.Text = "Alpha Multiplier4";
            this.alphaMultiplier4ToolStripMenuItem.Click += new System.EventHandler(this.insertToolStripMenuItem_Click);
            // 
            // GraphArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "GraphArea";
            this.Size = new System.Drawing.Size(150, 139);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GraphArea_Paint);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GraphArea_MouseMove);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GraphArea_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GraphArea_MouseUp);
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sourceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem destinationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem inverterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rgbSplitterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem adder4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem composerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem halverToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem brightnessToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alphaMultiplier4ToolStripMenuItem;
    }
}
