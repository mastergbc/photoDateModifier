using ExifPhotoReader.Types;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace ExifPhotoReader
{
    public class ExifPhoto
    {
        public static ExifImageProperties GetExifDataPhoto(string path)
        {
            Image file = new Bitmap(path);
            PropertyItem[] propItems;
            ExifImageProperties exifProperties = new ExifImageProperties(); ;
            try
            {
                propItems = file.PropertyItems;
                exifProperties.GPSInfo = new GPSInfo();

                foreach (PropertyItem item in propItems)
                {
                    Convert(item, exifProperties);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetExifDataPhoto path Error: " + ex.Message + path);
                throw;
            }
            return exifProperties;
        }

        public static ExifImageProperties GetExifDataPhoto(Image file)
        {
            PropertyItem[] propItems = file.PropertyItems;
            ExifImageProperties exifProperties = new ExifImageProperties();
            try
            {
                exifProperties.GPSInfo = new GPSInfo();

                foreach (PropertyItem item in propItems)
                {
                    Convert(item, exifProperties);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetExifDataPhoto image Error: " + ex.Message);
                throw;
            }
            return exifProperties;
        }

        private static void Convert(PropertyItem property, ExifImageProperties exifProperties)
        {
            try
            {
                switch (property.Id)
                {
                    case 0x010e:
                        exifProperties.ImageDescription = Utils.GetStringValue(property);
                        break;
                    case 0x010f:
                        exifProperties.Make = Utils.GetStringValue(property);
                        break;
                    case 0x0110:
                        exifProperties.Model = Utils.GetStringValue(property);
                        break;
                    case 0x0112:
                        exifProperties.Orientation = Utils.GetEnumObjectInt16<Orientation>(property);
                        break;
                    case 0x011a:
                        exifProperties.XResolution = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x011b:
                        exifProperties.YResolution = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0128:
                        exifProperties.ResolutionUnit = Utils.GetEnumObjectInt16<ResolutionUnit>(property);
                        break;
                    case 0x0131:
                        exifProperties.Software = Utils.GetStringValue(property);
                        break;
                    case 0x0132:
                        exifProperties.DateTime = Utils.GetConvertedDateTime(property, "yyyy:MM:dd HH:mm:ss\0");
                        break;
                    case 0x013e:
                        exifProperties.WhitePoint = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x013f:
                        exifProperties.PrimaryChromaticities = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0211:
                        exifProperties.YCbCrCoefficients = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0213:
                        exifProperties.YCbCrPositioning = Utils.GetEnumObjectInt16<YCbCrPositioning>(property);
                        break;
                    case 0x0214:
                        exifProperties.ReferenceBlackWhite = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x8298:
                        exifProperties.Copyright = Utils.GetStringValue(property);
                        break;
                    case 0x8769:
                        exifProperties.ExifOffset = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x829a:
                        exifProperties.ExposureTime = Utils.GetCalcFloatingNumber(property);
                        break;
                    case 0x829d:
                        exifProperties.FNumber = Utils.GetCalcFloatingNumber(property);
                        break;
                    case 0x8822:
                        exifProperties.ExposureProgram = Utils.GetEnumObjectInt16<ExposureProgram>(property);
                        break;
                    case 0xa403:
                        exifProperties.WhiteBalance = Utils.GetEnumObjectInt16<WhiteBalance>(property);
                        break;
                    case 0xa402:
                        exifProperties.ExposureMode = Utils.GetEnumObjectInt16<ExposureMode>(property);
                        break;
                    case 0x8827:
                        exifProperties.ISOSpeedRatings = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x9000:
                        exifProperties.ExifVersion = Utils.GetStringValue(property);
                        break;
                    case 0x9003:
                        exifProperties.DateTimeOriginal = Utils.GetConvertedDateTime(property, "yyyy:MM:dd HH:mm:ss\0");
                        break;
                    case 0x9004:
                        exifProperties.DateTimeDigitized = Utils.GetConvertedDateTime(property, "yyyy:MM:dd HH:mm:ss\0");
                        break;
                    case 0x9101:
                        exifProperties.ComponentConfiguration = Utils.GetStringValue(property);
                        break;
                    case 0x9102:
                        exifProperties.CompressedBitsPerPixel = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x9201:
                        exifProperties.ShutterSpeedValue = Utils.GetCalcShutterSpeedValue(property);
                        break;
                    case 0x9202:
                        exifProperties.ApertureValue = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x9203:
                        exifProperties.BrightnessValue = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x9204:
                        exifProperties.ExposureBiasValue = Utils.GetNumberValueFloat(property, 4);
                        break;
                    case 0x9205:
                        exifProperties.MaxApertureValue = Utils.GetCalcFloatingNumber(property);
                        break;
                    case 0x9206:
                        exifProperties.SubjectDistance = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x9207:
                        exifProperties.MeteringMode = Utils.GetEnumObjectInt16<MeteringMode>(property);
                        break;
                    case 0x9208:
                        exifProperties.LightSource = Utils.GetEnumObjectInt16<LightSource>(property);
                        break;
                    case 0x9209:
                        exifProperties.Flash = Utils.GetEnumObjectInt16<Flash>(property);
                        break;
                    case 0x920a:
                        exifProperties.FocalLength = Utils.GetCalcFloatingNumber(property);
                        break;
                    case 0x927c:
                        exifProperties.MakerNote = Utils.GetStringValue(property);
                        break;
                    case 0x9286:
                        exifProperties.UserComment = Utils.GetStringValue(property);
                        break;
                    case 0xa000:
                        exifProperties.FlashPixVersion = Utils.GetStringValue(property);
                        break;
                    case 0xa001:
                        exifProperties.ColorSpace = Utils.GetEnumObjectInt16<ColorSpace>(property);
                        break;
                    case 0xa002:
                        exifProperties.ExifImageWidth = Utils.GetNumberValueInt16(property);
                        break;
                    case 0xa003:
                        exifProperties.ExifImageHeight = Utils.GetNumberValueInt16(property);
                        break;
                    case 0xa004:
                        exifProperties.RelatedSoundFile = Utils.GetStringValue(property);
                        break;
                    case 0xa005:
                        exifProperties.ExifInteroperabilityOffset = Utils.GetNumberValueInt64(property);
                        break;
                    case 0xa20e:
                        exifProperties.FocalPlaneXResolution = Utils.GetNumberValueInt32(property);
                        break;
                    case 0xa20f:
                        exifProperties.FocalPlaneYResolution = Utils.GetNumberValueInt32(property);
                        break;
                    case 0xa210:
                        exifProperties.FocalPlaneResolutionUnit = Utils.GetEnumObjectInt16<FocalPlaneResolutionUnit>(property);
                        break;
                    case 0xa217:
                        exifProperties.SensingMethod = Utils.GetEnumObjectInt16<SensingMethod>(property);
                        break;
                    case 0xa300:
                        exifProperties.FileSource = Utils.GetEnumObjectInt32<FileSource>(property);
                        break;
                    case 0xa301:
                        exifProperties.SceneType = Utils.GetStringValue(property);
                        break;
                    case 0x0100:
                        exifProperties.ImageWidth = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0101:
                        exifProperties.ImageLength = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0102:
                        exifProperties.BitsPerSample = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0103:
                        exifProperties.Compression = Utils.GetEnumObjectInt16<Compression>(property);
                        break;
                    case 0x0106:
                        exifProperties.PhotometricInterpretation = Utils.GetEnumObjectInt16<PhotometricInterpretation>(property);
                        break;
                    case 0x0111:
                        exifProperties.StripOffsets = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0115:
                        exifProperties.SamplesPerPixel = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0116:
                        exifProperties.RowsPerStrip = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0117:
                        exifProperties.StripByteConunts = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x011c:
                        exifProperties.PlanarConfiguration = Utils.GetEnumObjectInt16<PlanarConfiguration>(property);
                        break;
                    case 0x0212:
                        exifProperties.YCbCrSubSampling = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x00fe:
                        exifProperties.NewSubfileType = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x00ff:
                        exifProperties.SubfileType = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x012d:
                        exifProperties.TransferFunction = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x013b:
                        exifProperties.Artist = Utils.GetStringValue(property);
                        break;
                    case 0x013d:
                        exifProperties.Predictor = Utils.GetEnumObjectInt16<Predictor>(property);
                        break;
                    case 0x0142:
                        exifProperties.TileWidth = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0143:
                        exifProperties.TileLength = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0144:
                        exifProperties.TileOffsets = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x0145:
                        exifProperties.TileByteCounts = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x014a:
                        exifProperties.SubIFDs = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x015b:
                        exifProperties.JPEGTables = Utils.GetStringValue(property);
                        break;
                    case 0x828d:
                        exifProperties.CFARepeatPatternDim = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x828e:
                        exifProperties.CFAPattern = property.Value;
                        break;
                    case 0x828f:
                        exifProperties.BatteryLevel = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x83bb:
                        exifProperties.IPTCNAA = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x8773:
                        exifProperties.InterColorProfile = Utils.GetStringValue(property);
                        break;
                    case 0x8824:
                        exifProperties.SpectralSensitivity = Utils.GetStringValue(property);
                        break;
                    case 0x0000:
                        exifProperties.GPSInfo.VersionID = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0001:
                        exifProperties.GPSInfo.LatitudeRef = Utils.GetEnumObjectInt16<LatitudeRef>(property);
                        break;
                    case 0x0002:
                        exifProperties.GPSInfo.Latitude = GPSInfo.ExifGpsToFloat(exifProperties.GPSInfo.LatitudeRef.ToString(), property);
                        break;
                    case 0x0003:
                        exifProperties.GPSInfo.LongitudeRef = Utils.GetEnumObjectInt16<LongitudeRef>(property);
                        break;
                    case 0x0004:
                        exifProperties.GPSInfo.Longitude = GPSInfo.ExifGpsToFloat(exifProperties.GPSInfo.LongitudeRef.ToString(), property);
                        break;
                    case 0x0005:
                        exifProperties.GPSInfo.AltitudeRef = Utils.GetEnumObjectInt16<AltitudeRef>(property);
                        break;
                    case 0x0006:
                        exifProperties.GPSInfo.Altitude = Utils.GetCalcFloatingNumber(property);
                        break;
                    case 0x0007:
                        //exifProperties.GPSInfo.TimeStamp = DateTime.ParseExact($"{BitConverter.ToInt32(property.Value, 0).ToString().PadLeft(2, '0')}:{BitConverter.ToInt32(property.Value, 8).ToString().PadLeft(2, '0')}:{BitConverter.ToInt32(property.Value, 16).ToString().PadLeft(2, '0')}\0", "HH:mm:ss\0", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case 0x0008:
                        exifProperties.GPSInfo.Satellites = Utils.GetStringValue(property);
                        break;
                    case 0x0009:
                        exifProperties.GPSInfo.Status = Utils.GetEnumObjectInt16<Status>(property);
                        break;
                    case 0x000a:
                        exifProperties.GPSInfo.MeasureMode = Utils.GetStringValue(property);
                        break;
                    case 0x000b:
                        exifProperties.GPSInfo.DOP = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x000c:
                        exifProperties.GPSInfo.SpeedRef = Utils.GetStringValue(property);
                        break;
                    case 0x000d:
                        exifProperties.GPSInfo.Speed = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x000e:
                        exifProperties.GPSInfo.TrackRef = Utils.GetEnumObjectInt16<TrackRef>(property);
                        break;
                    case 0x000f:
                        exifProperties.GPSInfo.Track = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0010:
                        exifProperties.GPSInfo.ImgDirectionRef = Utils.GetEnumObjectInt16<ImgDirectionRef>(property);
                        break;
                    case 0x0011:
                        exifProperties.GPSInfo.ImgDirection = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0012:
                        exifProperties.GPSInfo.MapDatum = Utils.GetStringValue(property);
                        break;
                    case 0x0013:
                        exifProperties.GPSInfo.DestLatitudeRef = Utils.GetStringValue(property);
                        break;
                    case 0x0014:
                        exifProperties.GPSInfo.DestLatitude = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0015:
                        exifProperties.GPSInfo.DestLongitudeRef = Utils.GetEnumObjectInt16<DestLongitudeRef>(property);
                        break;
                    case 0x0016:
                        exifProperties.GPSInfo.DestLongitude = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0017:
                        exifProperties.GPSInfo.DestBearingRef = Utils.GetEnumObjectInt16<DestBearingRef>(property);
                        break;
                    case 0x0018:
                        exifProperties.GPSInfo.DestBearing = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0019:
                        exifProperties.GPSInfo.DestDistanceRef = Utils.GetEnumObjectInt16<DestDistanceRef>(property);
                        break;
                    case 0x001a:
                        exifProperties.GPSInfo.DestDistance = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x001b:
                        exifProperties.GPSInfo.ProcessingMethod = Utils.GetStringValue(property);
                        break;
                    case 0x001c:
                        exifProperties.GPSInfo.AreaInformation = Utils.GetStringValue(property);
                        break;
                    case 0x001d:
                        exifProperties.GPSInfo.DateStamp = Utils.GetConvertedDateTime(property, "yyyy:MM:dd\0");
                        break;
                    case 0x001e:
                        exifProperties.GPSInfo.Differential = Utils.GetEnumObjectInt16<Differential>(property);
                        break;
                    case 0x001f:
                        exifProperties.GPSInfo.HPositioningError = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x8828:
                        exifProperties.OECF = Utils.GetStringValue(property);
                        break;
                    case 0x8829:
                        exifProperties.Interlace = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x882a:
                        exifProperties.TimeZoneOffset = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x882b:
                        exifProperties.SelfTimerMode = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x920b:
                        exifProperties.FlashEnergy = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x920c:
                        exifProperties.SpatialFrequencyResponse = Utils.GetStringValue(property);
                        break;
                    case 0x920d:
                        exifProperties.Noise = Utils.GetStringValue(property);
                        break;
                    case 0x9211:
                        exifProperties.ImageNumber = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x9212:
                        exifProperties.SecurityClassification = Utils.GetEnumObjectString<SecurityClassification>(property);
                        break;
                    case 0x9213:
                        exifProperties.ImageHistory = Utils.GetStringValue(property);
                        break;
                    case 0x9214:
                        exifProperties.SubjectLocation = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x9215:
                        exifProperties.ExposureIndex = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x9216:
                        exifProperties.TIFFEPStandardID = property.Value;
                        break;
                    case 0x9290:
                        exifProperties.SubSecTime = Utils.GetStringValue(property);
                        break;
                    case 0x9291:
                        exifProperties.SubSecTimeOriginal = Utils.GetStringValue(property);
                        break;
                    case 0x9292:
                        exifProperties.SubSecTimeDigitized = Utils.GetStringValue(property);
                        break;
                    case 0xa20b:
                        exifProperties.FlashEnergy = Utils.GetNumberValueInt32(property);
                        break;
                    case 0xa20c:
                        exifProperties.SpatialFrequencyResponse = Utils.GetStringValue(property);
                        break;
                    case 0xa214:
                        exifProperties.SubjectLocation = Utils.GetNumberValueInt16(property);
                        break;
                    case 0xa215:
                        exifProperties.ExposureIndex = Utils.GetNumberValueInt32(property);
                        break;
                    case 0xa302:
                        exifProperties.CFAPattern = property.Value;
                        break;
                    case 0x0200:
                        exifProperties.SpecialMode = Utils.GetNumberValueInt64(property);
                        break;
                    case 0x0201:
                        exifProperties.JpegQual = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0202:
                        exifProperties.Macro = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0203:
                        exifProperties.Unknown = Utils.GetNumberValueInt16(property);
                        break;
                    case 0x0204:
                        exifProperties.DigiZoom = Utils.GetNumberValueInt32(property);
                        break;
                    case 0x0207:
                        exifProperties.SoftwareRelease = Utils.GetStringValue(property);
                        break;
                    case 0x0208:
                        exifProperties.PictInfo = Utils.GetStringValue(property);
                        break;
                    case 0x0209:
                        exifProperties.CameraID = Utils.GetStringValue(property);
                        break;
                    case 0x0f00:
                        exifProperties.DataDump = Utils.GetNumberValueInt64(property);
                        break;
                    case 0xa404:
                        exifProperties.DigitalZoomRatio = Utils.GetNumberValueInt32(property);
                        break;
                    case 0xa405:
                        exifProperties.FocalLengthIn35mmFormat = Utils.GetNumberValueInt16(property);
                        break;
                    case 0xa406:
                        exifProperties.SceneCaptureType = Utils.GetEnumObjectInt16<SceneCaptureType>(property);
                        break;
                    case 0xa407:
                        exifProperties.GainControl = Utils.GetEnumObjectInt16<GainControl>(property);
                        break;
                    case 0xa408:
                        exifProperties.Contrast = Utils.GetEnumObjectInt16<Contrast>(property);
                        break;
                    case 0xa409:
                        exifProperties.Saturation = Utils.GetEnumObjectInt16<Saturation>(property);
                        break;
                    case 0xa40a:
                        exifProperties.Sharpness = Utils.GetEnumObjectInt16<Sharpness>(property);
                        break;
                    case 0xa40c:
                        exifProperties.SubjectDistanceRange = Utils.GetEnumObjectInt16<SubjectDistanceRange>(property);
                        break;
                    case 0xa432:
                        exifProperties.LensInfo = Utils.GetStringValue(property);
                        break;
                    case 0xa433:
                        exifProperties.LensMake = Utils.GetStringValue(property);
                        break;
                    case 0xa434:
                        exifProperties.LensModel = Utils.GetStringValue(property);
                        break;
                    case 0xa435:
                        exifProperties.LensSerialNumber = Utils.GetStringValue(property);
                        break;
                    default:
                        break;
                }
                //return exifProperties;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exifConvert: " + ex.Message + property.Id);
                throw;
            }
        }
    }
}
