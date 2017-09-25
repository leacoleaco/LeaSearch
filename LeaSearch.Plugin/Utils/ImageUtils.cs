using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LeaSearch.Plugin.Utils
{
    public class ImageUtils
    {
        public static BitmapImage ByteArrayToBitmapImage(byte[] array)
        {
            using (var ms = new System.IO.MemoryStream(array))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
                return image;
            }
        }

        public static ImageSource GetBitmapImageByPath(string iconPath)
        {
           return new BitmapImage(new Uri(iconPath));
        }
    }
}
