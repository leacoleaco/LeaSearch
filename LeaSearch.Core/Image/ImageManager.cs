using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LeaSearch.Common.Env;
using LeaSearch.Infrastructure.Logger;
using LeaSearch.Infrastructure.Storage;

namespace LeaSearch.Core.Image
{
    public class ImageManager
    {
        private readonly ImageCache _imageCache = new ImageCache();
        private static BinaryStorage<ConcurrentDictionary<string, int>> _storage;

        private readonly IList<string> _imageExtions = new List<string>()
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".gif",
            ".bmp",
            ".tiff",
            ".ico"
        };

        public void Initialize()
        {
            _storage = new BinaryStorage<ConcurrentDictionary<string, int>>("Image");
            _imageCache.Usage = _storage.TryLoad(new ConcurrentDictionary<string, int>());

            foreach (var icon in new[] { Constant.DefaultIcon, Constant.ErrorIcon })
            {
                ImageSource img = new BitmapImage(new Uri(icon));
                img.Freeze();
                _imageCache[icon] = img;
            }
            Task.Run(() =>
            {
                _imageCache.Usage.AsParallel().Where(i => !_imageCache.ContainsKey(i.Key)).ForAll(i =>
                {
                    var img = GetImageSource(i.Key);
                    if (img != null)
                    {
                        _imageCache[i.Key] = img;
                    }
                });
            });
        }

        public void Save()
        {
            _imageCache.Cleanup();
            _storage.Save(_imageCache.Usage);
        }


        public ImageSource GetImageSource(string path)
        {
            ImageSource image = null;
            if (string.IsNullOrEmpty(path))
            {
                image = _imageCache[Constant.ErrorIcon];
            }
            else if (_imageCache.ContainsKey(path))
            {
                image = _imageCache[path];
            }
            else
            {
                if (path.StartsWith("data:", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("pack:", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        image = new BitmapImage(new Uri(path));
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Read Plugin Image ERR: {e.Message}");
                    }
                }
                else if (Path.IsPathRooted(path))
                {
                    if (Directory.Exists(path))
                    {
                        image = ShellIcon(path);
                    }
                    else if (File.Exists(path))
                    {
                        var externsion = Path.GetExtension(path).ToLower();
                        if (_imageExtions.Contains(externsion))
                        {
                            image = new BitmapImage(new Uri(path));
                        }
                        else
                        {
                            image = ShellIcon(path);
                        }
                    }
                    else
                    {
                        image = _imageCache[Constant.ErrorIcon];
                        path = Constant.ErrorIcon;
                    }
                }
                else
                {
                    var defaultDirectoryPath = Path.Combine(Constant.ProgramDirectory, "Images", Path.GetFileName(path));
                    if (File.Exists(defaultDirectoryPath))
                    {
                        image = new BitmapImage(new Uri(defaultDirectoryPath));
                    }
                    else
                    {
                        image = _imageCache[Constant.ErrorIcon];
                        path = Constant.ErrorIcon;
                    }
                }
                if (image != null)
                {
                    _imageCache[path] = image;
                    image.Freeze();
                }
            }
            return image;
        }

        public ImageSource GetDefaultIcon()
        {
            return _imageCache[Constant.DefaultIcon];
        }


        private ImageSource ShellIcon(string fileName)
        {
            try
            {
                // http://blogs.msdn.com/b/oldnewthing/archive/2011/01/27/10120844.aspx
                var shfi = new SHFILEINFO();
                var himl = SHGetFileInfo(
                    fileName,
                    FILE_ATTRIBUTE_NORMAL,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    SHGFI_SYSICONINDEX
                );

                if (himl != IntPtr.Zero)
                {
                    var hIcon = ImageList_GetIcon(himl, shfi.iIcon, ILD_NORMAL);
                    // http://stackoverflow.com/questions/1325625/how-do-i-display-a-windows-file-icon-in-wpf
                    var img = Imaging.CreateBitmapSourceFromHIcon(
                        hIcon,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );
                    DestroyIcon(hIcon);
                    return img;
                }
                else
                {
                    return new BitmapImage(new Uri(Constant.ErrorIcon));
                }
            }
            catch (System.Exception e)
            {
                Logger.Exception($"|ImageLoader.ShellIcon|can't get shell icon for <{fileName}>", e);
                return _imageCache[Constant.ErrorIcon];
            }


        }

        private const int NAMESIZE = 80;
        private const int MAX_PATH = 256;
        private const uint SHGFI_SYSICONINDEX = 0x000004000; // get system icon index
        private const uint FILE_ATTRIBUTE_NORMAL = 0x00000080;
        private const uint ILD_NORMAL = 0x00000000;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct SHFILEINFO
        {
            readonly IntPtr hIcon;
            internal readonly int iIcon;
            readonly uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)] readonly string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = NAMESIZE)] readonly string szTypeName;
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("User32.dll")]
        private static extern int DestroyIcon(IntPtr hIcon);

        [DllImport("comctl32.dll")]
        private static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, uint flags);
    }
}
