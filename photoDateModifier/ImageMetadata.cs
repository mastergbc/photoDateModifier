using MetadataExtractor;
using MetadataExtractor.Formats.Xmp;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XmpCore;

namespace photoDateModifier
{
    /// <summary>
    ///   A class to store metadata of an image including dates and times from various sources<br/>
    ///   and provide a method to calculate the earliest available date.
    /// <list type="number">
    /// <item><code>
    /// <see cref="ImageMetadata"/> meta = new <see cref="ImageMetadata"/>();<br/>
    /// <see cref="thresholdYear"/> == <see cref="initialThresholdYear"/>
    /// </code></item>
    /// <item><code>
    /// <see cref="ImageMetadata"/> meta = new <see cref="ImageMetadata"/>(<paramref name="inputYear"/>);<br/>
    /// <see cref="thresholdYear"/> == <paramref name="inputYear"/>
    /// </code></item>
    /// <item><code>
    /// <see cref="ImageMetadata"/> meta = new <see cref="ImageMetadata"/>(<paramref name="fileFullName"/>);<br/>
    /// <see cref="fileName"/> and <see cref="folderName"/> are extracted from <paramref name="fileFullName"/> and assigned.<br/>
    /// The specified <paramref name="fileFullName"/> must be non-null and a valid file that exists.
    /// </code></item>
    /// <item><code>
    /// <see cref="ImageMetadata"/> meta = new <see cref="ImageMetadata"/>(<paramref name="inputYear"/>, <paramref name="fileFullName"/>);<br/>
    /// <see cref="thresholdYear"/> == <paramref name="inputYear"/><br/>
    /// <see cref="fileName"/> and <see cref="folderName"/> are extracted from <paramref name="fileFullName"/> and assigned.<br/>
    /// The specified <paramref name="fileFullName"/> must be non-null and a valid file that exists.
    /// </code></item>
    /// </list>
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <param name="fileFullName">The full path of the image file.</param>
    public class ImageMetadata
    {
        private static readonly Dictionary<string, ExifTags> _propertyTagMap = new Dictionary<string, ExifTags>
        {
            { nameof(xmpDateAcquiredTime), ExifTags.XmpDateAcquiredTimeTag },
            { nameof(fileName), ExifTags.FileNameTag },
            { nameof(folderName), ExifTags.FolderNameTag },
            { nameof(folderNameDateTime), ExifTags.FolderNameDateTimeTag },
            { nameof(fileNameDateTime), ExifTags.FileNameDateTimeTag },
            { nameof(fileCreationTime), ExifTags.CreationTimeTag },
            { nameof(fileLastWriteTime), ExifTags.LastWriteTimeTag },
            { nameof(fileLastAccessTime), ExifTags.LastAccessTimeTag },
            { nameof(exifDateTime), ExifTags.DateTimeTag },
            { nameof(exifDateTimeOriginal), ExifTags.DateTimeOriginalTag },
            { nameof(exifDateTimeDigitized), ExifTags.DateTimeDigitizedTag },
            { nameof(earliestDate), ExifTags.EarliestDateTag },
            { nameof(imageDescriptionTemp), ExifTags.ImageDescriptionTag },
        };

        private static readonly Dictionary<ExifTags, Func<ImageMetadata, object>> _propertyGetterMap = new Dictionary<ExifTags, Func<ImageMetadata, object>>
        {
            { ExifTags.XmpDateAcquiredTimeTag, metadata => metadata.xmpDateAcquiredTime },
            { ExifTags.FileNameTag, metadata => metadata.fileName },
            { ExifTags.FolderNameTag, metadata => metadata.folderName },
            { ExifTags.FolderNameDateTimeTag, metadata => metadata.folderNameDateTime },
            { ExifTags.FileNameDateTimeTag, metadata => metadata.fileNameDateTime },
            { ExifTags.CreationTimeTag, metadata => metadata.fileCreationTime },
            { ExifTags.LastWriteTimeTag, metadata => metadata.fileLastWriteTime },
            { ExifTags.LastAccessTimeTag, metadata => metadata.fileLastAccessTime },
            { ExifTags.DateTimeTag, metadata => metadata.exifDateTime },
            { ExifTags.DateTimeOriginalTag, metadata => metadata.exifDateTimeOriginal },
            { ExifTags.DateTimeDigitizedTag, metadata => metadata.exifDateTimeDigitized },
            { ExifTags.EarliestDateTag, metadata => metadata.earliestDate },
            { ExifTags.ImageDescriptionTag, metadata => metadata.imageDescriptionTemp },
        };

