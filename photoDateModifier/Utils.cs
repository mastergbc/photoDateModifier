using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace ExifPhotoReader
{
    class Utils
    {
        public static T GetEnumObjectString<T>(PropertyItem property)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            T tempObject = default(T);
            try
            {
                if (property.Value != null)
                {
                    tempObject = (T)Enum.ToObject(typeof(T), encoding.GetString(property.Value));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetEnumObjectString Error : {ex.Message} {property.Id}: {tempObject}");
                throw;
            }
            return tempObject;
        }

        public static T GetEnumObjectInt16<T>(PropertyItem property)
        {
            T tempObject = default(T);
            try
            {
                if (property.Value.Length >= 2)
                {
                    tempObject = (T)Enum.ToObject(typeof(T), BitConverter.ToInt16(property.Value, 0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetEnumObjectInt16 Error : {ex.Message} {property.Id}: {tempObject}");
                throw;
            }
            return tempObject;
        }

        public static T GetEnumObjectInt32<T>(PropertyItem property)
        {
            T tempObject = default(T);
            try
            {
                if (property.Value.Length >= 4)
                {
                    tempObject = (T)Enum.ToObject(typeof(T), Int32.Parse(BitConverter.ToString(property.Value)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetEnumObjectInt16 Error : {ex.Message} {property.Id}: {tempObject}");
                throw;
            }
            return tempObject;
        }

        public static float GetCalcFloatingNumber(PropertyItem property)
        {
            float tempNo = 0;
            try
            {
                if (property.Value.Length >= 8)
                {
                    tempNo = BitConverter.ToInt32(property.Value, 0) / (float)BitConverter.ToInt32(property.Value, 4);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetCalcFloatingNumber Error : {ex.Message} {property.Id}: {tempNo}");
                throw;
            }
            return tempNo;
        }

        public static float GetCalcShutterSpeedValue(PropertyItem property)
        {
            float tempNo = 0;
            try
            {
                if (property.Value.Length >= 4)
                {
                    tempNo = (float)Math.Pow(2, Math.Abs(BitConverter.ToInt32(property.Value, 0) / (float)BitConverter.ToInt32(property.Value, 4)));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetCalcShutterSpeedValue Error : {ex.Message} {property.Id}: {tempNo}");
                throw;
            }
            return tempNo;
        }

        public static DateTime GetConvertedDateTime(PropertyItem property, string format)
        {
            var temp = Encoding.UTF8.GetString(property.Value).TrimEnd('\0');
            DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            return dateTime;
        }

        public static int GetNumberValueInt32(PropertyItem property, int position = 0)
        {
            int tempNo = 0;
            try
            {
                if (property.Value.Length >= 4)
                {
                    tempNo = BitConverter.ToInt32(property.Value, position);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetNumberValueInt32 Error : {ex.Message} {property.Id}: {tempNo}");
                throw;
            }
            return tempNo;
        }

        public static short GetNumberValueInt16(PropertyItem property, int position = 0)
        {
            short tempNo = 0;
            try
            {
                if (property.Value.Length >= 2)
                {
                    tempNo = BitConverter.ToInt16(property.Value, position);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetNumberValueInt16 Error : {ex.Message} {property.Id}: {tempNo}");
                throw;
            }
            return tempNo;
        }

        public static long GetNumberValueInt64(PropertyItem property, int position = 0)
        {
            long tempNo = 0;
            try
            {
                if (property.Value.Length >= 8)
                {
                    tempNo = BitConverter.ToInt64(property.Value, position);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetNumberValueInt64 Error : {ex.Message} {property.Id}: {tempNo}");
                throw;
            }
            return tempNo;
        }

        public static float GetNumberValueFloat(PropertyItem property, int position = 0)
        {
            float tempNo = 0;
            try
            {
                if (property.Value.Length >= 2)
                {
                    tempNo = BitConverter.ToInt32(property.Value, position);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetNumberValueFloat Error : {ex.Message} {property.Id}: {tempNo}");
                throw;
            }
            return tempNo;
        }

        public static string GetStringValue(PropertyItem property)
        {
            string tempString = "";
            try
            {
                if (property.Value != null)
                {
                    tempString = new ASCIIEncoding().GetString(property.Value);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"GetStringValue Error : {ex.Message} {property.Id}: {tempString}");
                throw;
            }
            return tempString;
        }
    }
}
