using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using System.Globalization;
using XmpCore;
using MetadataExtractor;
using MetadataExtractor.Formats.Xmp;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using ExifPhotoReader;

/// <summary>
/// Base design used exifEditor: https://1drv.ms/u/s!AmsqTf3EgmJAoZdvU1tuW0xXWl-LTA by https://happybono.wordpress.com/
/// 
/// </summary>
namespace photoDateModifier
{
    public partial class mainDlg : Form, IDisposable
    {
        private readonly object _lock = new object();
        private String m_currImageFile;
        private String m_currImageFileTemp;
        private volatile bool isAutoLoadEnabled = false;
        ConcurrentDictionary<int, MetadataModifyLog> jsonList = new ConcurrentDictionary<int, MetadataModifyLog>();
        private List<string> fileNamesList = null;

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
                MessageBox.Show("임시 파일 삭제 오류 : " + excep.Message);
            }
        }

        private void m_btnChooseFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();

                openDialog.InitialDirectory = "g:\\Temp";
                openDialog.Filter = "jpg files (*.jpg,*.jpeg)|*.jpg;*.jpeg";
                //openDialog.Filter = "jpg files (*.jpg, *.jpeg)|*.jpg,*.jpeg"; //|tif files (*.tif)|*.tif|bmp files (*.bmp)|*.bmp|png files (*.png)|*.png";
                openDialog.FilterIndex = 1;
                openDialog.Multiselect = true;
                openDialog.RestoreDirectory = true;
                openDialog.CheckFileExists = true;
                openDialog.CheckPathExists = true;

                if (openDialog.ShowDialog() == DialogResult.OK)
                {
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
            dialog.IsFolderPicker = true; // true : select folder / false : select file

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                List<string> jpgFiles = System.IO.Directory.GetFiles(dialog.FileName, "*.jpg", SearchOption.AllDirectories).ToList();
                List<string> jpegFiles = System.IO.Directory.GetFiles(dialog.FileName, "*.jpeg", SearchOption.AllDirectories).ToList();
                fileNamesList = jpgFiles.Concat(jpegFiles).ToList();
                m_cmbImages.DataSource = fileNamesList;
            }
        }

        private void LoadCurrImage()
        {
            if (m_currImageFile == "") MessageBox.Show("LoadCurrImage 오류 : 연결된 이미지가 없습니다.");

            Cursor = Cursors.WaitCursor;
            try
            {
                m_currImageFileTemp = Path.GetTempPath() + "__exifEditor__" + Path.GetRandomFileName();
                File.Copy(m_currImageFile, m_currImageFileTemp);
                m_listViewProperties.Items.Clear();
                if (m_pBImage.Image != null)
                {
                    m_pBImage.Image.Dispose();
                }
                m_pBImage.SizeMode = PictureBoxSizeMode.Zoom;
                m_pBImage.Image = Bitmap.FromFile(m_currImageFileTemp);

                ListViewItem row = new ListViewItem();
                ImageMetadata imageMetadata = new ImageMetadata();

                // Image Property 얻기
                /// <summary>
                /// m_currImageFileTemp의 image PropertyItems
                /// </summary>
                PropertyItem[] propItems = m_pBImage.Image.PropertyItems;
                /// <summary >
                /// m_currImageFileTemp의 Fileinfo
                /// </summary>
                FileInfo fileInfo = null;
                /// <summary >
                /// m_currImageFileTemp의 exifProperties
                /// </summary>
                ExifImageProperties exifImage = null;
                fileInfo = new FileInfo(m_currImageFileTemp);
                try
                {
                    exifImage = ExifPhoto.GetExifDataPhoto(m_currImageFileTemp);
                }
                catch (Exception excep3)
                {
                    MessageBox.Show("Error LoadCurrImage exifImage: " + excep3.Message + m_currImageFileTemp);
                }
                try
                {
                    // Load the metadata for the image file
                    /// 참고가 되는 태그 키워드를 기록하기 위한 exif 속성값.
                    string imageDescription = "";
                    row = LoadMetaData(imageMetadata, propItems, fileInfo, exifImage, ref imageDescription);

                    m_listViewProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    onResize(null, null);
                    ShowControls(true);
                    Cursor = Cursors.Default;
                }
                catch (Exception excep2)
                {
                    MessageBox.Show("Error LoadCurrImage copy: " + excep2.Message + m_currImageFile);
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show("Error LoadCurrImage: " + excep.Message + m_currImageFile);
            }
        }

        private void LoadCurrImage(string currImageFile, int currIndex)
        {
            if (currImageFile == "") MessageBox.Show("LoadCurrImage 오류 : 연결된 이미지가 없습니다.");

            try
            {
                ImageMetadata imageMetadata = new ImageMetadata();
                imageMetadata.folderName = Path.GetDirectoryName(currImageFile).Split(Path.DirectorySeparatorChar).Last();
                imageMetadata.fileName = Path.GetFileNameWithoutExtension(currImageFile);
                string currImageFileTemp = Path.GetTempPath() + "__exifEditor__" + Path.GetRandomFileName();
                File.Copy(currImageFile, currImageFileTemp);
                Encoding _Encoding = Encoding.UTF8;

                /// Get the XMP metadata directory
                {
                    var xmpDirectory = ImageMetadataReader.ReadMetadata(currImageFileTemp).OfType<XmpDirectory>().FirstOrDefault();
                    // Check if the XMP metadata directory is present
                    if (xmpDirectory != null)
                    {
                        IXmpPropertyInfo xmpDateAcquiredPropertyItem = xmpDirectory.XmpMeta.Properties.FirstOrDefault(p => p.Path == "MicrosoftPhoto:DateAcquired");
                        if (xmpDateAcquiredPropertyItem != null)
                        {
                            // Get the value of the 'MICROSOFTPHOTO:DATEACQUIRED' property
                            DateTime.TryParse(xmpDateAcquiredPropertyItem.Value, out DateTime tempDate);
                            imageMetadata.xmpDateAcquiredTime = tempDate;
                        }
                        else
                        {
                            Console.WriteLine("The 'MICROSOFTPHOTO:DATEACQUIRED' property was not found in the XMP metadata.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("XMP metadata not found in the image file.");
                    }
                }

                /// 참고가 되는 태그 키워드를 기록하기 위한 exif 속성값.
                string imageDescription = "";
                using (Image images = Bitmap.FromFile(currImageFileTemp))
                {
                    /// currImageFileTemp의 Fileinfo
                    FileInfo fileInfo = new FileInfo(currImageFileTemp);
                    /// currImageFileTemp의 image PropertyItems
                    PropertyItem[] propItems = images.PropertyItems;
                    /// images의 exifProperties
                    var exifImage = ExifPhoto.GetExifDataPhoto(images);
                    LoadMetaData(currIndex, imageMetadata, propItems, fileInfo, exifImage, ref imageDescription);

                    //Save image part
                    var jsonLog = new MetadataModifyLog();
                    jsonLog.Number = currIndex;
                    //SaveMetadataImage(currImageFileTemp, cloneImages, imageMetadata, imageDescription, ref jsonLog);
                    var dateTimeBytes = _Encoding.GetBytes(imageMetadata.earliestDate.ToString("yyyy:MM:dd HH:mm:ss\0"));
                    try
                    {
                        jsonLog.FolderName = imageMetadata.folderName;
                        jsonLog.FileName = imageMetadata.fileName;
                        jsonLog.FilenameDateTime = imageMetadata.fileNameDateTime;
                        jsonLog.FoldernameDateTime = imageMetadata.folderNameDateTime;
                        jsonLog.EarliestDate = imageMetadata.earliestDate;
                        jsonLog.DateAcquired = imageMetadata.xmpDateAcquiredTime;

                        DateTime previousDateTime = imageMetadata.exifDateTime;
                        if (imageMetadata.exifDateTime != imageMetadata.earliestDate)
                        {
                            PropertyItem dateTimePropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.dateTimeTag);
                            if (dateTimePropertyItem != null)
                            {
                                var temp = Encoding.UTF8.GetString(dateTimePropertyItem.Value).TrimEnd('\0');
                                // DateTime.TryParse(temp, out previousDateTime) 구문으로 파싱되지 않는데 이유를 모르겠다...
                                DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out previousDateTime);
                            }
                            else
                            {
                                dateTimePropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                                dateTimePropertyItem.Id = (int)ExifTags.dateTimeTag;
                                dateTimePropertyItem.Type = 2;
                                dateTimePropertyItem.Len = 20;
                            }
                            dateTimePropertyItem.Value = dateTimeBytes;
                            images.SetPropertyItem(dateTimePropertyItem);
                        }
                        jsonLog.BeforeDateTime = previousDateTime;
                        jsonLog.AfterDateTime = imageMetadata.earliestDate;

                        DateTime previousDateTimeOriginal = imageMetadata.exifDateTimeOriginal;
                        if (imageMetadata.exifDateTimeOriginal != imageMetadata.earliestDate)
                        {
                            PropertyItem dateTimeOriginalPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.dateTimeOriginalTag);
                            if (dateTimeOriginalPropertyItem != null)
                            {
                                var temp = Encoding.UTF8.GetString(dateTimeOriginalPropertyItem.Value).TrimEnd('\0');
                                DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out previousDateTimeOriginal);
                            }
                            else
                            {
                                dateTimeOriginalPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                                dateTimeOriginalPropertyItem.Id = (int)ExifTags.dateTimeOriginalTag;
                                dateTimeOriginalPropertyItem.Type = 2;
                                dateTimeOriginalPropertyItem.Len = 20;
                            }
                            dateTimeOriginalPropertyItem.Value = dateTimeBytes;
                            images.SetPropertyItem(dateTimeOriginalPropertyItem);
                        }
                        jsonLog.BeforeDateTimeOriginal = previousDateTimeOriginal;
                        jsonLog.AfterDateTimeOriginal = imageMetadata.earliestDate;

                        DateTime previousDateTimeDigitized = imageMetadata.exifDateTimeDigitized;
                        if (imageMetadata.exifDateTimeDigitized != imageMetadata.earliestDate)
                        {
                            PropertyItem dateTimeDigitizedPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.dateTimeDigitizedTag);
                            if (dateTimeDigitizedPropertyItem != null)
                            {
                                var temp = Encoding.UTF8.GetString(dateTimeDigitizedPropertyItem.Value).TrimEnd('\0');
                                DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out previousDateTimeDigitized);
                            }
                            else
                            {
                                dateTimeDigitizedPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                                dateTimeDigitizedPropertyItem.Id = (int)ExifTags.dateTimeDigitizedTag;
                                dateTimeDigitizedPropertyItem.Type = 2;
                                dateTimeDigitizedPropertyItem.Len = 20;
                            }
                            dateTimeDigitizedPropertyItem.Value = dateTimeBytes;
                            images.SetPropertyItem(dateTimeDigitizedPropertyItem);
                        }
                        jsonLog.BeforeDateTimeDigitized = previousDateTimeDigitized;
                        jsonLog.AfterDateTimeDigitized = imageMetadata.earliestDate;

                        string previousImageDescription = imageMetadata.imageDescriptionTemp;
                        if (imageDescription == null)
                        {
                            // No change
                        }
                        else if (imageMetadata.imageDescriptionTemp == null || !imageDescription.Equals(imageMetadata.imageDescriptionTemp))
                        {
                            PropertyItem imageDescriptionPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.imageDescriptionTag);
                            var imageDescriptionVal = _Encoding.GetBytes(imageDescription.ToString() + "\0");
                            if (imageDescriptionPropertyItem != null)
                            {
                                // previousImageDescription = _Encoding.GetString(imageDescriptionPropertyItem.Value).TrimEnd('\0');
                            }
                            else
                            {
                                imageDescriptionPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                                imageDescriptionPropertyItem.Id = (int)ExifTags.imageDescriptionTag;
                                imageDescriptionPropertyItem.Type = 2;
                                imageDescriptionPropertyItem.Len = imageDescription.Length + 1;
                            }
                            imageDescriptionPropertyItem.Value = imageDescriptionVal;
                            images.SetPropertyItem(imageDescriptionPropertyItem);

                        }
                        jsonLog.BeforeImageDescription = previousImageDescription;
                        jsonLog.AfterImageDescription = imageDescription;

                        //update fileinfo property
                        jsonLog.BeforeCreationTime = imageMetadata.fileCreationTime;
                        jsonLog.AfterCreationTime = imageMetadata.earliestDate;
                        jsonLog.BeforeLastWriteTime = imageMetadata.fileLastWriteTime;
                        jsonLog.AfterLastWriteTime = imageMetadata.earliestDate;
                        jsonLog.BeforeLastAccessTime = imageMetadata.fileLastAccessTime;
                        jsonLog.AfterLastAccessTime = imageMetadata.earliestDate;

                        bool fileSaved = false;
                        while (!fileSaved)
                        {
                            try
                            {
                                images.Save(currImageFile, ImageFormat.Jpeg);// xmpDirectory metadata가 유실된다.
                                fileSaved = true;
                            }
                            catch (IOException)
                            {
                                // Wait for a moment and try again
                                System.Threading.Thread.Sleep(1);
                            }
                        }

                        ///FileInfo Save
                        try
                        {
                            if (imageMetadata.fileCreationTime != imageMetadata.earliestDate)
                            {
                                File.SetCreationTime(currImageFile, imageMetadata.earliestDate);
                            }

                            if (imageMetadata.fileLastWriteTime != imageMetadata.earliestDate)
                            {
                                File.SetLastWriteTime(currImageFile, imageMetadata.earliestDate);
                            }

                            if (imageMetadata.fileLastAccessTime != imageMetadata.earliestDate)
                            {
                                File.SetLastAccessTime(currImageFile, imageMetadata.earliestDate);
                            }
                        }
                        catch (Exception exfile)
                        {
                            MessageBox.Show("Error SaveFileInfo: " + exfile.InnerException + exfile.Message + currImageFile);
                        }

                    }
                    catch (Exception exprop)
                    {
                        MessageBox.Show("Error SaveImage: " + exprop.Message + currImageFile);
                    }

                    bool passFlag = jsonList.TryAdd(currIndex, jsonLog);
                    jsonList.TryGetValue(currIndex, out MetadataModifyLog metadataModifyLog);
                    Console.WriteLine(currIndex + ":" + jsonLog.FileName + "{0} {1}", passFlag, metadataModifyLog.FileName);
                } // image Auto Disposed
                try
                {
                    File.Delete(currImageFileTemp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting file " + currImageFileTemp + ": " + ex.Message);
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show("Error LoadCurrImage: " + excep.Message + currImageFile);
            }
        }

        private ListViewItem LoadMetaData(ImageMetadata imageMetadata, PropertyItem[] propItems,
                                          FileInfo fileInfo, ExifImageProperties exifImage, ref string imageDescription)
        {
            try
            {
                Encoding _Encoding = Encoding.UTF8;
                ListViewItem row;

                // exifImage.ImageDescription는 읽는 과정에서 이미 한글이 깨져서 쓸 수 없다.
                // Image.Property 에서 읽어와서 다시 인코딩.
                PropertyItem propertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.imageDescriptionTag);
                row = new ListViewItem(((int)ExifTags.imageDescriptionTag).ToString("X4"));
                row.SubItems.Add("ImageDescription");
                if (propertyItem != null)
                {
                    imageDescription = _Encoding.GetString(propertyItem.Value).TrimEnd('\0');
                    row.SubItems.Add(imageDescription);
                }
                else
                {
                    row.SubItems.Add("");
                }
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);
                // 기존 imageDescriptionTemp 내용을 백업.
                imageMetadata.imageDescriptionTemp = imageDescription;

                // Part of reading time information.
                // The date inferred from the folder name, file name
                //   folderNameDateTime uses only year, month, and day.
                //   Date difference within 15 days based on folderNameDateTime is ignored.
                string folderName = Path.GetDirectoryName(m_currImageFile).Split(Path.DirectorySeparatorChar).Last();
                string fileName = Path.GetFileNameWithoutExtension(m_currImageFile);
                string pattern = @"\d{8}";
                Match match = Regex.Match(folderName, pattern);
                if (match.Success)
                {
                    string dateString = match.Value;
                    imageMetadata.folderNameDateTime = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
                    row = new ListViewItem(0xff01.ToString("X4"));
                    row.SubItems.Add("FolderName DateTime");
                    row.SubItems.Add(imageMetadata.folderNameDateTime.ToString());// 파일로서의 생성 시간 (복사하면 갱신됨)
                    row.SubItems.Add("Unedited");
                    m_listViewProperties.Items.Add(row);
                    if (!imageDescription.Contains("afstFoDT"))
                    {
                        imageDescription += ",afstFoDT"; // 파일이름에서 날짜를 추론한 태그
                    }
                }
                else
                {
                    Console.WriteLine("Date not found in folder name.");
                }

                pattern = @"_(\d{8})_(\d{6})";
                match = Regex.Match(fileName, pattern);
                if (match.Success)
                {
                    string dateString = match.Groups[1].Value;
                    string timeString = match.Groups[2].Value;
                    imageMetadata.fileNameDateTime = new DateTime(
                        int.Parse(dateString.Substring(0, 4)),
                        int.Parse(dateString.Substring(4, 2)),
                        int.Parse(dateString.Substring(6, 2)),
                        int.Parse(timeString.Substring(0, 2)),
                        int.Parse(timeString.Substring(2, 2)),
                        int.Parse(timeString.Substring(4, 2)));
                    row = new ListViewItem(0xff02.ToString("X4"));
                    row.SubItems.Add("FileName DateTime");
                    row.SubItems.Add(imageMetadata.fileNameDateTime.ToString());// 파일로서의 생성 시간 (복사하면 갱신됨)
                    row.SubItems.Add("Unedited");
                    m_listViewProperties.Items.Add(row);
                    if (!imageDescription.Contains("afstFiDT"))
                    {
                        imageDescription += ",afstFiDT"; // 파일이름에서 날짜를 추론한 태그
                    }
                }
                else
                {
                    Console.WriteLine("Date not found in file name.");
                }

                /// Get the XMP metadata directory
                var xmpDirectory = ImageMetadataReader.ReadMetadata(m_currImageFileTemp).OfType<XmpDirectory>().FirstOrDefault();
                // Check if the XMP metadata directory is present
                if (xmpDirectory != null)
                {
                    IXmpPropertyInfo xmpDateAcquiredPropertyItem = xmpDirectory.XmpMeta.Properties.FirstOrDefault(p => p.Path == "MicrosoftPhoto:DateAcquired");
                    if (xmpDateAcquiredPropertyItem != null)
                    {
                        // Get the value of the 'MICROSOFTPHOTO:DATEACQUIRED' property
                        ;
                        DateTime.TryParse(xmpDateAcquiredPropertyItem.Value, out DateTime tempDate);
                        imageMetadata.xmpDateAcquiredTime = tempDate;
                        row = new ListViewItem(0xE001.ToString("X4"));
                        row.SubItems.Add("XMP DateAcquired Time");
                        row.SubItems.Add(imageMetadata.xmpDateAcquiredTime.ToString());// 원본파일 생성시간. 일반적인 exif 정보가 아니라서 읽어서 비교용으로만 쓴다.
                        if (imageMetadata.xmpDateAcquiredTime.Year < 1970 || imageMetadata.xmpDateAcquiredTime == null)
                        {
                            row.SubItems.Add("No DateAcquiredTime");
                            if (imageDescription != null && !imageDescription.Contains("NoDtAT"))
                            {
                                imageDescription += ",NoDtAT";
                            }
                        }
                        else
                        {
                            row.SubItems.Add("Unedited");
                        }
                        m_listViewProperties.Items.Add(row);
                    }
                    else
                    {
                        Console.WriteLine("The 'MICROSOFTPHOTO:DATEACQUIRED' property was not found in the XMP metadata.");
                        if (imageDescription != null && !imageDescription.Contains("NoDtAT"))
                        {
                            imageDescription += ",NoDtAT";
                        }
                    }
                }
                else
                {
                    Console.WriteLine("XMP metadata not found in the image file.");
                    if (imageDescription != null && !imageDescription.Contains("NoXMP"))
                    {
                        imageDescription += ",NoXMP";
                    }
                }

                row = new ListViewItem(0xf001.ToString("X4"));
                row.SubItems.Add("File CreationTime");
                imageMetadata.fileCreationTime = fileInfo.CreationTime;
                row.SubItems.Add(imageMetadata.fileCreationTime.ToString());// 파일로서의 생성 시간 (복사하면 갱신됨)
                if (imageMetadata.fileCreationTime.Year < 1970 || imageMetadata.fileCreationTime == null)
                {
                    row.SubItems.Add("No fileCreationTime");
                    if (imageDescription != null && !imageDescription.Contains("NofCT"))
                    {
                        imageDescription += ",NofCT";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(0xf002.ToString("X4"));
                row.SubItems.Add("File LastWriteTime");
                imageMetadata.fileLastWriteTime = fileInfo.LastWriteTime;
                row.SubItems.Add(imageMetadata.fileLastWriteTime.ToString()); // 파일로서의 마지막 수정시간
                if (imageMetadata.fileLastWriteTime.Year < 1970 || imageMetadata.fileLastWriteTime == null)
                {
                    row.SubItems.Add("No fileLastWriteTime");
                    if (imageDescription != null && !imageDescription.Contains("NofLWT"))
                    {
                        imageDescription += ",NofLWT";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(0xf003.ToString("X4"));
                row.SubItems.Add("File LastAccessTime");
                imageMetadata.fileLastAccessTime = fileInfo.LastAccessTime;
                row.SubItems.Add(imageMetadata.fileLastAccessTime.ToString()); // 파일로서의 마지막 접근시간
                if (imageMetadata.fileLastAccessTime.Year < 1970 || imageMetadata.fileLastAccessTime == null)
                {
                    row.SubItems.Add("No fileLastAccessTime");
                    if (imageDescription != null && !imageDescription.Contains("NofLAT"))
                    {
                        imageDescription += ",NofLAT";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.makeTag).ToString("X4"));
                row.SubItems.Add("Make");
                row.SubItems.Add(exifImage.Make);
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.modelTag).ToString("X4"));
                row.SubItems.Add("Model");
                row.SubItems.Add(exifImage.Model);
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.imageWidthTag).ToString("X4"));
                row.SubItems.Add("Image Width");
                row.SubItems.Add(exifImage.ImageWidth.ToString());
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.imageLengthTag).ToString("X4"));
                row.SubItems.Add("Image Length");
                row.SubItems.Add(exifImage.ImageLength.ToString());
                row.SubItems.Add("Unedited");
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.dateTimeTag).ToString("X4"));
                row.SubItems.Add("DateTime");
                imageMetadata.exifDateTime = exifImage.DateTime;
                row.SubItems.Add(imageMetadata.exifDateTime.ToString()); // 이미지 생성 시간(찍은 시간)
                if (imageMetadata.exifDateTime.Year < 1970 || imageMetadata.exifDateTime == null)
                {
                    row.SubItems.Add("No DateTime");
                    if (imageDescription != null && !imageDescription.Contains("NoDT"))
                    {
                        imageDescription += ",NoDT";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.dateTimeOriginalTag).ToString("X4"));
                row.SubItems.Add("DateTimeOriginal");
                imageMetadata.exifDateTimeOriginal = exifImage.DateTimeOriginal;
                row.SubItems.Add(imageMetadata.exifDateTimeOriginal.ToString());
                if (imageMetadata.exifDateTimeOriginal.Year < 1970 || imageMetadata.exifDateTimeOriginal == null)
                {
                    row.SubItems.Add("No DateTimeOriginal");
                    if (imageDescription != null && !imageDescription.Contains("NoDtTO"))
                    {
                        imageDescription += ",NoDtTO";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.dateTimeDigitizedTag).ToString("X4"));
                row.SubItems.Add("DateTimeDigitized");
                imageMetadata.exifDateTimeDigitized = exifImage.DateTimeDigitized;
                row.SubItems.Add(imageMetadata.exifDateTimeDigitized.ToString());
                if (imageMetadata.exifDateTimeDigitized.Year < 1970 || imageMetadata.exifDateTimeDigitized == null)
                {
                    row.SubItems.Add("No DateTimeDigitized");
                    if (imageDescription != null && !imageDescription.Contains("NoDtTD"))
                    {
                        imageDescription += ",NoDtTD";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.longitudeTag).ToString("X4"));
                row.SubItems.Add("Longitude");
                row.SubItems.Add(exifImage.GPSInfo.Longitude.ToString());
                if (exifImage.GPSInfo.Longitude == 0)
                {
                    row.SubItems.Add("No GPS");
                    if (imageDescription != null && !imageDescription.Contains("NoGPS"))
                    {
                        imageDescription += ",NoGPS";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(0xf004.ToString("X4"));
                row.SubItems.Add("Earliest Date");
                if (imageMetadata.FindEarliestDate())
                {
                    row.SubItems.Add(imageMetadata.earliestDate.ToString());
                    row.SubItems.Add("Updated");
                }
                else
                {
                    row.SubItems.Add(imageMetadata.earliestDate.ToString());
                    row.SubItems.Add("Not Process");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(0x010c.ToString("X4"));
                row.SubItems.Add("Edited ImageDescription");
                row.SubItems.Add(imageDescription);
                row.SubItems.Add("-");
                m_listViewProperties.Items.Add(row);
                if (imageDescription != null)
                {
                    if (!imageDescription.Equals(imageMetadata.imageDescriptionTemp))
                    {
                        imageMetadata.flagValue = true;
                    }
                }

                return row;
            }
            catch (Exception excep)
            {
                MessageBox.Show("Error LoadMetaData: " + excep.Message + m_currImageFile);
                throw;
            }
        }

        private void LoadMetaData(int index, ImageMetadata imageMetadata, PropertyItem[] propItems,
                                  FileInfo fileInfo, ExifImageProperties exifImage, ref string imageDescription)
        {
            try
            {
                Encoding _Encoding = Encoding.UTF8;
                // exifImage.ImageDescription는 읽는 과정에서 이미 한글이 깨져서 쓸 수 없다.
                // Image.Property 에서 읽어와서 다시 인코딩.
                PropertyItem propertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.imageDescriptionTag);
                if (propertyItem != null)
                {
                    imageDescription = _Encoding.GetString(propertyItem.Value).TrimEnd('\0');
                }
                // 기존 imageDescriptionTemp 내용을 백업.
                imageMetadata.imageDescriptionTemp = imageDescription;

                // Part of reading time information.
                // The date inferred from the folder name, file name
                //   folderNameDateTime uses only year, month, and day.
                //   Date difference within 15 days based on folderNameDateTime is ignored.
                string pattern = @"\d{8}";
                Match match = Regex.Match(imageMetadata.fileName, pattern);
                if (match.Success)
                {
                    string dateString = match.Value;
                    imageMetadata.folderNameDateTime = DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
                    if (!imageDescription.Contains("afstFoDT"))
                    {
                        imageDescription += ",afstFoDT"; // 파일이름에서 날짜를 추론한 태그
                    }
                }
                else
                {
                    Console.WriteLine("Date not found in folder name.");
                }

                pattern = @"_(\d{8})_(\d{6})";
                match = Regex.Match(imageMetadata.fileName, pattern);
                if (match.Success)
                {
                    string dateString = match.Groups[1].Value;
                    string timeString = match.Groups[2].Value;
                    imageMetadata.fileNameDateTime = new DateTime(
                        int.Parse(dateString.Substring(0, 4)),
                        int.Parse(dateString.Substring(4, 2)),
                        int.Parse(dateString.Substring(6, 2)),
                        int.Parse(timeString.Substring(0, 2)),
                        int.Parse(timeString.Substring(2, 2)),
                        int.Parse(timeString.Substring(4, 2)));
                    if (!imageDescription.Contains("afstFiDT"))
                    {
                        imageDescription += ",afstFiDT"; // 파일이름에서 날짜를 추론한 태그
                    }
                }
                else
                {
                    Console.WriteLine("Date not found in file name.");
                }

                imageMetadata.fileCreationTime = fileInfo.CreationTime;
                if (imageMetadata.fileCreationTime.Year < 1970 || imageMetadata.fileCreationTime == null)
                {
                    if (imageDescription != null && !imageDescription.Contains("NofCT"))
                    {
                        imageDescription += ",NofCT";
                    }
                }

                imageMetadata.fileLastWriteTime = fileInfo.LastWriteTime;
                if (imageMetadata.fileLastWriteTime.Year < 1970 || imageMetadata.fileLastWriteTime == null)
                {
                    if (imageDescription != null && !imageDescription.Contains("NofLWT"))
                    {
                        imageDescription += ",NofLWT";
                    }
                }

                imageMetadata.fileLastAccessTime = fileInfo.LastAccessTime;
                if (imageMetadata.fileLastAccessTime.Year < 1970 || imageMetadata.fileLastAccessTime == null)
                {
                    if (imageDescription != null && !imageDescription.Contains("NofLAT"))
                    {
                        imageDescription += ",NofLAT";
                    }
                }

                imageMetadata.exifDateTime = exifImage.DateTime;
                if (imageMetadata.exifDateTime.Year < 1970 || imageMetadata.exifDateTime == null)
                {
                    if (imageDescription != null && !imageDescription.Contains("NoDT"))
                    {
                        imageDescription += ",NoDT";
                    }
                }

                imageMetadata.exifDateTimeOriginal = exifImage.DateTimeOriginal;
                if (imageMetadata.exifDateTimeOriginal.Year < 1970 || imageMetadata.exifDateTimeOriginal == null)
                {
                    if (imageDescription != null && !imageDescription.Contains("NoDtTO"))
                    {
                        imageDescription += ",NoDtTO";
                    }
                }

                imageMetadata.exifDateTimeDigitized = exifImage.DateTimeDigitized;
                if (imageMetadata.exifDateTimeDigitized.Year < 1970 || imageMetadata.exifDateTimeDigitized == null)
                {
                    if (imageDescription != null && !imageDescription.Contains("NoDtTD"))
                    {
                        imageDescription += ",NoDtTD";
                    }
                }

                if (exifImage.GPSInfo.Longitude == 0)
                {
                    if (imageDescription != null && !imageDescription.Contains("NoGPS"))
                    {
                        imageDescription += ",NoGPS";
                    }
                }

                imageMetadata.FindEarliestDate();
                if (imageDescription != null)
                {
                    if (!imageDescription.Equals(imageMetadata.imageDescriptionTemp))
                    {
                        imageMetadata.flagValue = true;
                    }
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show("Error LoadMetaData: " + excep.Message + fileInfo.FullName);
                throw;
            }
        }

        private void SaveMetadataImage(string currImageFile, Image images, ImageMetadata imageMetadata,
                                       string imageDescription, ref MetadataModifyLog jsonLog)
        {
            Encoding _Encoding = Encoding.UTF8;
            PropertyItem[] propItems = images.PropertyItems;
            var dateTimeBytes = _Encoding.GetBytes(imageMetadata.earliestDate.ToString("yyyy:MM:dd HH:mm:ss\0"));
            try
            {
                jsonLog.FolderName = imageMetadata.folderName;
                jsonLog.FileName = imageMetadata.fileName;
                jsonLog.FilenameDateTime = imageMetadata.fileNameDateTime;
                jsonLog.FoldernameDateTime = imageMetadata.folderNameDateTime;
                jsonLog.EarliestDate = imageMetadata.earliestDate;
                jsonLog.DateAcquired = imageMetadata.xmpDateAcquiredTime;

                DateTime previousDateTime = imageMetadata.exifDateTime;
                if (imageMetadata.exifDateTime != imageMetadata.earliestDate)
                {
                    PropertyItem dateTimePropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.dateTimeTag);
                    if (dateTimePropertyItem != null)
                    {
                        var temp = Encoding.UTF8.GetString(dateTimePropertyItem.Value).TrimEnd('\0');
                        // DateTime.TryParse(temp, out previousDateTime) 구문으로 파싱되지 않는데 이유를 모르겠다...
                        DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out previousDateTime);
                    }
                    else
                    {
                        dateTimePropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                        dateTimePropertyItem.Id = (int)ExifTags.dateTimeTag;
                        dateTimePropertyItem.Type = 2;
                        dateTimePropertyItem.Len = 20;
                    }
                    dateTimePropertyItem.Value = dateTimeBytes;
                    images.SetPropertyItem(dateTimePropertyItem);
                }
                jsonLog.BeforeDateTime = previousDateTime;
                jsonLog.AfterDateTime = imageMetadata.earliestDate;

                DateTime previousDateTimeOriginal = imageMetadata.exifDateTimeOriginal;
                if (imageMetadata.exifDateTimeOriginal != imageMetadata.earliestDate)
                {
                    PropertyItem dateTimeOriginalPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.dateTimeOriginalTag);
                    if (dateTimeOriginalPropertyItem != null)
                    {
                        var temp = Encoding.UTF8.GetString(dateTimeOriginalPropertyItem.Value).TrimEnd('\0');
                        DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out previousDateTimeOriginal);
                    }
                    else
                    {
                        dateTimeOriginalPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                        dateTimeOriginalPropertyItem.Id = (int)ExifTags.dateTimeOriginalTag;
                        dateTimeOriginalPropertyItem.Type = 2;
                        dateTimeOriginalPropertyItem.Len = 20;
                    }
                    dateTimeOriginalPropertyItem.Value = dateTimeBytes;
                    images.SetPropertyItem(dateTimeOriginalPropertyItem);
                }
                jsonLog.BeforeDateTimeOriginal = previousDateTimeOriginal;
                jsonLog.AfterDateTimeOriginal = imageMetadata.earliestDate;

                DateTime previousDateTimeDigitized = imageMetadata.exifDateTimeDigitized;
                if (imageMetadata.exifDateTimeDigitized != imageMetadata.earliestDate)
                {
                    PropertyItem dateTimeDigitizedPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.dateTimeDigitizedTag);
                    if (dateTimeDigitizedPropertyItem != null)
                    {
                        var temp = Encoding.UTF8.GetString(dateTimeDigitizedPropertyItem.Value).TrimEnd('\0');
                        DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out previousDateTimeDigitized);
                    }
                    else
                    {
                        dateTimeDigitizedPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                        dateTimeDigitizedPropertyItem.Id = (int)ExifTags.dateTimeDigitizedTag;
                        dateTimeDigitizedPropertyItem.Type = 2;
                        dateTimeDigitizedPropertyItem.Len = 20;
                    }
                    dateTimeDigitizedPropertyItem.Value = dateTimeBytes;
                    images.SetPropertyItem(dateTimeDigitizedPropertyItem);
                }
                jsonLog.BeforeDateTimeDigitized = previousDateTimeDigitized;
                jsonLog.AfterDateTimeDigitized = imageMetadata.earliestDate;

                string previousImageDescription = imageMetadata.imageDescriptionTemp;
                if (imageDescription == null)
                {
                    // No change
                }
                else if (imageMetadata.imageDescriptionTemp == null || !imageDescription.Equals(imageMetadata.imageDescriptionTemp))
                {
                    PropertyItem imageDescriptionPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)ExifTags.imageDescriptionTag);
                    var imageDescriptionVal = _Encoding.GetBytes(imageDescription.ToString() + "\0");
                    if (imageDescriptionPropertyItem != null)
                    {
                        // previousImageDescription = _Encoding.GetString(imageDescriptionPropertyItem.Value).TrimEnd('\0');
                    }
                    else
                    {
                        imageDescriptionPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                        imageDescriptionPropertyItem.Id = (int)ExifTags.imageDescriptionTag;
                        imageDescriptionPropertyItem.Type = 2;
                        imageDescriptionPropertyItem.Len = imageDescription.Length + 1;
                    }
                    imageDescriptionPropertyItem.Value = imageDescriptionVal;
                    images.SetPropertyItem(imageDescriptionPropertyItem);

                }
                jsonLog.BeforeImageDescription = previousImageDescription;
                jsonLog.AfterImageDescription = imageDescription;

                //update fileinfo property
                jsonLog.BeforeCreationTime = imageMetadata.fileCreationTime;
                jsonLog.AfterCreationTime = imageMetadata.earliestDate;
                jsonLog.BeforeLastWriteTime = imageMetadata.fileLastWriteTime;
                jsonLog.AfterLastWriteTime = imageMetadata.earliestDate;
                jsonLog.BeforeLastAccessTime = imageMetadata.fileLastAccessTime;
                jsonLog.AfterLastAccessTime = imageMetadata.earliestDate;

                bool fileSaved = false;
                while (!fileSaved)
                {
                    try
                    {
                        images.Save(currImageFile, ImageFormat.Jpeg);// xmpDirectory metadata가 유실된다.
                        fileSaved = true;
                    }
                    catch (IOException)
                    {
                        // Wait for a moment and try again
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show("Error SaveMetadata: " + excep.Message);
            }
        }

        private void SaveFileInfo(string currImageFile, FileStream fileStream, FileInfo fileInfo, ImageMetadata imageMetadata)
        {
            try
            {
                if (imageMetadata.fileCreationTime != imageMetadata.earliestDate)
                {
                    File.SetCreationTime(currImageFile, imageMetadata.earliestDate);
                }

                if (imageMetadata.fileLastWriteTime != imageMetadata.earliestDate)
                {
                    File.SetLastWriteTime(currImageFile, imageMetadata.earliestDate);
                }

                if (imageMetadata.fileLastAccessTime != imageMetadata.earliestDate)
                {
                    File.SetLastAccessTime(currImageFile, imageMetadata.earliestDate);
                }
                File.SetCreationTime(currImageFile, imageMetadata.earliestDate);
                bool fileUnlocked = false;
                while (!fileUnlocked)
                {
                    try
                    {
                        fileStream.Close();
                        fileUnlocked = true;
                    }
                    catch (IOException)
                    {
                        // Wait for a short time before trying again
                        System.Threading.Thread.Sleep(1);
                    }
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show("Error SaveFileInfo: " + excep.Message + currImageFile);
            }
        }

        public class ListViewColumnSorter : IComparer
        {
            private int columnIndex;
            private SortOrder sortOrder;

            public ListViewColumnSorter(int columnIndex)
            {
                this.columnIndex = columnIndex;
                sortOrder = SortOrder.Ascending;
            }

            public int Compare(object x, object y)
            {
                int xValue, yValue;
                if (Int32.TryParse(((ListViewItem)x).SubItems[columnIndex].Text, out xValue) &&
                    Int32.TryParse(((ListViewItem)y).SubItems[columnIndex].Text, out yValue))
                {
                    int compareResult = xValue - yValue;

                    if (sortOrder == SortOrder.Ascending)
                    {
                        return compareResult;
                    }
                    else if (sortOrder == SortOrder.Descending)
                    {
                        return -compareResult;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
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
                    // start loading images in sequence
                    m_btnUpdateAndJump.Text = "Stop";

                    ShowControls(false);
                    m_pBImage.Image.Dispose();
                    ProcessImages(fileNamesList);
                    isAutoLoadEnabled = false;
                    Console.Beep();

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
                    ShowControls(true);
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

        private async void ProcessImages(List<string> fileNamesList)
        {
            HashSet<string> processedFileNames = new HashSet<string>();
            var tasks = new List<Task>();
            int currKeys = 0;
            var logFileName = "metadataModify_" + DateTime.Now.ToString("yyMMdd_HHmmdd") + ".json";
            var logFilePath = Path.Combine(Path.GetDirectoryName(m_currImageFile), logFileName);
            m_currImageFile = null;
            m_currImageFileTemp = null;

            while (currKeys < fileNamesList.Count)
            {
                try
                {
                    string currImageFile = fileNamesList[currKeys];
                    if (!processedFileNames.Contains(currImageFile))
                    {
                        processedFileNames.Add(currImageFile);
                        tasks.Add(Task.Factory.StartNew(() => LoadAndModifyImage(currImageFile, currKeys++, fileNamesList.Count)));
                        //System.Threading.Interlocked.Increment(ref currKeys);
                    }
                    if (tasks.Count >= Environment.ProcessorCount)
                    {
                        await Task.WhenAll(tasks);
                        tasks.Clear();
                        Console.WriteLine(currKeys + "/" + jsonList.Count);
                    }
                }
                catch (Exception exTask)
                {
                    Console.WriteLine("ProcessImages add tast: " + exTask.Message);
                }
            }
            if (currKeys >= fileNamesList.Count)
            {
                currKeys = fileNamesList.Count - 1;
            }
            if (tasks.Count > 0)
            {
                try
                {
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                    Console.WriteLine(currKeys + "/" + jsonList.Count);
                }
                catch(Exception exTask)
                {
                    Console.WriteLine(exTask.Message);
                }
            }
            string jsonString = JsonConvert.SerializeObject(jsonList);
            File.WriteAllText(logFilePath, jsonString);
            currKeys = 0;
        }

        private void LoadAndModifyImage(string currImageFile, int currIndex, int listCount)
        {
            try
            {
                //lock (_lock)
                {
                    string tempString = Convert.ToString(currIndex + 1) + " / " + listCount;

                    if (m_lblCounter.InvokeRequired)
                    {
                        m_lblCounter.Invoke(method: (MethodInvoker)delegate
                        {
                            m_lblCounter.Text = tempString;
                        });
                    }
                    else
                    {
                        m_lblCounter.Text = tempString;
                    }
                    LoadCurrImage(currImageFile, currIndex);
                }// end of lock
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadAndModifyImage add tast: " + ex.Message);
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
