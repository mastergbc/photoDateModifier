using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace photoDateModifier
{
    /// <summary>
    /// Represents the creation log information for an image file.
    /// </summary>
    [Serializable]
    public class MetadataModifyLog
    {
        [JsonProperty(PropertyName = @"Number", Order = 1)]
        /// <summary>
        /// Number refers to the count of files processed in the batch
        /// </summary>
        public int Number { get; set; }
        [JsonProperty(PropertyName = @"FolderName", Order = 2)]
        /// <summary>
        /// FolderName refers to the name of the folder containing the processed images
        /// </summary>
        public string FolderName { get; set; }
        [JsonProperty(PropertyName = @"FileName", Order = 3)]
        /// <summary>
        /// FileName refers to the name of the processed image file
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// A variable to store the time found from the file name.
        /// </summary>
        [JsonProperty(PropertyName = @"FilenameDateTime", Order = 4)]
        public DateTime FilenameDateTime { get; set; }
        /// <summary>
        /// A variable to store the time found from the folder name.
        /// </summary>
        [JsonProperty(PropertyName = @"FoldernameDateTime", Order = 5)]
        public DateTime FoldernameDateTime { get; set; }
        /// <summary>
        /// A variable to store the earliest time.
        /// </summary>
        [JsonProperty(PropertyName = @"EarliestDate", Order = 6)]
        public DateTime EarliestDate { get; set; }
        /// <summary>
        /// The value of the XMP metadata "DateAcquired" property.
        /// A date obtained from the XMP 'MICROSOFTPHOTO:DATEACQUIRED' property, which is not a general value.
        /// Acquisition date and time present only in file information.
        /// </summary>
        [JsonProperty(PropertyName = @"DateAcquired", Order = 7)]
        public DateTime DateAcquired { get; set; }

        [JsonProperty(PropertyName = @"BeforeCreationTime")]
        /// <summary>
        /// BeforeCreationTime refers to the creation time of the file before modification
        /// </summary>
        public DateTime BeforeCreationTime { get; set; }
        [JsonProperty(PropertyName = @"AfterCreationTime")]
        /// <summary>
        /// AfterCreationTime refers to the creation time of the file after modification
        /// </summary>
        public DateTime AfterCreationTime { get; set; }
        [JsonProperty(PropertyName = @"BeforeLastWriteTime")]
        /// <summary>
        /// BeforeLastWriteTime refers to the last write time of the file before modification
        /// </summary>
        public DateTime BeforeLastWriteTime { get; set; }
        [JsonProperty(PropertyName = @"AfterLastWriteTime")]
        /// <summary>
        /// AfterLastWriteTime refers to the last write time of the file after modification
        /// </summary>
        public DateTime AfterLastWriteTime { get; set; }
        [JsonProperty(PropertyName = @"BeforeLastAccessTime")]
        /// <summary>
        /// BeforeLastAccessTime refers to the last access time of the file before modification
        /// </summary>
        public DateTime BeforeLastAccessTime { get; set; }
        [JsonProperty(PropertyName = @"AfterLastAccessTime")]
        /// <summary>
        /// AfterLastAccessTime refers to the last access time of the file after modification
        /// </summary>
        public DateTime AfterLastAccessTime { get; set; }
        [JsonProperty(PropertyName = @"BeforeDateTime")]
        /// <summary>
        /// BeforeDateTime refers to the date time of the image before modification
        /// </summary>
        public DateTime BeforeDateTime { get; set; }
        [JsonProperty(PropertyName = @"AfterDateTime")]
        /// <summary>
        /// AfterDateTime refers to the date time of the image after modification
        /// </summary>
        public DateTime AfterDateTime { get; set; }
        [JsonProperty(PropertyName = @"BeforeDateTimeOriginal")]
        /// <summary>
        /// BeforeDateTimeOriginal refers to the original date time of the image before modification
        /// </summary>
        public DateTime BeforeDateTimeOriginal { get; set; }
        [JsonProperty(PropertyName = @"AfterDateTimeOriginal")]
        /// <summary>
        /// AfterDateTimeOriginal refers to the original date time of the image after modification
        /// </summary>
        public DateTime AfterDateTimeOriginal { get; set; }
        [JsonProperty(PropertyName = @"BeforeDateTimeDigitized")]
        /// <summary>
        /// BeforeDateTimeDigitized refers to the digitized date time of the image before modification
        /// </summary>
        public DateTime BeforeDateTimeDigitized { get; set; }
        [JsonProperty(PropertyName = @"AfterDateTimeDigitized")]
        /// <summary>
        /// AfterDateTimeDigitized refers to the digitized date time of the image after modification
        /// </summary>
        public DateTime AfterDateTimeDigitized { get; set; }
        [JsonProperty(PropertyName = @"BeforeImageDescription")]
        /// <summary>
        /// BeforeImageDescription refers to the image description of the image before modification
        /// </summary>
        public string BeforeImageDescription { get; set; }
        [JsonProperty(PropertyName = @"AfterImageDescription")]
        /// <summary>
        /// AfterImageDescription refers to the image description of the image after modification
        /// </summary>
        public string AfterImageDescription { get; set; }

        public enum indexNo
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
            BeforeModifyingImageDescription,
            AfterModifyingImageDescription
        }

        public MetadataModifyLog()
        {
            Console.WriteLine("MetadataModifyLog object created.");
        }

        ~MetadataModifyLog()
        {
            Console.WriteLine("MetadataModifyLog object destroyed.");
        }

        public static MetadataModifyLog Read(string fileName)
        {
            var jsonData = File.ReadAllText(fileName);
            return JsonConvert.DeserializeObject<MetadataModifyLog>(jsonData);
        }
    }

}
