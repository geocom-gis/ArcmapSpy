﻿namespace ArcmapSpy.Views
{
    partial class FeatureSpyView
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
            this.featureSpyViewWpf1 = new ArcmapSpy.Views.FeatureSpyViewWpf();
            this.SuspendLayout();
            // 
            // wpfHost
            // 
            this.wpfHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(114)))), ((int)(((byte)(173)))), ((int)(((byte)(231)))));
            this.wpfHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfHost.Location = new System.Drawing.Point(0, 0);
            this.wpfHost.Name = "wpfHost";
            this.wpfHost.Size = new System.Drawing.Size(409, 403);
            this.wpfHost.TabIndex = 1;
            this.wpfHost.Child = this.featureSpyViewWpf1;
            // 
            // FeatureSpyView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 403);
            this.Controls.Add(this.wpfHost);
            this.Name = "FeatureSpyView";
            this.Text = "FeatureSpy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FeatureSpyView_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost wpfHost;
        private FeatureSpyViewWpf featureSpyViewWpf1;
    }
}