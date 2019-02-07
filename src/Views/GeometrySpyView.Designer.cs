namespace ArcmapSpy.Views
{
    partial class GeometrySpyView
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
            if (disposing && (components != null))
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
            this.wpfHost = new System.Windows.Forms.Integration.ElementHost();
            this.applicationSpyViewWpf1 = new ArcmapSpy.Views.ApplicationSpyViewWpf();
            this.SuspendLayout();
            // 
            // wpfHost
            // 
            this.wpfHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(173)))), ((int)(((byte)(231)))));
            this.wpfHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfHost.Location = new System.Drawing.Point(0, 0);
            this.wpfHost.Name = "wpfHost";
            this.wpfHost.Size = new System.Drawing.Size(617, 351);
            this.wpfHost.TabIndex = 1;
            this.wpfHost.Text = "elementHost1";
            this.wpfHost.Child = this.applicationSpyViewWpf1;
            // 
            // GeometrySpyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 351);
            this.Controls.Add(this.wpfHost);
            this.Name = "GeometrySpyView";
            this.Text = "GeometrySpyView";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.GeometrySpyView_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost wpfHost;
        private ApplicationSpyViewWpf applicationSpyViewWpf1;
    }
}