using System.ComponentModel.DataAnnotations;

namespace photoDateModifier
{
    public enum ExifTags
    {
        imageDescriptionTag = 0x010e,
        makeTag = 0x010f,
        modelTag = 0x0110,
        [Display(Name = "Utils.convertDateTime(property, \"yyyy: MM:dd HH: mm:ss\0\")")]
        dateTimeTag = 0x0132,
        [Display(Name = "Utils.convertDateTime(property, \"yyyy: MM:dd HH: mm:ss\0\")")]
        dateTimeOriginalTag = 0x9003,
        [Display(Name = "Utils.convertDateTime(property, \"yyyy: MM:dd HH: mm:ss\0\")")]
        dateTimeDigitizedTag = 0x9004,
        userCommentTag = 0x9286,
        exifImageWidthTag = 0xa002,
        exifImageHeightTag = 0xa003,
        imageWidthTag = 0x0100,
        imageLengthTag = 0x0101,
        [Display(Name = "(LatitudeRef) Enum.ToObject(typeof(LatitudeRef), BitConverter.ToInt16(property.Value, 0))")]
        latitudeRefTag = 0x0001,
        [Display(Name = "GPSInfo.ExifGpsToFloat(exifProperties.GPSInfo.LatitudeRef.ToString(), property)")]
        latitudeTag = 0x0002,
        [Display(Name = "(LongitudeRef) Enum.ToObject(typeof(LongitudeRef), BitConverter.ToInt16(property.Value, 0))")]
        longitudeRefTag = 0x0003,
        [Display(Name = "GPSInfo.ExifGpsToFloat(exifProperties.GPSInfo.LongitudeRef.ToString(), property)")]
        longitudeTag = 0x0004,
        timeZoneOffsetTag = 0x882a
    }

}
