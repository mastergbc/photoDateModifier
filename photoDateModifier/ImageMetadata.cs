using System;

namespace photoDateModifier
{
    /// <summary>
    /// Represents the creation time information for an image file.
    /// </summary>
    public class ImageMetadata
    {
        /// <summary>
        /// The value of the XMP metadata "DateAcquired" property.
        /// A date obtained from the XMP 'MICROSOFTPHOTO:DATEACQUIRED' property, which is not a general value.
        /// Acquisition date and time present only in file information.
        /// </summary>
        public DateTime xmpDateAcquiredTime { get; set; }

        /// <summary>
        /// The date extracted from the folder name containing the image file.
        /// </summary>
        public DateTime folderNameDateTime { get; set; }

        /// <summary>
        /// The date extracted from the file name of the image.
        /// </summary>
        public DateTime fileNameDateTime { get; set; }

        /// <summary>
        /// The date and time the file was created.
        /// </summary>
        public DateTime fileCreationTime { get; set; }

        /// <summary>
        /// The date and time the file was last written to.
        /// </summary>
        public DateTime fileLastWriteTime { get; set; }

        /// <summary>
        /// The date and time the file was last accessed.
        /// </summary>
        public DateTime fileLastAccessTime { get; set; }

        /// <summary>
        /// The value of the EXIF metadata "DateTime" property.
        /// </summary>
        public DateTime exifDateTime { get; set; }

        /// <summary>
        /// The value of the EXIF metadata "DateTimeOriginal" property.
        /// </summary>
        public DateTime exifDateTimeOriginal { get; set; }

        /// <summary>
        /// The value of the EXIF metadata "DateTimeDigitized" property.
        /// </summary>
        public DateTime exifDateTimeDigitized { get; set; }

        /// <summary>
        /// The earliest date and time of the image, determined from all available metadata.
        /// </summary>
        public DateTime earliestDate { get; set; }

        public string fileName { get; set; }
        public string folderName { get; set; }
        /// <summary>
        /// A string variable to store the original imageDescription.
        /// </summary>
        public string imageDescriptionTemp { get; set; }

        /// <summary>
        /// A flag variable to check if the metadata has been modified.
        /// </summary>
        public bool flagValue { get; set; }

        public ImageMetadata()
        {
            xmpDateAcquiredTime = DateTime.MinValue;
            folderNameDateTime = DateTime.MinValue;
            fileNameDateTime = DateTime.MinValue;
            fileCreationTime = DateTime.MinValue;
            fileLastWriteTime = DateTime.MinValue;
            fileLastAccessTime = DateTime.MinValue;
            exifDateTime = DateTime.MinValue;
            exifDateTimeOriginal = DateTime.MinValue;
            exifDateTimeDigitized = DateTime.MinValue;
            earliestDate = DateTime.MaxValue;
            fileName = "";
            folderName = "";
            imageDescriptionTemp = "";
            flagValue = false;
        }

        public bool FindEarliestDate()
        {
            if (xmpDateAcquiredTime.Ticks > 0 && xmpDateAcquiredTime.Year > 1970)
            {
                earliestDate = xmpDateAcquiredTime;
                flagValue = true;
            }

            if (fileNameDateTime.Ticks > 0 && fileNameDateTime.Year > 1970 && fileNameDateTime < earliestDate)
            {
                earliestDate = new DateTime(fileNameDateTime.Year, fileNameDateTime.Month, fileNameDateTime.Day, fileNameDateTime.Hour, fileNameDateTime.Minute, fileNameDateTime.Second);
                flagValue = true;
            }

            if (fileCreationTime.Ticks > 0 && fileCreationTime.Year > 1970 && fileCreationTime < earliestDate)
            {
                earliestDate = fileCreationTime;
                flagValue = true;
            }
            if (fileLastWriteTime.Ticks > 0 && fileLastWriteTime.Year > 1970 && fileLastWriteTime < earliestDate)
            {
                earliestDate = fileLastWriteTime;
                flagValue = true;
            }
            if (fileLastAccessTime.Ticks > 0 && fileLastAccessTime.Year > 1970 && fileLastAccessTime < earliestDate)
            {
                earliestDate = fileLastAccessTime;
                flagValue = true;
            }

            //If the picture was taken with a camera and the EXIF information remains, DateTime is the most accurate creation time.
            if (exifDateTime.Ticks > 0 && exifDateTime.Year > 1970 && exifDateTime < earliestDate)
            {
                earliestDate = exifDateTime;
                flagValue = true;
            }
            if (exifDateTimeOriginal.Ticks > 0 && exifDateTimeOriginal.Year > 1970 && exifDateTimeOriginal < earliestDate)
            {
                earliestDate = exifDateTimeOriginal;
                flagValue = true;
            }
            if (exifDateTimeDigitized.Ticks > 0 && exifDateTimeDigitized.Year > 1970 && exifDateTimeDigitized < earliestDate)
            {
                earliestDate = exifDateTimeDigitized;
                flagValue = true;
            }

            // Compare folderNameDateTime, the date inferred from the folder name, with earliestDate at the very end.
            // folderNameDateTime uses only year, month, and day.
            // Date difference within 15 days based on folderNameDateTime is ignored.
            if (folderNameDateTime.Ticks > 0 && folderNameDateTime.Year > 1970
                && Math.Abs((folderNameDateTime - earliestDate).Days) >= 15)
            {
                earliestDate = new DateTime(folderNameDateTime.Year, folderNameDateTime.Month, folderNameDateTime.Day,
                                            earliestDate.Hour, earliestDate.Minute, earliestDate.Second);
                flagValue = true;
            }

            return flagValue;
        }
    }

}
