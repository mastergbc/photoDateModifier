using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExifPhotoReader;
using Exiv2Wrapper;

namespace photoDateModifier
{
    public class Exiv2Wrapper
    {
        private Exiv2Metadata exiv2Metadata = null;

        public void ReadMetadata(string imagePath)
        {
            exiv2Metadata = new Exiv2Metadata(imagePath);
        }

        public Dictionary<string, string> GetAllMetadata()
        {
            return exiv2Metadata.GetAllMetadata();
        }

        public void WriteMetadata(Dictionary<string, string> metadata)
        {
            exiv2Metadata.WriteMetadata(metadata);
        }

        public void ReadExif(string key, out string value)
        {
            value = exiv2Metadata.ReadExif(key);
        }

        public void WriteExif(string key, string value)
        {
            exiv2Metadata.WriteExif(key, value);
        }

        public void ReadIptc(string key, out string value)
        {
            value = exiv2Metadata.ReadIptc(key);
        }

        public void WriteIptc(string key, string value)
        {
            exiv2Metadata.WriteIptc(key, value);
        }

        public void ReadXmp(string key, out string value)
        {
            value = exiv2Metadata.ReadXmp(key);
        }

        public void WriteXmp(string key, string value)
        {
            exiv2Metadata.WriteXmp(key, value);
        }

        public void SaveChanges()
        {
            if (exiv2Metadata != null)
            {
                exiv2Metadata.SaveChanges();
            }
        }
    }
}
