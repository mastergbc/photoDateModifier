﻿using System.ComponentModel;

namespace ExifPhotoReader
{
    public enum Orientation
    {
        Horizontal = 1,
        [Description("Mirror horizontal")]
        MirrorHorizontal = 2,
        [Description("Rotate 180º")]
        Rotate180 = 3,
        [Description("Mirror vertical")]
        MirrorVertical = 4,
        [Description("Mirror horizontal and rotate 270º CW")]
        MirrorHorizontalRotate270 = 5,
        [Description("Rotate 90º CW")]
        Rotate90 = 6,
        [Description("Mirror horizontal and rotate 90º CW")]
        MirrorHorizontalRotate90 = 7,
        [Description("Rotate 270º CW")]
        Rotate270 = 8
    }
}
