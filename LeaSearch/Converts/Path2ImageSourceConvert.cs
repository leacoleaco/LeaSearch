using System;
using System.Globalization;
using System.Windows.Data;
using LeaSearch.Core.Image;
using LeaSearch.Core.Ioc;

namespace LeaSearch.Converts
{
    public class Path2ImageSourceConvert:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            return Ioc.Reslove<ImageManager>().GetImageSource(path);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}