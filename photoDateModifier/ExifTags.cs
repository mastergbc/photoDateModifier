﻿namespace photoDateModifier
{
    public enum ExifTags
    {
        ImageWidthTag = 0x0100,
        ImageLengthTag = 0x0101,
        ImageDescriptionTag = 0x010E,
        MakeTag = 0x010F,
        ModelTag = 0x0110,
        OrientationTag = 0x0112,
        XResolutionTag = 0x011A,
        YResolutionTag = 0x011B,
        ResolutionUnitTag = 0x0128,
        SoftwareTag = 0x0131,
        DateTimeTag = 0x0132,
        YCbCrPositioningTag = 0x0213,
        ExifIFDPointerTag = 0x8769,
        GPSInfoIFDPointerTag = 0x8825,
        ExifVersionTag = 0x9000,
        DateTimeOriginalTag = 0x9003,
        DateTimeDigitizedTag = 0x9004,
        ComponentsConfigurationTag = 0x9101,
        CompressedBitsPerPixelTag = 0x9102,
        ShutterSpeedValueTag = 0x9201,
        ApertureValueTag = 0x9202,
        BrightnessValueTag = 0x9203,
        ExposureBiasValueTag = 0x9204,
        MaxApertureValueTag = 0x9205,
        SubjectDistanceTag = 0x9206,
        MeteringModeTag = 0x9207,
        LightSourceTag = 0x9208,
        FlashTag = 0x9209,
        FocalLengthTag = 0x920A,
        SubjectAreaTag = 0x9214,
        MakerNoteTag = 0x927C,
        UserCommentTag = 0x9286,
        SubSecTimeTag = 0x9290,
        SubSecTimeOriginalTag = 0x9291,
        SubSecTimeDigitizedTag = 0x9292,
        FlashpixVersionTag = 0xA000,
        ColorSpaceTag = 0xA001,
        PixelXDimensionTag = 0xA002, //exifImageWidthTag
        PixelYDimensionTag = 0xA003, //exifImageHeightTag
        RelatedSoundFileTag = 0xA004,
        InteroperabilityIFDPointerTag = 0xA005,
        FocalPlaneXResolutionTag = 0xA20E,
        FocalPlaneYResolutionTag = 0xA20F,
        FocalPlaneResolutionUnitTag = 0xA210,
        SensingMethodTag = 0xA217,
        FileSourceTag = 0xA300,
        SceneTypeTag = 0xA301,
        CFAPatternTag = 0xA302,
        CustomRenderedTag = 0xA401,
        ExposureModeTag = 0xA402,
        WhiteBalanceTag = 0xA403,
        DigitalZoomRatioTag = 0xA404,
        FocalLengthIn35mmFilmTag = 0xA405,
        SceneCaptureTypeTag = 0xA406,
        GainControlTag = 0xA407,
        ContrastTag = 0xA408,
        SaturationTag = 0xA409,
        SharpnessTag = 0xA40A,
        DeviceSettingDescriptionTag = 0xA40B,
        SubjectDistanceRangeTag = 0xA40C,
        ImageUniqueIDTag = 0xA420,
        LatitudeRefTag = 0x0001,
        LatitudeTag = 0x0002,
        LongitudeRefTag = 0x0003,
        LongitudeTag = 0x0004,
        TimeZoneOffsetTag = 0x882a,
        FileNameTag = 0xF001,
        FolderNameTag = 0xF002,
        FolderNameDateTimeTag = 0xF101,
        FileNameDateTimeTag = 0xF102,
        EditedImageDescriptionTag = 0xF10E,
        XmpDateAcquiredTimeTag = 0xF103,
        CreationTimeTag = 0xF201,
        LastWriteTimeTag = 0xF202,
        LastAccessTimeTag = 0xF203,
        EarliestDateTag = 0xF301
    }
}
