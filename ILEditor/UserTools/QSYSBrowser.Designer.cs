﻿namespace ILEditor.UserTools
{
    partial class QsysBrowser
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QsysBrowser));
            this.objectList = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.addSpf = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.addSpf.SuspendLayout();
            this.SuspendLayout();
            // 
            // objectList
            // 
            this.objectList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.objectList.ImageIndex = 0;
            this.objectList.ImageList = this.imageList1;
            this.objectList.Location = new System.Drawing.Point(0, 0);
            this.objectList.Name = "objectList";
            this.objectList.SelectedImageIndex = 0;
            this.objectList.Size = new System.Drawing.Size(321, 365);
            this.objectList.TabIndex = 0;
            this.objectList.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.objectList_BeforeExpand);
            this.objectList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.objectList_NodeMouseDoubleClick);
            this.objectList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objectList_KeyDown);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "info.png");
            this.imageList1.Images.SetKeyName(1, "cubedark.png");
            this.imageList1.Images.SetKeyName(2, "folder.png");
            this.imageList1.Images.SetKeyName(3, "edit.png");
            // 
            // addSpf
            // 
            this.addSpf.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.addSpf.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.addSpf.Location = new System.Drawing.Point(0, 340);
            this.addSpf.Name = "addSpf";
            this.addSpf.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.addSpf.Size = new System.Drawing.Size(321, 25);
            this.addSpf.TabIndex = 1;
            this.addSpf.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::ILEditor.Properties.Resources.folder;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(71, 22);
            this.toolStripButton1.Text = "Add SPF";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // QSYSBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(321, 365);
            this.Controls.Add(this.addSpf);
            this.Controls.Add(this.objectList);
            this.Name = "QsysBrowser";
            this.Load += new System.EventHandler(this.TreeBrowse_Load);
            this.addSpf.ResumeLayout(false);
            this.addSpf.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView objectList;
        private System.Windows.Forms.ToolStrip addSpf;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ImageList imageList1;
    }
}
