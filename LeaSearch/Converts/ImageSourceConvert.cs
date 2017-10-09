using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LeaSearch.Core.Image;
using LeaSearch.Core.Ioc;
using LeaSearch.Plugin.Utils;

namespace LeaSearch.Converts
{
    public class ImageSourceConvert : IValueConverter
    {
        private readonly ImageManager _imageManager = Ioc.Reslove<ImageManager>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageSource = value as ImageSource;
            if (imageSource != null)
            {
                return imageSource;
            }

            var s = value as string;
            if (s != null)
            {
                return _imageManager.GetImageSource(s);
            }

            var stream = value as Stream;
            if (stream != null)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }


            var bytes = value as byte[];
            if (bytes != null)
            {
                return ImageUtils.ByteArrayToBitmapImage(bytes);
            }

            return _imageManager.GetErrorIcon();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}