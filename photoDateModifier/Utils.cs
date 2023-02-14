using System;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace ExifPhotoReader
{
    class Utils
    {
        public static float calcFnumber(PropertyItem property)
        {
            float tempNo = 0;
            try
            {
                if (property.Value.Length >= 4)
                {
                    tempNo = BitConverter.ToInt32(property.Value, 0) / (float)BitConverter.ToInt32(property.Value, 4);
                }
                else
                {
                    // handle the error or provide alternative logic for arrays with insufficient length
                    tempNo = 0;
                }

                return tempNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("calcFnumber Error : " + ex.Message + property.Id + ": " + tempNo);
                throw;
            }
        }

        public static float calcShutterSpeedValue(PropertyItem property)
        {
            float tempNo = 0;
            try
            {
                if (property.Value.Length >= 4)
                {
                    tempNo = (float)Math.Pow(2, Math.Abs(BitConverter.ToInt32(property.Value, 0) / (float)BitConverter.ToInt32(property.Value, 4)));
                }
                else
                {
                    // handle the error or provide alternative logic for arrays with insufficient length
                    tempNo = 0;
                }

                return tempNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("calcShutterSpeedValue Error : " + ex.Message + property.Id + ": " + tempNo);
                throw;
            }
        }

        public static DateTime convertDateTime(PropertyItem property, string format)
        {
            var temp = Encoding.UTF8.GetString(property.Value).TrimEnd('\0');
            DateTime.TryParseExact(temp, "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            return dateTime;
        }

        public static int getNumberValueInt32(PropertyItem property, int position = 0)
        {
            int tempNo = 0;
            try
            {
                if (property.Value.Length >= 4)
                {
                    tempNo = BitConverter.ToInt32(property.Value, position);
                }
                else
                {
                    // handle the error or provide alternative logic for arrays with insufficient length
                    tempNo = 0;
                }

                return tempNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("getNumberValueInt32 Error : " + ex.Message + property.Id + ": " + tempNo);
                throw;
            }
        }

        public static short getNumberValueInt16(PropertyItem property, int position = 0)
        {
            short tempNo = 0;
            try
            {
                if (property.Value.Length >= 2)
                {
                    tempNo = BitConverter.ToInt16(property.Value, position);
                }
                else
                {
                    // handle the error or provide alternative logic for arrays with insufficient length
                    tempNo = 0;
                }

                return tempNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("getNumberValueInt16 Error : " + ex.Message + property.Id + ": " + tempNo);
                throw;
            }
        }

        public static long getNumberValueInt64(PropertyItem property, int position = 0)
        {
            long tempNo = 0;
            try
            {
                if (property.Value.Length >= 8)
                {
                    tempNo = BitConverter.ToInt64(property.Value, position);
                }
                else
                {
                    // handle the error or provide alternative logic for arrays with insufficient length
                    tempNo = 0;
                }

                return tempNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("getNumberValueInt64 Error : " + ex.Message + property.Id + ": " + tempNo);
                throw;
            }
        }

        public static float getNumberValueFloat(PropertyItem property, int position = 0)
        {
            float tempNo = 0;
            try
            {
                if (property.Value.Length >= 2)
                {
                    tempNo = BitConverter.ToInt32(property.Value, position);
                }
                else
                {
                    // handle the error or provide alternative logic for arrays with insufficient length
                    tempNo = 0;
                }

                return tempNo;
            }
            catch (Exception ex)
            {
                MessageBox.Show("getNumberValueFloat Error : " + ex.Message + property.Id + ": " + tempNo);
                throw;
            }
        }

        public static string getStringValue(PropertyItem property)
        {
            string tempString = "";
            try
            {
                tempString = new ASCIIEncoding().GetString(property.Value);

                return tempString;
            }
            catch (Exception ex)
            {
                MessageBox.Show("getStringValue Error : " + ex.Message + property.Id + ": " + tempString);
                throw;
            }
        }
    }
}
