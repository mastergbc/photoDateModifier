using ExifPhotoReader;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace photoDateModifier
{
    public partial class mainDlg : Form, IDisposable
    {
        ConcurrentDictionary<int, MetadataModifyLog> jsonList;

        private async void ProcessImages(List<string> fileNamesList)
        {
            HashSet<string> processedFileNames = new HashSet<string>();
            jsonList = new ConcurrentDictionary<int, MetadataModifyLog>();
            var tasks = new List<Task>();
            int currKeys = 0;
            var logFileName = "metadataModify_" + DateTime.Now.ToString("yyMMdd_HHmmdd") + ".json";
            var logFilePath = Path.Combine(Path.GetDirectoryName(m_currImageFile), logFileName);

            while (currKeys < fileNamesList.Count)
            {
                try
                {
                    string currImageFile = fileNamesList[currKeys];
                    if (!processedFileNames.Contains(currImageFile))
                    {
                        processedFileNames.Add(currImageFile);
                        tasks.Add(Task.Factory.StartNew(() => LoadAndModifyImage(currImageFile, currKeys++, fileNamesList.Count)));
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
                    Console.WriteLine($"ProcessImages Error[{currKeys}]: {exTask.Message}");
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
                catch (Exception exTask)
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
            }
            catch (Exception ex)
            {
                Console.WriteLine("LoadAndModifyImage add tast: " + ex.Message);
            }
        }

        private void LoadCurrImage()
        {
            if (m_currImageFile == "") MessageBox.Show("LoadCurrImage 오류 : 연결된 이미지가 없습니다.");

            Cursor = Cursors.WaitCursor;
            try
            {
                m_currImageFileTemp = Path.GetTempPath() + "__exifEditor__" + Path.GetRandomFileName();
                File.Copy(m_currImageFile, m_currImageFileTemp, false); // set overwrite to false to preserve file attributes
                m_listViewProperties.Items.Clear();
                if (m_pBImage.Image != null)
                {
                    m_pBImage.Image.Dispose();
                }
                m_pBImage.SizeMode = PictureBoxSizeMode.Zoom;
                m_pBImage.Image = Bitmap.FromFile(m_currImageFileTemp);
                try
                {
                    using (ExifImageProperties exifImage = ExifPhoto.GetExifDataPhoto(m_currImageFileTemp))
                    {
                        string imageDescription = ""; // EXIF attribute value to record referenced tag keywords
                        LoadMetaData(exifImage, ref imageDescription); // Load the metadata for the image file
                    }
                }
                catch (Exception exLoadMeta)
                {
                    MessageBox.Show($"Error LoadCurrImage LoadMetaData[{m_currImageFile}]: {exLoadMeta.Message}");
                }
                m_listViewProperties.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                onResize(null, null);
                ShowControls(true);
                Cursor = Cursors.Default;
            }
            catch (Exception excep)
            {
                MessageBox.Show($"Error LoadCurrImage[{m_currImageFile}]: {excep.Message}");
            }
        }

        private void LoadCurrImage(string currImageFile, int currIndex)
        {
            if (currImageFile == "") MessageBox.Show("LoadCurrImage 오류 : 연결된 이미지가 없습니다.");

            try
            {
                ImageMetadata imageMetadata = new ImageMetadata(currImageFile);
                string currImageFileTemp = Path.GetTempPath() + "__exifEditor__" + Path.GetRandomFileName();
                File.Copy(currImageFile, currImageFileTemp, false); // set overwrite to false to preserve file attributes
                string imageDescription = "";
                using (Image images = Bitmap.FromFile(currImageFileTemp))
                {
                    PropertyItem[] propItems = images.PropertyItems;
                    var exifImage = ExifPhoto.GetExifDataPhoto(images);
                    bool isModifyMetadata = false;
                    isModifyMetadata = LoadMetaData(currImageFile, imageMetadata, exifImage, ref imageDescription); // If succeeds, imageMetadata data is updated.

                    if (isModifyMetadata == true)
                    {
                        //수정사항 있음.
                        var jsonLog = new MetadataModifyLog();
                        try
                        {
                            jsonLog.Number = currIndex;
                            jsonLog.FolderName = imageMetadata.folderName;
                            jsonLog.FileName = imageMetadata.fileName;
                            jsonLog.FilenameDateTime = imageMetadata.fileNameDateTime;
                            jsonLog.FoldernameDateTime = imageMetadata.folderNameDateTime;
                            jsonLog.EarliestDate = imageMetadata.earliestDate;
                            jsonLog.DateAcquired = imageMetadata.xmpDateAcquiredTime;
                            jsonLog.BeforeDateTime = imageMetadata.exifDateTime;
                            jsonLog.BeforeDateTimeOriginal = imageMetadata.exifDateTimeOriginal;
                            jsonLog.BeforeDateTimeDigitized = imageMetadata.exifDateTimeDigitized;
                            jsonLog.BeforeImageDescription = imageMetadata.imageDescriptionTemp;
                            jsonLog.BeforeCreationTime = imageMetadata.fileCreationTime;
                            jsonLog.BeforeLastWriteTime = imageMetadata.fileLastWriteTime;
                            jsonLog.BeforeLastAccessTime = imageMetadata.fileLastAccessTime;
                            jsonLog.AfterCreationTime = imageMetadata.earliestDate;
                            jsonLog.AfterLastWriteTime = imageMetadata.earliestDate;
                            jsonLog.AfterLastAccessTime = imageMetadata.earliestDate;

                            DateTime previousTime = DateTime.MinValue;
                            PropertyItem dateTimePropertyItem = imageMetadata.GetDataOnTag(ExifTags.DateTimeTag, propItems, ref previousTime);
                            if (dateTimePropertyItem != null)
                            {
                                images.SetPropertyItem(dateTimePropertyItem);
                                jsonLog.AfterDateTime = imageMetadata.earliestDate;
                            }
                            PropertyItem dateTimeOriginalPropertyItem = imageMetadata.GetDataOnTag(ExifTags.DateTimeOriginalTag, propItems, ref previousTime);
                            if (dateTimeOriginalPropertyItem != null)
                            {
                                images.SetPropertyItem(dateTimeOriginalPropertyItem);
                                jsonLog.AfterDateTimeOriginal = imageMetadata.earliestDate;
                            }
                            PropertyItem dateTimeDigitizedPropertyItem = imageMetadata.GetDataOnTag(ExifTags.DateTimeDigitizedTag, propItems, ref previousTime);
                            if (dateTimeDigitizedPropertyItem != null)
                            {
                                images.SetPropertyItem(dateTimeDigitizedPropertyItem);
                                jsonLog.AfterDateTimeDigitized = imageMetadata.earliestDate;
                            }
                            string previousImageDescription = imageMetadata.imageDescriptionTemp;
                            PropertyItem imageDescriptionPropertyItem = imageMetadata.GetDataOnTag(ExifTags.ImageDescriptionTag, propItems, ref imageDescription);
                            if (imageDescriptionPropertyItem != null)
                            {
                                images.SetPropertyItem(imageDescriptionPropertyItem);
                                jsonLog.AfterImageDescription = imageDescription;
                            }

                            bool fileSaved = false;
                            while (!fileSaved)
                            {
                                try
                                {
                                    // Saving as Jpeg loses xmpDirectory metadata.
                                    // It should be saved as BMP or PNG, but then the capacity increases, right?
                                    // Is there any solution??
                                    images.Save(currImageFile, ImageFormat.Jpeg);
                                    fileSaved = true;
                                }
                                catch (IOException)
                                {
                                    System.Threading.Thread.Sleep(1); // Wait for a moment and try again
                                }
                            }

                            bool fileInfoSaved = false;
                            while (!fileInfoSaved)
                            {
                                try
                                {
                                    lock (currImageFile)
                                    {
                                        File.SetCreationTime(currImageFile, imageMetadata.earliestDate);
                                        File.SetLastWriteTime(currImageFile, imageMetadata.earliestDate);
                                        File.SetLastAccessTime(currImageFile, imageMetadata.earliestDate);
                                        fileInfoSaved = true;
                                    }
                                }
                                catch (IOException)
                                {
                                    System.Threading.Thread.Sleep(1); // Wait for a moment and try again
                                }
                            }
                        }
                        catch (Exception exprop)
                        {
                            MessageBox.Show($"Error save modified image file[{currImageFile}]: {exprop.Message}");
                        }

                        bool isJsonLogAdded = jsonList.TryAdd(currIndex, jsonLog);
                        jsonList.TryGetValue(currIndex, out MetadataModifyLog metadataModifyLog);
                        Console.WriteLine(currIndex + ":" + jsonLog.FileName + "{0} {1}", isJsonLogAdded, metadataModifyLog.FileName);
                    }
                } // image Auto Disposed
                try
                {
                    File.Delete(currImageFileTemp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting file[{currImageFileTemp}]: {ex.Message}");
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show($"Error LoadCurrImage for modify[{currImageFile}]: {excep.Message}");
            }
        }

        private void LoadMetaData(ExifImageProperties exifImage, ref string imageDescription)
        {
            try
            {
                DateTime creationTime = File.GetCreationTime(m_currImageFile);
                DateTime lastWriteTime = File.GetLastWriteTime(m_currImageFile);
                DateTime lastAccessTime = File.GetLastAccessTime(m_currImageFile);
                ImageMetadata imageMetadata = new ImageMetadata(m_currImageFile);
                ListViewItem row;

                imageDescription = exifImage.ImageDescription; // Back up the string read from exifImage.ImageDescription to imageMetadata.imageDescriptionTemp.
                imageMetadata.imageDescriptionTemp = imageDescription;
                row = new ListViewItem(((int)ExifTags.ImageDescriptionTag).ToString("X4"));
                row.SubItems.Add("ImageDescription");
                row.SubItems.Add(imageDescription);
                m_listViewProperties.Items.Add(row);

                if (imageMetadata.DateTimeValidator(imageMetadata.folderNameDateTime))
                {
                    row = new ListViewItem(((int)ExifTags.FolderNameDateTimeTag).ToString("X4"));
                    row.SubItems.Add("FolderName DateTime");
                    row.SubItems.Add(imageMetadata.folderNameDateTime.ToString());
                    if (imageDescription?.Contains("afstFoDT") != true)
                    {
                        imageDescription += ",afstFoDT";
                    }
                    m_listViewProperties.Items.Add(row);
                }

                if (imageMetadata.DateTimeValidator(imageMetadata.fileNameDateTime))
                {
                    row = new ListViewItem(((int)ExifTags.FileNameDateTimeTag).ToString("X4"));
                    row.SubItems.Add("FileName DateTime");
                    row.SubItems.Add(imageMetadata.fileNameDateTime.ToString());
                    if (imageDescription?.Contains("afstFiDT") != true)
                    {
                        imageDescription += ",afstFiDT";
                    }
                    m_listViewProperties.Items.Add(row);
                }

                if (imageMetadata.DateTimeValidator(imageMetadata.xmpDateAcquiredTime))
                {
                    row = new ListViewItem(((int)ExifTags.XmpDateAcquiredTimeTag).ToString("X4"));
                    row.SubItems.Add("XMP DateAcquired Time");
                    row.SubItems.Add(imageMetadata.xmpDateAcquiredTime.ToString());
                    if (imageDescription?.Contains("XmpDtAT") != true)
                    {
                        imageDescription += ",XmpDtAT";
                    }
                    m_listViewProperties.Items.Add(row);
                }

                row = new ListViewItem(((int)ExifTags.CreationTimeTag).ToString("X4"));
                row.SubItems.Add("File CreationTime");
                row.SubItems.Add(creationTime.ToString());
                if (imageMetadata.DateTimeValidator(creationTime))
                {
                    imageMetadata.fileCreationTime = creationTime;
                }
                else
                {
                    if (imageDescription?.Contains("NofCT") != true)
                    {
                        imageDescription += ",NofCT";
                        row.SubItems.Add("No fileCreationTime");
                    }
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.LastWriteTimeTag).ToString("X4"));
                row.SubItems.Add("File LastWriteTime");
                row.SubItems.Add(lastWriteTime.ToString());
                if (imageMetadata.DateTimeValidator(lastWriteTime))
                {
                    imageMetadata.fileLastWriteTime = lastWriteTime;
                }
                else
                {
                    if (imageDescription?.Contains("NofLWT") != true)
                    {
                        imageDescription += ",NofLWT";
                        row.SubItems.Add("No fileLastWriteTime");
                    }
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.LastAccessTimeTag).ToString("X4"));
                row.SubItems.Add("File LastAccessTime");
                row.SubItems.Add(lastAccessTime.ToString());
                if (imageMetadata.DateTimeValidator(lastAccessTime))
                {
                    imageMetadata.fileLastAccessTime = lastAccessTime;
                }
                else
                {
                    if (imageDescription?.Contains("NofLAT") != true)
                    {
                        imageDescription += ",NofLAT";
                        row.SubItems.Add("No fileLastAccessTime");
                    }
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.MakeTag).ToString("X4"));
                row.SubItems.Add("Make");
                row.SubItems.Add(exifImage.Make);
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.ModelTag).ToString("X4"));
                row.SubItems.Add("Model");
                row.SubItems.Add(exifImage.Model);
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.PixelXDimensionTag).ToString("X4"));
                row.SubItems.Add("Image Width");
                row.SubItems.Add(exifImage.ExifImageWidth.ToString());
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.PixelYDimensionTag).ToString("X4"));
                row.SubItems.Add("Image Length");
                row.SubItems.Add(exifImage.ExifImageHeight.ToString());
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.DateTimeTag).ToString("X4"));
                row.SubItems.Add("DateTime");
                row.SubItems.Add(exifImage.DateTime.ToString());
                if (imageMetadata.DateTimeValidator(exifImage.DateTime))
                {
                    imageMetadata.exifDateTime = exifImage.DateTime;
                }
                else
                {
                    if (imageDescription?.Contains("NoDT") != true)
                    {
                        imageDescription += ",NoDT";
                        row.SubItems.Add("No DateTime");
                    }
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.DateTimeOriginalTag).ToString("X4"));
                row.SubItems.Add("DateTimeOriginal");
                row.SubItems.Add(exifImage.DateTimeOriginal.ToString());
                if (imageMetadata.DateTimeValidator(exifImage.DateTimeOriginal))
                {
                    imageMetadata.exifDateTimeOriginal = exifImage.DateTimeOriginal;
                }
                else
                {
                    if (imageDescription?.Contains("NoDtTO") != true)
                    {
                        imageDescription += ",NoDtTO";
                        row.SubItems.Add("No DateTimeOriginal");
                    }
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.DateTimeDigitizedTag).ToString("X4"));
                row.SubItems.Add("DateTimeDigitized");
                row.SubItems.Add(exifImage.DateTimeDigitized.ToString());
                if (imageMetadata.DateTimeValidator(exifImage.DateTimeDigitized))
                {
                    imageMetadata.exifDateTimeDigitized = exifImage.DateTimeDigitized;
                }
                else
                {
                    if (imageDescription?.Contains("NoDtTD") != true)
                    {
                        imageDescription += ",NoDtTD";
                        row.SubItems.Add("No DateTimeDigitized");
                    }
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.EarliestDateTag).ToString("X4"));
                row.SubItems.Add("Ealiest DateTime");
                bool isEarliestDateValid = imageMetadata.FindEarliestDate();
                row.SubItems.Add(imageMetadata.earliestDate.ToString());
                if (isEarliestDateValid)
                {
                    row.SubItems.Add("Updated");
                }
                else
                {
                    row.SubItems.Add("Not Valid");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.LongitudeTag).ToString("X4"));
                row.SubItems.Add("Longitude");
                row.SubItems.Add(exifImage.GPSInfo.Longitude.ToString());
                if (exifImage.GPSInfo.Longitude == 0)
                {
                    row.SubItems.Add("No GPS");
                    if (imageDescription?.Contains("NoGPS") != true)
                    {
                        imageDescription += ",NoGPS";
                    }
                }
                else
                {
                    row.SubItems.Add("Unedited");
                }
                m_listViewProperties.Items.Add(row);

                row = new ListViewItem(((int)ExifTags.EditedImageDescriptionTag).ToString("X4"));
                row.SubItems.Add("Edited ImageDescription");
                row.SubItems.Add(imageDescription);
                if (imageDescription?.Equals(imageMetadata?.imageDescriptionTemp) ?? false)
                {
                    imageMetadata.flagValue = true;
                }
                else
                {
                    row.SubItems.Add("Updated");
                }
                m_listViewProperties.Items.Add(row);
            }
            catch (Exception excep)
            {
                MessageBox.Show($"Error LoadMetaData[{m_currImageFile}] & ListViewRow[{m_listViewProperties.Items.Count}]: {excep.Message }");
                throw;
            }
        }

        private bool LoadMetaData(string imageFileFullName, ImageMetadata imageMetadata, ExifImageProperties exifImage,
                                  ref string imageDescription)
        {
            try
            {
                DateTime creationTime = File.GetCreationTime(imageFileFullName);
                DateTime lastWriteTime = File.GetLastWriteTime(imageFileFullName);
                DateTime lastAccessTime = File.GetLastAccessTime(imageFileFullName);

                // Back up the string read from exifImage.ImageDescription to imageMetadata.imageDescriptionTemp.
                imageDescription = exifImage.ImageDescription;
                imageDescription?.Trim();
                imageMetadata.imageDescriptionTemp = imageDescription;
                imageMetadata.fileCreationTime = creationTime;
                imageMetadata.fileLastWriteTime = lastWriteTime;
                imageMetadata.fileLastAccessTime = lastAccessTime;
                imageMetadata.exifDateTime = exifImage.DateTime;
                imageMetadata.exifDateTimeOriginal = exifImage.DateTimeOriginal;
                imageMetadata.exifDateTimeDigitized = exifImage.DateTimeDigitized;
                imageMetadata.FindEarliestDate();

                if (imageMetadata.DateTimeValidator(imageMetadata.folderNameDateTime))
                {
                    if (imageDescription?.Contains("afstFoDT") != true)
                    {
                        imageDescription += ",afstFoDT";
                    }
                }
                if (imageMetadata.DateTimeValidator(imageMetadata.fileNameDateTime))
                {
                    if (imageDescription?.Contains("afstFiDT") != true)
                    {
                        imageDescription += ",afstFiDT";
                    }
                }
                if (imageMetadata.DateTimeValidator(imageMetadata.xmpDateAcquiredTime))
                {
                    if (imageDescription?.Contains("XmpDtAT") != true)
                    {
                        imageDescription += ",XmpDtAT";
                    }
                }
                if (!imageMetadata.DateTimeValidator(imageMetadata.fileCreationTime))
                {
                    if (imageDescription?.Contains("NofCT") != true)
                    {
                        imageDescription += ",NofCT";
                    }
                }
                if (!imageMetadata.DateTimeValidator(imageMetadata.fileLastWriteTime))
                {
                    if (imageDescription?.Contains("NofLWT") != true)
                    {
                        imageDescription += ",NofLWT";
                    }
                }
                if (!imageMetadata.DateTimeValidator(imageMetadata.fileLastAccessTime))
                {
                    if (imageDescription?.Contains("NofLAT") != true)
                    {
                        imageDescription += ",NofLAT";
                    }
                }
                if (!imageMetadata.DateTimeValidator(imageMetadata.exifDateTime))
                {
                    if (imageDescription?.Contains("NoDT") != true)
                    {
                        imageDescription += ",NoDT";
                    }
                }
                if (!imageMetadata.DateTimeValidator(imageMetadata.exifDateTimeOriginal))
                {
                    if (imageDescription?.Contains("NoDtTO") != true)
                    {
                        imageDescription += ",NoDtTO";
                    }
                }
                if (!imageMetadata.DateTimeValidator(imageMetadata.exifDateTimeDigitized))
                {
                    if (imageDescription?.Contains("NoDtTD") != true)
                    {
                        imageDescription += ",NoDtTD";
                    }
                }
                if (exifImage.GPSInfo.Longitude == 0)
                {
                    if (imageDescription?.Contains("NoGPS") != true)
                    {
                        imageDescription += ",NoGPS";
                    }
                }
                if (imageDescription?.Equals(imageMetadata?.imageDescriptionTemp) ?? false)
                {
                    imageMetadata.flagValue = true;
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show($"Error LoadMetaData[{imageFileFullName}]: {excep.Message }");
                throw;
            }
            return imageMetadata.flagValue;
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
    }
}
