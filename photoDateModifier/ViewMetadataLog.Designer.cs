
namespace photoDateModifier
{
    partial class ViewMetadataLog
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
            this.components = new System.ComponentModel.Container();
            this.m_fastObjectListView = new BrightIdeasSoftware.FastObjectListView();
            this.m_toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.m_fastObjectListView)).BeginInit();
            this.SuspendLayout();
            // 
            // m_fastObjectListView
            // 
            this.m_fastObjectListView.CellEditUseWholeCell = false;
            this.m_fastObjectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_fastObjectListView.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.m_fastObjectListView.HideSelection = false;
            this.m_fastObjectListView.Location = new System.Drawing.Point(0, 0);
            this.m_fastObjectListView.MinimumSize = new System.Drawing.Size(300, 100);
            this.m_fastObjectListView.Name = "m_fastObjectListView";
            this.m_fastObjectListView.RowHeight = 9;
            this.m_fastObjectListView.ShowGroups = false;
            this.m_fastObjectListView.Size = new System.Drawing.Size(1624, 561);
            this.m_fastObjectListView.TabIndex = 0;
            this.m_fastObjectListView.UseCompatibleStateImageBehavior = false;
            this.m_fastObjectListView.UseFilterIndicator = true;
            this.m_fastObjectListView.UseFiltering = true;
            this.m_fastObjectListView.UseHyperlinks = true;
            this.m_fastObjectListView.View = System.Windows.Forms.View.Details;
            this.m_fastObjectListView.VirtualMode = true;
            this.m_fastObjectListView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.m_listMouseMove);
            // 
            // ViewMetadataLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1624, 561);
            this.Controls.Add(this.m_fastObjectListView);
            this.Name = "ViewMetadataLog";
            this.Text = "Metadata modify log Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.m_fastObjectListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView m_fastObjectListView;
        private System.Windows.Forms.ToolTip m_toolTip;
    }
}