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
            this.portContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.branchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.breakConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.portContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(96, 26);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(95, 22);
            this.newToolStripMenuItem.Text = "New";
            // 
            // portContextMenu
            // 
            this.portContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.breakConnectionToolStripMenuItem,
            this.branchToolStripMenuItem});
            this.portContextMenu.Name = "portContextMenu";
            this.portContextMenu.Size = new System.Drawing.Size(165, 70);
            // 
            // branchToolStripMenuItem
            // 
            this.branchToolStripMenuItem.Name = "branchToolStripMenuItem";
            this.branchToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.branchToolStripMenuItem.Text = "Branch Connection";
            this.branchToolStripMenuItem.Click += new System.EventHandler(this.branchToolStripMenuItem_Click);
            // 
            // breakConnectionToolStripMenuItem
            // 
            this.breakConnectionToolStripMenuItem.Name = "breakConnectionToolStripMenuItem";
            this.breakConnectionToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.breakConnectionToolStripMenuItem.Text = "Break Connection";
            this.breakConnectionToolStripMenuItem.Click += new System.EventHandler(this.breakConnectionToolStripMenuItem_Click);
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
            this.portContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip portContextMenu;
        private System.Windows.Forms.ToolStripMenuItem branchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem breakConnectionToolStripMenuItem;
    }
}
