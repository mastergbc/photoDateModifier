/// <summary>
/// Base design used exifEditor: https://1drv.ms/u/s!AmsqTf3EgmJAoZdvU1tuW0xXWl-LTA by https://happybono.wordpress.com/
/// The code of ExifPhotoReader was modified at my own discretion.
///   - v1.0.4 clone by 2023.02.14: https://github.com/andersonpereiradossantos/dotnet-exif-photo-reader
/// </summary>
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace photoDateModifier
{
    public partial class mainDlg : Form, IDisposable
    {
        private String m_currImageFile;
        private String m_currImageFileTemp;
        private String m_currFolder;
        private volatile bool isAutoLoadEnabled = false;
        private List<string> fileNamesList = null;
        private bool isCheckedFileTimePrioty = false;

        public mainDlg()
        {
            InitializeComponent();
            m_currImageFile = m_currImageFileTemp = "";

            // Delete the temporary image that was previously created.
            try
            {
                DirectoryInfo di = new DirectoryInfo(Path.GetTempPath());
                FileInfo[] fi = di.GetFiles("__exifEditor__*.*");
                foreach (FileInfo f in fi)
                {
                    File.Delete(f.FullName);
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show("Deleting temporary files Error: " + excep.Message);
            }
        }

        private void m_btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "jpg files (*.jpg,*.jpeg)|*.jpg;*.jpeg";
                openDialog.FilterIndex = 1;
                openDialog.Multiselect = true;
                openDialog.RestoreDirectory = true;
                openDialog.CheckFileExists = true;
                openDialog.CheckPathExists = true;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    m_currFolder = Path.GetDirectoryName(openDialog.FileName);
                    fileNamesList = openDialog.FileNames.ToList();
                    m_cmbImages.DataSource = openDialog.FileNames.ToList();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("m_btnChooseFile_Click error: " + exception.Message);
            }
        }

        private void m_btnChooseFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.RestoreDirectory = true;
            dialog.EnsurePathExists = true;
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                m_currFolder = Path.GetDirectoryName(dialog.FileName);
                List<string> jpgFiles = System.IO.Directory.GetFiles(dialog.FileName, "*.jpg", SearchOption.AllDirectories).ToList();
                List<string> jpegFiles = System.IO.Directory.GetFiles(dialog.FileName, "*.jpeg", SearchOption.AllDirectories).ToList();
                fileNamesList = jpgFiles.Concat(jpegFiles).ToList();
                m_cmbImages.DataSource = fileNamesList;
            }
        }

        private void ShowControls(bool p)
        {
            m_pBImage.Visible = p;
            m_listViewProperties.Visible = p;
            m_btnUpdate.Visible = p;
            m_btnUpdateAndJump.Visible = p;
        }

        private void onResize(object sender, EventArgs e)
        {
            Rectangle clientRect = this.ClientRectangle;
            int sideMargin = 16;
            int upperMargin = 43;
            int bottomMargin = 40;

            // 이미지
            m_pBImage.Left = sideMargin;
            m_pBImage.Top = upperMargin;
            m_pBImage.Width = (clientRect.Width - (3 * sideMargin)) / 2;
            m_pBImage.Height = clientRect.Height - bottomMargin - upperMargin;

            // 속성
            m_listViewProperties.Left = (clientRect.Width / 2) + (sideMargin / 2);
            m_listViewProperties.Top = upperMargin;
            m_listViewProperties.Width = (clientRect.Width - (3 * sideMargin)) / 2;
            m_listViewProperties.Height = clientRect.Height - bottomMargin - upperMargin;

            // 업데이트 버튼
            m_btnUpdate.Top = clientRect.Height - 30;

            // 업데이트 및 넘기기 버튼
            m_btnUpdateAndJump.Left = m_btnUpdate.Right + sideMargin;
            m_btnUpdateAndJump.Top = clientRect.Height - 30;
        }

        private void m_btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                // 저장된 이미지를 다시 로드합니다.
                ShowControls(false);
                LoadCurrImage();
            }
            catch (Exception excp)
            {
                MessageBox.Show("m_btnUpdate_Click 오류 : " + excp.Message);
            }
        }

        private void m_btnUpdateAndJump_Click(object sender, EventArgs e)
        {
            try
            {
                isAutoLoadEnabled = true;

                if (isAutoLoadEnabled)
                {
                    isCheckedFileTimePrioty = m_checkBoxFileTimePrioty.Checked;
                    // start loading images in sequence
                    m_btnUpdateAndJump.Text = "Stop";

                    ShowControls(false);
                    m_pBImage.Image.Dispose();
                    ProcessImages(fileNamesList);
                    isAutoLoadEnabled = false;
                    Console.Beep();

                    if (m_cmbImages.SelectedIndex == fileNamesList.Count - 1)
                    {
                        onChangeImageCombo(null, null);
                    }
                    else
                    {
                        if (m_cmbImages.InvokeRequired)
                        {
                            m_cmbImages.Invoke(method: (MethodInvoker)delegate
                            {
                                m_cmbImages.SelectedIndex = fileNamesList.Count - 1;
                            });
                        }
                        else
                        {
                            m_cmbImages.SelectedIndex = fileNamesList.Count - 1;
                        }
                    }
                    m_btnUpdateAndJump.Text = "Update and Jump";
                }
                else
                {
                    // stop loading images
                    m_btnUpdateAndJump.Text = "Update and Jump";
                }
            }
            catch (Exception excp)
            {
                MessageBox.Show("Error m_btnUpdate_Click 오류 : " + excp.Message);
            }
        }

        private void onChangeImageCombo(object sender, EventArgs e)
        {
            m_currImageFile = m_cmbImages.SelectedItem.ToString();
            m_lblCounter.Text = Convert.ToString(m_cmbImages.SelectedIndex + 1) + " / " + m_cmbImages.Items.Count;
            m_cmbImages.Focus();
            LoadCurrImage();
        }

        private void m_btnLoadMetadataLog_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.InitialDirectory = "g:\\Temp";
                openFileDialog.Filter = "JSON Files (*.json)|*.json";
                openFileDialog.Title = "Select JSON File";
                openFileDialog.Multiselect = false;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string fileName = openFileDialog.FileName;

                    // Pass the selected file name to the ViewMetadataLog method
                    var viewMetadataLog = new ViewMetadataLog(fileName);
                    viewMetadataLog.ShowDialog();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("m_btnLoadMetadataLog_Click error: " + exception.Message);
            }
        }

    }

}
