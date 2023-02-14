using System.ComponentModel;

namespace ExifPhotoReader
{
    public enum MeteringMode
    {
        Unknown = 0,
        Average = 1,
        [Description("Center-weighted average")]
        CenterWeightedAverage = 2,
        Spot = 3,
        [Description("Multi-spot")]
        MultiSpot = 4,
        [Description("Multi-segment")]
        MultiSegment = 5,
        Partial = 6,
        Other = 255
    }
}
