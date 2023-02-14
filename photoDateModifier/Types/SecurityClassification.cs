using System.ComponentModel;

namespace ExifPhotoReader
{
    public enum SecurityClassification
    {
        Confidential = 'C',
        Restricted = 'R',
        Secret = 'S',
        [Description("Top Secret")]
        TopSecret = 'T',
        Unclassified = 'U'
    }
}
