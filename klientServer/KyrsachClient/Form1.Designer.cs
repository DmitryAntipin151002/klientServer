namespace KyrsachClient
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            menuStrip1 = new MenuStrip();
            kgtkgtkkgtToolStripMenuItem = new ToolStripMenuItem();
            filesToolStripMenuItem = new ToolStripMenuItem();
            panelchildform = new Panel();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.ActiveCaptionText;
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { kgtkgtkkgtToolStripMenuItem, filesToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(732, 28);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // kgtkgtkkgtToolStripMenuItem
            // 
            kgtkgtkkgtToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight;
            kgtkgtkkgtToolStripMenuItem.Name = "kgtkgtkkgtToolStripMenuItem";
            kgtkgtkkgtToolStripMenuItem.Size = new Size(98, 24);
            kgtkgtkkgtToolStripMenuItem.Text = "Connection";
            kgtkgtkkgtToolStripMenuItem.Click += kgtkgtkkgtToolStripMenuItem_Click;
            // 
            // filesToolStripMenuItem
            // 
            filesToolStripMenuItem.ForeColor = SystemColors.ButtonHighlight;
            filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            filesToolStripMenuItem.Size = new Size(52, 24);
            filesToolStripMenuItem.Text = "Files";
            // 
            // panelchildform
            // 
            panelchildform.BackColor = Color.Transparent;
            panelchildform.Dock = DockStyle.Fill;
            panelchildform.Location = new Point(0, 28);
            panelchildform.Name = "panelchildform";
            panelchildform.Size = new Size(732, 402);
            panelchildform.TabIndex = 1;
            panelchildform.Paint += panelchildform_Paint;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            BackgroundImage = Properties.Resources.c7419bd22b20d999bad14d68ba66ad2b;
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new Size(732, 430);
            Controls.Add(panelchildform);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(750, 477);
            Name = "Form1";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem kgtkgtkkgtToolStripMenuItem;
        private ToolStripMenuItem filesToolStripMenuItem;
        private Panel panelchildform;
    }
}