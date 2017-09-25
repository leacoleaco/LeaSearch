using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace LeaSearch.Converts
{
    public class Bytes2ImageSourceConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var byteArrayIn = value as byte[];
            if (byteArrayIn == null)
            {
                return null;
            }
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}