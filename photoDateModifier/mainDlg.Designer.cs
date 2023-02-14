namespace photoDateModifier
{
    partial class mainDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.m_btnChooseFile = new System.Windows.Forms.Button();
            this.m_pBImage = new System.Windows.Forms.PictureBox();
            this.m_listViewProperties = new System.Windows.Forms.ListView();
            this.hexID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DS = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_cmbImages = new System.Windows.Forms.ComboBox();
            this.m_btnUpdateAndJump = new System.Windows.Forms.Button();
            this.m_lblCounter = new System.Windows.Forms.Label();
            this.m_btnUpdate = new System.Windows.Forms.Button();
            this.m_btnChooseFolder = new System.Windows.Forms.Button();
            this.m_btnLoadMetadataLog = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.m_pBImage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Image";
            // 
            // m_btnChooseFile
            // 
            this.m_btnChooseFile.Location = new System.Drawing.Point(897, 9);
            this.m_btnChooseFile.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_btnChooseFile.Name = "m_btnChooseFile";
            this.m_btnChooseFile.Size = new System.Drawing.Size(34, 22);
            this.m_btnChooseFile.TabIndex = 2;
            this.m_btnChooseFile.Text = "&...";
            this.m_btnChooseFile.UseVisualStyleBackColor = true;
            this.m_btnChooseFile.Click += new System.EventHandler(this.m_btnChooseFile_Click);
            // 
            // m_pBImage
            // 
            this.m_pBImage.BackColor = System.Drawing.Color.White;
            this.m_pBImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.m_pBImage.ErrorImage = null;
            this.m_pBImage.InitialImage = null;
            this.m_pBImage.Location = new System.Drawing.Point(18, 49);
            this.m_pBImage.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_pBImage.Name = "m_pBImage";
            this.m_pBImage.Size = new System.Drawing.Size(492, 305);
            this.m_pBImage.TabIndex = 3;
            this.m_pBImage.TabStop = false;
            this.m_pBImage.Visible = false;
            // 
            // m_listViewProperties
            // 
            this.m_listViewProperties.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.m_listViewProperties.AllowColumnReorder = true;
            this.m_listViewProperties.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.hexID,
            this.DS,
            this.Value,
            this.Status});
            this.m_listViewProperties.FullRowSelect = true;
            this.m_listViewProperties.HideSelection = false;
            this.m_listViewProperties.HoverSelection = true;
            this.m_listViewProperties.Location = new System.Drawing.Point(518, 49);
            this.m_listViewProperties.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_listViewProperties.MultiSelect = false;
            this.m_listViewProperties.Name = "m_listViewProperties";
            this.m_listViewProperties.Size = new System.Drawing.Size(779, 348);
            this.m_listViewProperties.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.m_listViewProperties.TabIndex = 6;
            this.m_listViewProperties.UseCompatibleStateImageBehavior = false;
            this.m_listViewProperties.View = System.Windows.Forms.View.Details;
            this.m_listViewProperties.Visible = false;
            // 
            // hexID
            // 
            this.hexID.Text = "hexID";
            // 
            // DS
            // 
            this.DS.Text = "Ds";
            // 
            // Value
            // 
            this.Value.Text = "Value";
            this.Value.Width = 180;
            // 
            // Status
            // 
            this.Status.Text = "Status";
            this.Status.Width = 102;
            // 
            // m_cmbImages
            // 
            this.m_cmbImages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbImages.FormattingEnabled = true;
            this.m_cmbImages.Location = new System.Drawing.Point(97, 7);
            this.m_cmbImages.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_cmbImages.Name = "m_cmbImages";
            this.m_cmbImages.Size = new System.Drawing.Size(731, 20);
            this.m_cmbImages.TabIndex = 10;
            this.m_cmbImages.SelectedIndexChanged += new System.EventHandler(this.onChangeImageCombo);
            // 
            // m_btnUpdateAndJump
            // 
            this.m_btnUpdateAndJump.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.m_btnUpdateAndJump.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnUpdateAndJump.Location = new System.Drawing.Point(1086, 9);
            this.m_btnUpdateAndJump.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_btnUpdateAndJump.Name = "m_btnUpdateAndJump";
            this.m_btnUpdateAndJump.Size = new System.Drawing.Size(211, 22);
            this.m_btnUpdateAndJump.TabIndex = 11;
            this.m_btnUpdateAndJump.Text = "&Update and Jump";
            this.m_btnUpdateAndJump.UseVisualStyleBackColor = true;
            this.m_btnUpdateAndJump.Visible = false;
            this.m_btnUpdateAndJump.Click += new System.EventHandler(this.m_btnUpdateAndJump_Click);
            // 
            // m_lblCounter
            // 
            this.m_lblCounter.AutoSize = true;
            this.m_lblCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblCounter.Location = new System.Drawing.Point(848, 11);
            this.m_lblCounter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.m_lblCounter.Name = "m_lblCounter";
            this.m_lblCounter.Size = new System.Drawing.Size(27, 13);
            this.m_lblCounter.TabIndex = 12;
            this.m_lblCounter.Text = "0/0";
            this.m_lblCounter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_btnUpdate
            // 
            this.m_btnUpdate.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.m_btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.m_btnUpdate.Location = new System.Drawing.Point(406, 374);
            this.m_btnUpdate.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_btnUpdate.Name = "m_btnUpdate";
            this.m_btnUpdate.Size = new System.Drawing.Size(104, 23);
            this.m_btnUpdate.TabIndex = 9;
            this.m_btnUpdate.Text = "&Update EXIF";
            this.m_btnUpdate.UseVisualStyleBackColor = true;
            this.m_btnUpdate.Visible = false;
            this.m_btnUpdate.Click += new System.EventHandler(this.m_btnUpdate_Click);
            // 
            // m_btnChooseFolder
            // 
            this.m_btnChooseFolder.Location = new System.Drawing.Point(939, 9);
            this.m_btnChooseFolder.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.m_btnChooseFolder.Name = "m_btnChooseFolder";
            this.m_btnChooseFolder.Size = new System.Drawing.Size(139, 22);
            this.m_btnChooseFolder.TabIndex = 13;
            this.m_btnChooseFolder.Text = "&Open Folder";
            this.m_btnChooseFolder.UseVisualStyleBackColor = true;
            this.m_btnChooseFolder.Click += new System.EventHandler(this.m_btnChooseFolder_Click);
            // 
            // m_btnLoadMetadataLog
            // 
            this.m_btnLoadMetadataLog.Location = new System.Drawing.Point(188, 374);
            this.m_btnLoadMetadataLog.Name = "m_btnLoadMetadataLog";
            this.m_btnLoadMetadataLog.Size = new System.Drawing.Size(211, 23);
            this.m_btnLoadMetadataLog.TabIndex = 14;
            this.m_btnLoadMetadataLog.Text = "Load Metadata Log";
            this.m_btnLoadMetadataLog.UseVisualStyleBackColor = true;
            this.m_btnLoadMetadataLog.Click += new System.EventHandler(this.m_btnLoadMetadataLog_Click);
            // 
            // mainDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1310, 414);
            this.Controls.Add(this.m_btnLoadMetadataLog);
            this.Controls.Add(this.m_btnChooseFolder);
            this.Controls.Add(this.m_lblCounter);
            this.Controls.Add(this.m_btnUpdateAndJump);
            this.Controls.Add(this.m_cmbImages);
            this.Controls.Add(this.m_btnUpdate);
            this.Controls.Add(this.m_listViewProperties);
            this.Controls.Add(this.m_pBImage);
            this.Controls.Add(this.m_btnChooseFile);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.Name = "mainDlg";
            this.Text = "JPG 파일 촬영시간 보정 프로그램";
            this.Resize += new System.EventHandler(this.onResize);
            ((System.ComponentModel.ISupportInitialize)(this.m_pBImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button m_btnChooseFile;
        private System.Windows.Forms.PictureBox m_pBImage;
        private System.Windows.Forms.ListView m_listViewProperties;
        private System.Windows.Forms.ColumnHeader DS;
        private System.Windows.Forms.ColumnHeader Value;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.ComboBox m_cmbImages;
        private System.Windows.Forms.Button m_btnUpdateAndJump;
        private System.Windows.Forms.Label m_lblCounter;
        private System.Windows.Forms.ColumnHeader hexID;
        private System.Windows.Forms.Button m_btnUpdate;
        private System.Windows.Forms.Button m_btnChooseFolder;
        private System.Windows.Forms.Button m_btnLoadMetadataLog;
    }
}

