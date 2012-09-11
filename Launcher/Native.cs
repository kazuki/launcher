using System;
using System.Runtime.InteropServices;

namespace org.oikw.Launcher
{
    public static class Native
    {
        [DllImport ("shell32.dll")]
        public static extern IntPtr SHGetFileInfo (string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, SHGetFileInfoFlags uFlags);

        [DllImport ("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageList (int iImageList, ref Guid riid, ref IImageList ppv);

        [DllImport ("shell32.dll", EntryPoint = "#727")]
        public extern static int SHGetImageListHandle (int iImageList, ref Guid riid, ref IntPtr handle);

        [DllImport ("gdi32.dll")]
        public static extern bool DeleteObject (IntPtr hObject);

        [StructLayout (LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs (UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs (UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [Flags]
        public enum SHGetFileInfoFlags : uint
        {
            Icon = 0x000000100,
            DisplayName = 0x000000200,
            TypeName = 0x000000400,
            Attributes = 0x000000800,
            IconLocation = 0x000001000,
            ExeType = 0x000002000,
            SysIconIndex = 0x000004000,
            LinkOverlay = 0x000008000,
            Selected = 0x000010000,
            Attr_Specified = 0x000020000,
            LargeIcon = 0x000000000,
            SmallIcon = 0x000000001,
            OpenIcon = 0x000000002,
            ShellIconSize = 0x000000004,
            PIDL = 0x000000008,
            UseFileAttributes = 0x000000010,
            AddOverlays = 0x000000020,
            OverlayIndex = 0x000000040,
        }

        [ComImportAttribute ()]
        [GuidAttribute ("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute (ComInterfaceType.InterfaceIsIUnknown)]
        public interface IImageList
        {
            [PreserveSig]
            int Add (IntPtr hbmImage, IntPtr hbmMask, ref int pi);

            [PreserveSig]
            int ReplaceIcon (int i, IntPtr hicon, ref int pi);

            [PreserveSig]
            int SetOverlayImage (int iImage, int iOverlay);

            [PreserveSig]
            int Replace (int i, IntPtr hbmImage, IntPtr hbmMask);

            [PreserveSig]
            int AddMasked (IntPtr hbmImage, int crMask, ref int pi);

            [PreserveSig]
            int Draw (ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove (int i);

            [PreserveSig]
            int GetIcon (int i, ImageListDrawItemConstants flags, ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo (int i, ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy (int iDst, IImageList punkSrc, int iSrc, int uFlags);

            [PreserveSig]
            int Merge (int i1, IImageList punk2, int i2, int dx, int dy, ref Guid riid, ref IntPtr ppv);

            [PreserveSig]
            int Clone (ref Guid riid, ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect (int i, ref RECT prc);

            [PreserveSig]
            int GetIconSize (ref int cx, ref int cy);

            [PreserveSig]
            int SetIconSize (int cx, int cy);

            [PreserveSig]
            int GetImageCount (ref int pi);

            [PreserveSig]
            int SetImageCount (int uNewCount);

            [PreserveSig]
            int SetBkColor (int clrBk, ref int pclr);

            [PreserveSig]
            int GetBkColor (ref int pclr);

            [PreserveSig]
            int BeginDrag (int iTrack, int dxHotspot, int dyHotspot);

            [PreserveSig]
            int EndDrag ();

            [PreserveSig]
            int DragEnter (IntPtr hwndLock, int x, int y);

            [PreserveSig]
            int DragLeave (IntPtr hwndLock);

            [PreserveSig]
            int DragMove (int x, int y);

            [PreserveSig]
            int SetDragCursorImage (ref IImageList punk, int iDrag, int dxHotspot, int dyHotspot);

            [PreserveSig]
            int DragShowNolock (int fShow);

            [PreserveSig]
            int GetDragImage (ref POINT ppt, ref POINT pptHotspot, ref Guid riid, ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags (int i, ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage (int iOverlay, ref int piIndex);
        };

        [StructLayout (LayoutKind.Sequential)]
        public struct RECT
        {
            int left;
            int top;
            int right;
            int bottom;
        }

        [StructLayout (LayoutKind.Sequential)]
        public struct POINT
        {
            int x;
            int y;
        }

        [StructLayout (LayoutKind.Sequential)]
        public struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;        // x offest from the upperleft of bitmap
            public int yBitmap;        // y offset from the upperleft of bitmap
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout (LayoutKind.Sequential)]
        public struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }

        [Flags]
        public enum ImageListDrawItemConstants : int
        {
            ILD_NORMAL = 0x0,
            ILD_TRANSPARENT = 0x1,
            ILD_BLEND25 = 0x2,
            ILD_SELECTED = 0x4,
            ILD_MASK = 0x10,
            ILD_IMAGE = 0x20,
            ILD_ROP = 0x40,
            ILD_PRESERVEALPHA = 0x1000,
            ILD_SCALE = 0x2000,
            ILD_DPISCALE = 0x4000
        }
    }
}
