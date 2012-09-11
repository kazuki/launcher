using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace org.oikw.Launcher
{
    [ValueConversion (typeof (MainWindow.ResultEntry), typeof (ImageSource))]
    class EntryToIconConverter : IMultiValueConverter
    {
        Native.IImageList _imgListJumbo = null;
        Native.IImageList _imgListLarge = null;
        static readonly System.Drawing.Color EmptyColor = System.Drawing.Color.FromArgb (0, 0, 0, 0);

        public EntryToIconConverter ()
        {
            Guid iidImageList = new Guid ("46EB5926-582E-4017-9FDF-E8998DAA0950");
            Native.SHGetImageList (4, ref iidImageList, ref _imgListJumbo);
            Native.SHGetImageList (0, ref iidImageList, ref _imgListLarge);
        }

        public object Convert (object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            MainWindow.ResultEntry entry = (MainWindow.ResultEntry)values[0];
            uint attr = 0x80;
            Native.SHGetFileInfoFlags flags = Native.SHGetFileInfoFlags.SysIconIndex;
            if (!entry.IsDirectory) flags |= Native.SHGetFileInfoFlags.UseFileAttributes;
            Native.SHFILEINFO shfi = new Native.SHFILEINFO ();
            uint shfiSize = (uint)Marshal.SizeOf (shfi.GetType ());
            Native.SHGetFileInfo (entry.Path, attr, ref shfi, shfiSize, flags);

            IntPtr hIcon = IntPtr.Zero;
            _imgListJumbo.GetIcon (shfi.iIcon, Native.ImageListDrawItemConstants.ILD_TRANSPARENT, ref hIcon);
            BitmapSource src = Imaging.CreateBitmapSourceFromHIcon (hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions ());
            if (src.Width == 256 && src.Height == 256) {
                // 左上の48x48以外の領域が無色透明だったらJumboアイコン取得エラーと見なし、Largeアイコンを取得する
                const int error_img_size = 48;
                byte[] pixels = new byte[(256 - error_img_size) * (256 - error_img_size) * src.Format.BitsPerPixel / 8];
                src.CopyPixels (new Int32Rect (error_img_size, error_img_size, 256 - error_img_size, 256 - error_img_size), pixels, (src.PixelWidth - error_img_size) * src.Format.BitsPerPixel / 8, 0);
                for (int i = 0; i < pixels.Length; ++i) {
                    if (pixels[i] != 0) {
                        pixels = null;
                        break;
                    }
                }
                if (pixels != null) {
                    hIcon = IntPtr.Zero;
                    _imgListLarge.GetIcon (shfi.iIcon, Native.ImageListDrawItemConstants.ILD_TRANSPARENT, ref hIcon);
                    src = Imaging.CreateBitmapSourceFromHIcon (hIcon, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions ());
                }
            }
            return src;
        }
        
        public object[] ConvertBack (object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException ();
        }
    }
}