        public ExifTags GetTag(string propertyName)
        {
            if (_propertyTagMap.TryGetValue(propertyName, out var tag))
            {
                return tag;
            }
            throw new ArgumentException($"Property '{propertyName}' does not have a corresponding Exif tag.");
        }

        public object GetData(ExifTags tag)
        {
            if (_propertyGetterMap.TryGetValue(tag, out var getter))
            {
                return getter(this);
            }
            throw new ArgumentException($"Tag '{tag}' does not have a corresponding metadata property getter.");
        }

        private int initialThresholdYear = 1940;

        /// <summary>
        /// Gets or sets the earliest effective year.
        /// </summary>
        public int thresholdYear { get; set; }

        /// <summary>
        ///   Gets or sets the value of the XMP metadata "DateAcquired" property.
        /// <para>
        /// <see cref="xmpDateAcquiredTime"/> is obtained from the XMP 'MICROSOFTPHOTO:DATEACQUIRED' property,<br/>
        /// which is not a general value. Acquisition date and time are present only in file information.
        /// </para>
        /// </summary>
        /// <exception cref="MICROSOFTPHOTO:DATEACQUIRED is empty">Thrown when the MICROSOFTPHOTO:DATEACQUIRED property is empty.</exception>
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
        /// The date and time the file was created.<br/>
        /// Usually updated when copied.
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

        /// <summary>
        /// The byte[] type value for image properties
        /// </summary>
        public byte[] earliestDateTimeByte { get; set; }

        /// <summary>
        /// The file name of the image.
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// The folder name containing the image file.
        /// </summary>
        public string folderName { get; set; }

        /// <summary>
        /// A string variable to store the original imageDescription.
        /// </summary>
        public string imageDescriptionTemp { get; set; }

        /// <summary>
        /// A flag variable to check if the metadata has been modified.
        /// </summary>
        public bool isDifferentMetadata { get; set; }

