using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace photoDateModifier
{
    /// <summary>
    /// Represents a log that records the metadata modifications made to an image.<br/>
    /// This class is serializable for JSON.
    /// </summary>
    [Serializable]
    public class MetadataModifyLog
    {
        /// <summary>
        /// Number refers to the count of files processed in the batch
        /// </summary>
        [JsonProperty(PropertyName = @"Number", Order = 0)]
        public int Number { get; set; }

        /// <summary>
        /// FolderName refers to the name of the folder containing the processed images
        /// </summary>
        [JsonProperty(PropertyName = @"FolderName", Order = 1)]
        public string FolderName { get; set; }

        /// <summary>
        /// FileName refers to the name of the processed image file
        /// </summary>
        [JsonProperty(PropertyName = @"FileName", Order = 2)]
        public string FileName { get; set; }

        /// <summary>
        /// A variable to store the time found from the file name.
        /// </summary>
        [JsonProperty(PropertyName = @"FilenameDateTime", Order = 3)]
        public DateTime FilenameDateTime { get; set; }
        /// <summary>
        /// A variable to store the time found from the folder name.
        /// </summary>
        [JsonProperty(PropertyName = @"FoldernameDateTime", Order = 4)]
        public DateTime FoldernameDateTime { get; set; }

        /// <summary>
        /// A variable to store the earliest time.
        /// </summary>
        [JsonProperty(PropertyName = @"EarliestDate", Order = 5)]
        public DateTime EarliestDate { get; set; }

        /// <summary>
        /// The value of the XMP metadata "DateAcquired" property.
        /// A date obtained from the XMP 'MICROSOFTPHOTO:DATEACQUIRED' property, which is not a general value.
        /// <para>Acquisition date and time present only in file information.</para>
        /// </summary>
        [JsonProperty(PropertyName = @"DateAcquired", Order = 6)]
        public DateTime DateAcquired { get; set; }


        /// <summary>
        /// BeforeCreationTime refers to the creation time of the file before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeCreationTime")]
        public DateTime BeforeCreationTime { get; set; }

        /// <summary>
        /// AfterCreationTime refers to the creation time of the file after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterCreationTime")]
        public DateTime AfterCreationTime { get; set; }

        /// <summary>
        /// BeforeLastWriteTime refers to the last write time of the file before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeLastWriteTime")]
        public DateTime BeforeLastWriteTime { get; set; }

        /// <summary>
        /// AfterLastWriteTime refers to the last write time of the file after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterLastWriteTime")]
        public DateTime AfterLastWriteTime { get; set; }

        /// <summary>
        /// BeforeLastAccessTime refers to the last access time of the file before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeLastAccessTime")]
        public DateTime BeforeLastAccessTime { get; set; }

        /// <summary>
        /// AfterLastAccessTime refers to the last access time of the file after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterLastAccessTime")]
        public DateTime AfterLastAccessTime { get; set; }

        /// <summary>
        /// BeforeDateTime refers to the date time of the image before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeDateTime")]
        public DateTime BeforeDateTime { get; set; }

        /// <summary>
        /// AfterDateTime refers to the date time of the image after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterDateTime")]
        public DateTime AfterDateTime { get; set; }

        /// <summary>
        /// BeforeDateTimeOriginal refers to the original date time of the image before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeDateTimeOriginal")]
        public DateTime BeforeDateTimeOriginal { get; set; }

        /// <summary>
        /// AfterDateTimeOriginal refers to the original date time of the image after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterDateTimeOriginal")]
        public DateTime AfterDateTimeOriginal { get; set; }

        /// <summary>
        /// BeforeDateTimeDigitized refers to the digitized date time of the image before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeDateTimeDigitized")]
        public DateTime BeforeDateTimeDigitized { get; set; }

        /// <summary>
        /// AfterDateTimeDigitized refers to the digitized date time of the image after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterDateTimeDigitized")]
        public DateTime AfterDateTimeDigitized { get; set; }

        /// <summary>
        /// BeforeImageDescription refers to the image description of the image before modification
        /// </summary>
        [JsonProperty(PropertyName = @"BeforeImageDescription")]
        public string BeforeImageDescription { get; set; }

        /// <summary>
        /// AfterImageDescription refers to the image description of the image after modification
        /// </summary>
        [JsonProperty(PropertyName = @"AfterImageDescription")]
        public string AfterImageDescription { get; set; }

        public enum IndexNo
        {
            Number = 0,
            FolderName,
            FileName,
            FilenameDateTime,
            FoldernameDateTime,
            EarliestDate,
            DateAcquired,
            BeforeCreationTime,
            AfterCreationTime,
            BeforeLastWriteTime,
            AfterLastWriteTime,
            BeforeLastAccessTime,
            AfterLastAccessTime,
            BeforeDateTime,
            AfterDateTime,
            BeforeDateTimeOriginal,
            AfterDateTimeOriginal,
            BeforeDateTimeDigitized,
            AfterDateTimeDigitized,
            BeforeImageDescription,
            AfterImageDescription
        }

        public MetadataModifyLog()
        {
            Number = 0;
            FolderName = "";
            FileName = "";
            FilenameDateTime = DateTime.MinValue;
            FoldernameDateTime = DateTime.MinValue;
            EarliestDate = DateTime.MinValue;
            DateAcquired = DateTime.MinValue;
            BeforeCreationTime = DateTime.MinValue;
            AfterCreationTime = DateTime.MinValue;
            BeforeLastWriteTime = DateTime.MinValue;
            AfterLastWriteTime = DateTime.MinValue;
            BeforeLastAccessTime = DateTime.MinValue;
            AfterLastAccessTime = DateTime.MinValue;
            BeforeDateTime = DateTime.MinValue;
            AfterDateTime = DateTime.MinValue;
            BeforeDateTimeOriginal = DateTime.MinValue;
            AfterDateTimeOriginal = DateTime.MinValue;
            BeforeDateTimeDigitized = DateTime.MinValue;
            AfterDateTimeDigitized = DateTime.MinValue;
            BeforeImageDescription = "";
            AfterImageDescription = "";
            Console.WriteLine("MetadataModifyLog object created.");
        }
    }
}
