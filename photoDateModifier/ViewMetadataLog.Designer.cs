
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
            this.m_dgvMetadataLog = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.m_dgvMetadataLog)).BeginInit();
            this.SuspendLayout();
            // 
            // m_dgvMetadataLog
            // 
            this.m_dgvMetadataLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dgvMetadataLog.Location = new System.Drawing.Point(19, 21);
            this.m_dgvMetadataLog.Margin = new System.Windows.Forms.Padding(5);
            this.m_dgvMetadataLog.Name = "m_dgvMetadataLog";
            this.m_dgvMetadataLog.RowTemplate.Height = 23;
            this.m_dgvMetadataLog.Size = new System.Drawing.Size(2158, 746);
            this.m_dgvMetadataLog.TabIndex = 0;
            // 
            // ViewMetadataLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1924, 788);
            this.Controls.Add(this.m_dgvMetadataLog);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "ViewMetadataLog";
            this.Text = "ViewMetadataLog";
            ((System.ComponentModel.ISupportInitialize)(this.m_dgvMetadataLog)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView m_dgvMetadataLog;
    }
}