        /// <summary>
        /// A flag variable to check if FileNameDateTime has high priority.
        /// </summary>
        public bool doesFileNameDateTimePriority { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageMetadata"/> class with default threshold year.
        /// </summary>
        public ImageMetadata()
        {
            thresholdYear = initialThresholdYear;
            xmpDateAcquiredTime = DateTime.MinValue;
            fileName = "";
            folderName = "";
            folderNameDateTime = DateTime.MinValue;
            fileNameDateTime = DateTime.MinValue;
            fileCreationTime = DateTime.MinValue;
            fileLastWriteTime = DateTime.MinValue;
            fileLastAccessTime = DateTime.MinValue;
            exifDateTime = DateTime.MinValue;
            exifDateTimeOriginal = DateTime.MinValue;
            exifDateTimeDigitized = DateTime.MinValue;
            earliestDate = DateTime.MaxValue;
            earliestDateTimeByte = null;
            imageDescriptionTemp = "";
            isDifferentMetadata = false;
            doesFileNameDateTimePriority = false;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageMetadata"/> class with <paramref name="fileFullName"/> value.
        /// <para>
        /// The specified <paramref name="fileFullName"/> must be non-null and a valid file that exists.<br/>
        /// <paramref name="thresholdYear"/> is assigned <paramref name="initialThresholdYear"/>.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="thresholdYear">The earliest valid year.</param>
        /// <param name="initialThresholdYear">Initial value for the earliest valid year.</param>
        /// <param name="fileFullName">The full path of the image file.</param>
        public ImageMetadata(string fileFullName)
        {
            if (fileFullName == null)
            {
                throw new ArgumentNullException(nameof(fileFullName));
            }
            if (!File.Exists(fileFullName))
            {
                throw new ArgumentException($"The specified file \"{fileFullName}\" does not exist.");
            }

            thresholdYear = initialThresholdYear;
            GetXmpDateAcquiredTime(fileFullName);
            fileName = Path.GetFileNameWithoutExtension(fileFullName);
            folderName = Path.GetDirectoryName(fileFullName).Split(Path.DirectorySeparatorChar).Last();
            TrySetFolderNameDateTime();
            TrySetFileNameDateTime();
            fileCreationTime = DateTime.MinValue;
            fileLastWriteTime = DateTime.MinValue;
            fileLastAccessTime = DateTime.MinValue;
            exifDateTime = DateTime.MinValue;
            exifDateTimeOriginal = DateTime.MinValue;
            exifDateTimeDigitized = DateTime.MinValue;
            earliestDate = DateTime.MaxValue;
            earliestDateTimeByte = null;
            imageDescriptionTemp = "";
            isDifferentMetadata = false;
            doesFileNameDateTimePriority = false;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageMetadata"/> class with <paramref name="fileFullName"/> value.
        /// <para>
        /// The specified <paramref name="fileFullName"/> must be non-null and a valid file that exists.<br/>
        /// <paramref name="thresholdYear"/> is assigned <paramref name="initialThresholdYear"/>.<br/>
        /// <paramref name="isTrue"/> is assigned <see cref="doesFileNameDateTimePriority"/>.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="thresholdYear">The earliest valid year.</param>
        /// <param name="initialThresholdYear">Initial value for the earliest valid year.</param>
        /// <param name="fileFullName">The full path of the image file.</param>
        /// <param name="isTrue">A checkbox flag to ensure that FileNameDateTime takes precedence.</param>
        public ImageMetadata(string fileFullName, bool isTrue)
        {
            if (fileFullName == null)
            {
                throw new ArgumentNullException(nameof(fileFullName));
            }
            if (!File.Exists(fileFullName))
            {
                throw new ArgumentException($"The specified file \"{fileFullName}\" does not exist.");
            }

            thresholdYear = initialThresholdYear;
            GetXmpDateAcquiredTime(fileFullName);
            fileName = Path.GetFileNameWithoutExtension(fileFullName);
            folderName = Path.GetDirectoryName(fileFullName).Split(Path.DirectorySeparatorChar).Last();
            TrySetFolderNameDateTime();
            TrySetFileNameDateTime();
            fileCreationTime = DateTime.MinValue;
            fileLastWriteTime = DateTime.MinValue;
            fileLastAccessTime = DateTime.MinValue;
            exifDateTime = DateTime.MinValue;
            exifDateTimeOriginal = DateTime.MinValue;
            exifDateTimeDigitized = DateTime.MinValue;
            earliestDate = DateTime.MaxValue;
            earliestDateTimeByte = null;
            imageDescriptionTemp = "";
            isDifferentMetadata = false;
            doesFileNameDateTimePriority = isTrue;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageMetadata"/> class with <paramref name="inputYear"/> value.
        /// <para>
        /// <paramref name="inputYear"/> must be between <paramref name="minYear"/> and <paramref name="maxYear"/>.<br/>
        /// <paramref name="maxYear"/> is today's year.<br/>
        /// <paramref name="thresholdYear"/> is assigned <paramref name="inputYear"/>.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <param name="thresholdYear">The earliest valid year.</param>
        /// <param name="initialThresholdYear">Initial value for the earliest valid year.</param>
        /// <param name="inputYear">The minimum year.</param>
        /// <param name="minYear">The minimum year.</param>
        /// <param name="maxYear">The maximum year.</param>
        public ImageMetadata(int inputYear)
        {
            int minYear = 1800; // set your desired minimum year here
            int maxYear = DateTime.Now.Year; // set the current year as the maximum year

            if (inputYear < minYear || inputYear > maxYear)
            {
                throw new ArgumentException($"Invalid year: {inputYear}. Year must be between {minYear} and {maxYear}.");
            }
            thresholdYear = inputYear;
            xmpDateAcquiredTime = DateTime.MinValue;
            fileName = "";
            folderName = "";
            folderNameDateTime = DateTime.MinValue;
            fileNameDateTime = DateTime.MinValue;
            fileCreationTime = DateTime.MinValue;
            fileLastWriteTime = DateTime.MinValue;
            fileLastAccessTime = DateTime.MinValue;
            exifDateTime = DateTime.MinValue;
            exifDateTimeOriginal = DateTime.MinValue;
            exifDateTimeDigitized = DateTime.MinValue;
            earliestDate = DateTime.MaxValue;
            earliestDateTimeByte = null;
            imageDescriptionTemp = "";
            isDifferentMetadata = false;
            doesFileNameDateTimePriority = false;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ImageMetadata"/> class with <paramref name="inputYear"/> and <paramref name="fileFullName"/> value.
        /// <para>
        /// <paramref name="inputYear"/> must be between <paramref name="minYear"/> and <paramref name="maxYear"/>.<br/>
        /// <paramref name="maxYear"/> is today's year.<br/>
        /// <paramref name="thresholdYear"/> is assigned <paramref name="inputYear"/>.<br/>
        /// The specified <paramref name="fileFullName"/> must be non-null and a valid file that exists.
        /// </para>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="thresholdYear">The earliest valid year.</param>
        /// <param name="initialThresholdYear">Initial value for the earliest valid year.</param>
        /// <param name="inputYear">The minimum year.</param>
        /// <param name="minYear">The minimum year.</param>
        /// <param name="maxYear">The maximum year.</param>
        /// <param name="fileFullName">The full path of the image file.</param>
        public ImageMetadata(int inputYear, string fileFullName)
        {
            int minYear = 1800; // set your desired minimum year here
            int maxYear = DateTime.Now.Year; // set the current year as the maximum year

            if (inputYear < minYear || inputYear > maxYear)
            {
                throw new ArgumentException($"Invalid year: {inputYear}. Year must be between {minYear} and {maxYear}.");
            }
            if (fileFullName == null)
            {
                throw new ArgumentNullException(nameof(fileFullName));
            }
            if (!File.Exists(fileFullName))
            {
                throw new ArgumentException($"The specified file \"{fileFullName}\" does not exist.");
            }

            thresholdYear = initialThresholdYear;
            GetXmpDateAcquiredTime(fileFullName);
            fileName = Path.GetFileNameWithoutExtension(fileFullName);
            folderName = Path.GetDirectoryName(fileFullName).Split(Path.DirectorySeparatorChar).Last();
            TrySetFolderNameDateTime();
            TrySetFileNameDateTime();
            fileCreationTime = DateTime.MinValue;
            fileLastWriteTime = DateTime.MinValue;
            fileLastAccessTime = DateTime.MinValue;
            exifDateTime = DateTime.MinValue;
            exifDateTimeOriginal = DateTime.MinValue;
            exifDateTimeDigitized = DateTime.MinValue;
            earliestDate = DateTime.MaxValue;
            earliestDateTimeByte = null;
            imageDescriptionTemp = "";
            isDifferentMetadata = false;
            doesFileNameDateTimePriority = false;
        }

        /// <summary>
        ///   Attempts to extract a date in the format "yyyyMMdd" from the folder name of the current instance and assign it to the<br/>
        ///   <see cref="folderNameDateTime"/> property.
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> if a date is successfully extracted and assigned to the <see cref="folderNameDateTime"/> property.<br/>
        /// Returns <see langword="false"/> if no date is found in the folder name or if the date cannot be parsed into a valid <see cref="DateTime"/> object.
        /// </returns>
        /// <remarks>
        /// The method uses a regular expression to search for a pattern of 8 consecutive digits in the folder name of the current instance.<br/>
        /// If a match is found, it attempts to parse the matched string into a valid <see cref="DateTime"/> object using the "yyyyMMdd"<br/>
        /// format and assigns the result to the <see cref="folderNameDateTime"/> property.<br/>
        /// If no match is found or the date cannot be parsed, the method returns false and outputs a corresponding message to the console.<br/>
        /// </remarks>
        public bool TrySetFolderNameDateTime()
        {
            string pattern = @"\d{8}";
            Match match = Regex.Match(this.folderName, pattern);
            if (match.Success)
            {
                string dateString = match.Value;
                DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                this.folderNameDateTime = dateTime;
                if (DateTimeValidator(folderNameDateTime))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Parsing fail. Date not found in folder name.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Date not found in folder name.");
            }
            return false;
        }

        /// <summary>
        ///   Tries to parse the date and time information from the file name to set the <see cref="fileNameDateTime"/> property of this <see cref="ImageMetadata"/> instance.
        /// <para>
        /// The date and time information is expected to be in the format "_yyyyMMdd_HHmmss" following the file name.<br/>
        /// If the date and time information is found and successfully parsed, the <see cref="fileNameDateTime"/> property is set to the resulting <see cref="DateTime"/> object.<br/>
        /// If the parsing fails, an error message is printed to the console and the method returns false.
        /// </para>
        /// </summary>
        /// <returns>
        /// Returns <see langword="true"/> if the date and time information is found and successfully parsed,<br/>
        /// and the <see cref="fileNameDateTime"/> property is set to the resulting <see cref="DateTime"/> object. Otherwise, returns <see langword="false"/>.
        /// </returns>
        public bool TrySetFileNameDateTime()
        {
            string pattern = @"_(\d{8})_(\d{6})";
            Match match = Regex.Match(this.fileName, pattern);
            if (match.Success)
            {
                string dateString = match.Groups[1].Value;
                string timeString = match.Groups[2].Value;
                DateTime.TryParseExact(dateString + timeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
                this.fileNameDateTime = dateTime;
                if (DateTimeValidator(fileNameDateTime))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Parsing fail. Date not found in file name.");
                    return false;
                }
            }
            else if (TryConvertToDateTime(this.fileName, out DateTime dateTime))
            {
                this.fileNameDateTime = dateTime;
                if (DateTimeValidator(fileNameDateTime))
                {
                    return true;
                }
                else
                {
                    Console.WriteLine("Parsing fail. Date not found in file name.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Date not found in file name.");
            }
            return false;
        }

        /// <summary>
        /// Tries to convert a string representation of a Unix timestamp to a DateTime value and returns a Boolean indicating whether the conversion succeeded.<br/>
        /// If the conversion succeeds, the output parameter is set to the resulting DateTime value.
        /// </summary>
        /// <param name="input">The string representation of a Unix timestamp</param>
        /// <param name="output">The resulting DateTime value if the conversion succeeded, or DateTime.MinValue if the conversion failed</param>
        /// <returns> <see langword="true"/> if the conversion succeeded, <see langword="false"/> otherwise</returns>
        public static bool TryConvertToDateTime(string input, out DateTime output)
        {
            long timestamp;

            if (input.Length >= 13 && long.TryParse(input, out timestamp))
            {
                // Convert to DateTime
                DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                output = unixEpoch.AddMilliseconds(timestamp);

                // Convert to GMT+9
                output = TimeZoneInfo.ConvertTimeToUtc(output, TimeZoneInfo.Utc).AddHours(9);
                return true;
            }
            else
            {
                output = DateTime.MinValue;
            }
            return false;
        }

        /// <summary>
        ///   Retrieves the date and time that the image was acquired from the XMP metadata of the specified image file.<br/>
        ///   If the 'MICROSOFTPHOTO:DATEACQUIRED' property is not found or is earlier than the threshold year, returns false.
        /// </summary>
        /// <param name="fileFullName">The full path of the image file.</param>
        /// <returns><see langword="true"/> if the date and time were successfully retrieved and are equal to or later than the threshold year, <see langword="false"/> otherwise.</returns>
        public bool GetXmpDateAcquiredTime(string fileFullName)
        {
            var xmpDirectory = ImageMetadataReader.ReadMetadata(fileFullName).OfType<XmpDirectory>().FirstOrDefault();

            if (xmpDirectory != null)
            {
                IXmpPropertyInfo xmpDateAcquiredPropertyItem = xmpDirectory.XmpMeta.Properties.FirstOrDefault(p => p.Path == "MicrosoftPhoto:DateAcquired");

                if (xmpDateAcquiredPropertyItem != null)
                {
                    DateTime.TryParse(xmpDateAcquiredPropertyItem.Value, out DateTime tempDate);
                    xmpDateAcquiredTime = tempDate;
                    if (DateTimeValidator(xmpDateAcquiredTime))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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
            return false;
        }

        /// <summary>
        /// Validates whether the given DateTime is within the specified range and is valid.
        /// </summary>
        /// <param name="dateTime">The DateTime object to validate</param>
        /// <returns><see langword="true"/> if the <paramref name="dateTime"/> is within range and is valid, <see langword="false"/> otherwise</returns>
        /// <exception cref="ArgumentException">Thrown when the dateTime parameter is not a valid DateTime object</exception>
        public bool DateTimeValidator(DateTime dateTime)
        {
            if (dateTime == null || !DateTime.TryParse(dateTime.ToString(), out DateTime result) || result.Year < thresholdYear)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///   Gets the PropertyItem of the specified Exif tag ID and returns the corresponding DateTime value.<br/>
        ///   If the PropertyItem does not exist, a new PropertyItem is created.
        /// </summary>
        /// <param name="tagId">The Exif tag ID of the desired PropertyItem</param>
        /// <param name="propItems">An array of `PropertyItem` objects from which to retrieve the desired `PropertyItem`</param>
        /// <param name="tempDateTime">A `DateTime` value used to create a new `PropertyItem` if the desired `PropertyItem` does not exist</param>
        /// <returns>The `PropertyItem` object with the specified tag ID, or `null` if the desired `PropertyItem` cannot be found</returns>
        /// <exception cref="ArgumentException">Thrown when the specified tag ID is not a valid Exif tag ID</exception>
        public PropertyItem GetDataOnTag(ExifTags tagId, PropertyItem[] propItems, ref DateTime tempDateTime)
        {
            Encoding _Encoding = Encoding.UTF8;
            if (!Enum.IsDefined(typeof(ExifTags), tagId))
            {
                throw new ArgumentException("The specified tag ID is not a valid Exif tag ID.");
            }

            tempDateTime = (DateTime)GetData(tagId);
            if (tempDateTime != earliestDate)
            {
                PropertyItem dateTimePropertyItem = propItems.FirstOrDefault(p => p.Id == (int)tagId);
                if (dateTimePropertyItem != null)
                {
                    var temp = _Encoding.GetString(dateTimePropertyItem.Value).TrimEnd('\0');
                    DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDateTime);
                }
                else
                {
                    dateTimePropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                    dateTimePropertyItem.Id = (int)tagId;
                    dateTimePropertyItem.Type = 2;
                    dateTimePropertyItem.Len = 20;
                }
                var dateTimeBytes = _Encoding.GetBytes(earliestDate.ToString("yyyy:MM:dd HH:mm:ss\0"));
                dateTimePropertyItem.Value = dateTimeBytes;
                return dateTimePropertyItem;
            }

            return null;
        }

        /// <summary>
        ///   Gets the `PropertyItem` of the specified Exif tag ID and returns the corresponding `string` value.<br/>
        ///   If the `PropertyItem` does not exist, a new `PropertyItem` is created.
        /// </summary>
        /// <param name="tagId">The Exif tag ID of the desired `PropertyItem`</param>
        /// <param name="propItems">An array of `PropertyItem` objects from which to retrieve the desired `PropertyItem`</param>
        /// <param name="tempString">A `string` value used to create a new `PropertyItem` if the desired `PropertyItem` does not exist</param>
        /// <returns>The `PropertyItem` object with the specified tag ID, or `null` if the desired `PropertyItem` cannot be found</returns>
        /// <exception cref="ArgumentException">Thrown when the specified tag ID is not a valid Exif tag ID</exception>
        public PropertyItem GetDataOnTag(ExifTags tagId, PropertyItem[] propItems, ref string tempString)
        {
            Encoding _Encoding = Encoding.UTF8;
            if (!Enum.IsDefined(typeof(ExifTags), tagId))
            {
                throw new ArgumentException("The specified tag ID is not a valid Exif tag ID.");
            }

            if (!string.IsNullOrEmpty(tempString) && (imageDescriptionTemp?.Equals(tempString) != true))
            {
                PropertyItem stringPropertyItem = propItems.FirstOrDefault(p => p.Id == (int)tagId);
                var tempStringBytes = _Encoding.GetBytes(tempString.ToString() + "\0");
                if (stringPropertyItem == null)
                {
                    stringPropertyItem = (PropertyItem)Activator.CreateInstance(typeof(PropertyItem), true);
                    stringPropertyItem.Id = (int)tagId;
                    stringPropertyItem.Type = 2;
                    stringPropertyItem.Len = tempString.Length + 1;
                }
                stringPropertyItem.Value = tempStringBytes;
                return stringPropertyItem;
            }

            return null;
        }

        /// <summary>
        ///   Finds the earliest available date from the different metadata sources.<br/>
        ///   After calling FindEarliestDate, <seealso cref="earliestDate"/> will be set to the earliest date from the available sources.
        /// </summary>
        /// <returns>
        ///   <see cref="System.Boolean"/> value indicating whether the earliest date was found.<br/>
        ///   Returns <see langword="true"/> if the earliest date was found, otherwise returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The earliest available date is calculated from the following metadata sources, in order of priority:<br/>
        /// 1. XMP metadata "DateAcquired" property.<br/>
        /// 2. Date and time inferred from the file name.<br/>
        /// 3. File creation time.<br/>
        /// 4. File last write time.<br/>
        /// 5. File last access time.<br/>
        /// 6. Date and time froㅠm the EXIF information in the image file.<br/>
        /// 7. Date and time from the "EXIF DateTimeOriginal" property in the image file.<br/>
        /// 8. Date and time from the "EXIF DateTimeDigitized" property in the image file.<br/>
        /// 9. Date and time inferred from the folder name.<br/>
        /// </remarks>
        public bool FindEarliestDate()
        {

            if (DateTimeValidator(xmpDateAcquiredTime) && xmpDateAcquiredTime.Year > thresholdYear)
            {
                earliestDate = xmpDateAcquiredTime;
                isDifferentMetadata = true;
            }
            if (DateTimeValidator(fileCreationTime) && fileCreationTime < earliestDate)
            {
                earliestDate = fileCreationTime;
                isDifferentMetadata = true;
            }
            if (DateTimeValidator(fileLastWriteTime) && fileLastWriteTime < earliestDate)
            {
                earliestDate = fileLastWriteTime;
                isDifferentMetadata = true;
            }
            if (DateTimeValidator(fileLastAccessTime) && fileLastAccessTime < earliestDate)
            {
                earliestDate = fileLastAccessTime;
                isDifferentMetadata = true;
            }
            //If the picture was taken with a camera and the EXIF information remains, The most high-priority and accurate creation time is DateTime.
            if (DateTimeValidator(exifDateTime) && exifDateTime < earliestDate)
            {
                earliestDate = exifDateTime;
                isDifferentMetadata = true;
            }
            if (DateTimeValidator(exifDateTimeOriginal) && exifDateTimeOriginal < earliestDate)
            {
                earliestDate = exifDateTimeOriginal;
                isDifferentMetadata = true;
            }
            if (DateTimeValidator(exifDateTimeDigitized) && exifDateTimeDigitized < earliestDate)
            {
                earliestDate = exifDateTimeDigitized;
                isDifferentMetadata = true;
            }

            // When fileNameDateTime is a valid value,
            //   if doesFileNameDateTimePriority is true or fileNameDateTime is the earliest time, the value of earliestDate is replaced.
            // Compare folderNameDateTime, the date inferred from the folder name, with earliestDate at the very end.
            //   folderNameDateTime uses only year, month, and day.
            //   Date difference within 15 days based on folderNameDateTime is ignored.
            if (DateTimeValidator(fileNameDateTime))
            {
                if (fileNameDateTime < earliestDate || doesFileNameDateTimePriority)
                {
                    earliestDate = new DateTime(fileNameDateTime.Year, fileNameDateTime.Month, fileNameDateTime.Day,
                    fileNameDateTime.Hour, fileNameDateTime.Minute, fileNameDateTime.Second);
                    isDifferentMetadata = true;
                }
            }
            if (DateTimeValidator(folderNameDateTime) &&  && folderNameDateTime < earliestDate
            && Math.Abs((folderNameDateTime - earliestDate).Days) >= 15)
            {
                earliestDate = new DateTime(folderNameDateTime.Year, folderNameDateTime.Month, folderNameDateTime.Day,
                earliestDate.Hour, earliestDate.Minute, earliestDate.Second);
                isDifferentMetadata = true;
            }

            return isDifferentMetadata;
        }
    }
}
