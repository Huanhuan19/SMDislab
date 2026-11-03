using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Prism.Services.Dialogs;

namespace SMDisLabSys.UIServer
{
    /// <summary>
    /// 所有窗口使用同一个 AppUserModelID，任务栏只显示一个图标。
    /// 无边框全屏窗口的实时预览会被画成最上层页面，因此为每个窗口缓存自己的缩略图。
    /// </summary>
    public static class WindowTaskbarHelper
    {
        public const string AppUserModelId = "SMDisLabSys";

        const int DWMWA_FORCE_ICONIC_REPRESENTATION = 7;
        const int DWMWA_HAS_ICONIC_BITMAP = 10;
        const int WM_DWMSENDICONICTHUMBNAIL = 0x0323;
        const int WM_DWMSENDICONICLIVEPREVIEWBITMAP = 0x0326;

        static readonly ConditionalWeakTable<Window, BitmapSource> PreviewCache = new ConditionalWeakTable<Window, BitmapSource>();

        public static void InitProcess()
        {
            try
            {
                SetCurrentProcessExplicitAppUserModelID(AppUserModelId);
            }
            catch
            {
            }
        }

        public static void Attach(Window window, string fallbackTitle = null)
        {
            if (window == null)
            {
                return;
            }

            window.SourceInitialized += (s, e) =>
            {
                ApplyTitle(window, fallbackTitle);
                SetAppUserModelId(window);
                EnableIconicPreview(window);
                var hwnd = new WindowInteropHelper(window).Handle;
                var source = HwndSource.FromHwnd(hwnd);
                source?.AddHook((IntPtr h, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) =>
                    WndProc(window, h, msg, wParam, lParam, ref handled));
            };

            window.Loaded += (s, e) =>
            {
                ApplyTitle(window, fallbackTitle);
                SetAppUserModelId(window);
                window.Dispatcher.BeginInvoke(new Action(() => Capture(window)), System.Windows.Threading.DispatcherPriority.Loaded);
            };

            window.ContentRendered += (s, e) => Capture(window);
            window.Deactivated += (s, e) => Capture(window);
        }

        static void ApplyTitle(Window window, string fallbackTitle)
        {
            string title = window.Title;
            if (window.DataContext is IDialogAware dialog && !string.IsNullOrWhiteSpace(dialog.Title))
            {
                title = dialog.Title;
            }
            else if (string.IsNullOrWhiteSpace(title) || title == "UcWinSize" || title == "MainWindow")
            {
                if (!string.IsNullOrWhiteSpace(fallbackTitle))
                {
                    title = fallbackTitle;
                }
            }
            if (!string.IsNullOrWhiteSpace(title) && window.Title != title)
            {
                window.Title = title;
            }
        }

        static void EnableIconicPreview(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
            {
                return;
            }
            int enable = 1;
            DwmSetWindowAttribute(hwnd, DWMWA_FORCE_ICONIC_REPRESENTATION, ref enable, sizeof(int));
            DwmSetWindowAttribute(hwnd, DWMWA_HAS_ICONIC_BITMAP, ref enable, sizeof(int));
        }

        static void Capture(Window window)
        {
            if (window == null || !window.IsLoaded || window.ActualWidth < 8 || window.ActualHeight < 8)
            {
                return;
            }
            try
            {
                var bmp = RenderWindow(window);
                if (bmp == null)
                {
                    return;
                }
                PreviewCache.Remove(window);
                PreviewCache.Add(window, bmp);
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd != IntPtr.Zero)
                {
                    DwmInvalidateIconicBitmaps(hwnd);
                }
            }
            catch
            {
            }
        }

        static BitmapSource RenderWindow(Window window)
        {
            double dpi = 1;
            try
            {
                dpi = VisualTreeHelper.GetDpi(window).PixelsPerDip;
            }
            catch
            {
            }
            int width = Math.Max(1, (int)Math.Ceiling(window.ActualWidth * dpi));
            int height = Math.Max(1, (int)Math.Ceiling(window.ActualHeight * dpi));
            var rtb = new RenderTargetBitmap(width, height, 96 * dpi, 96 * dpi, PixelFormats.Pbgra32);
            rtb.Render(window);
            rtb.Freeze();
            return rtb;
        }

        static IntPtr WndProc(Window window, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DWMSENDICONICTHUMBNAIL)
            {
                int reqW = (int)(lParam.ToInt64() & 0xFFFF);
                int reqH = (int)((lParam.ToInt64() >> 16) & 0xFFFF);
                SendIconic(window, hwnd, reqW, reqH, livePreview: false);
                handled = true;
            }
            else if (msg == WM_DWMSENDICONICLIVEPREVIEWBITMAP)
            {
                SendIconic(window, hwnd, 0, 0, livePreview: true);
                handled = true;
            }
            return IntPtr.Zero;
        }

        static void SendIconic(Window window, IntPtr hwnd, int reqW, int reqH, bool livePreview)
        {
            if (!PreviewCache.TryGetValue(window, out BitmapSource source) || source == null)
            {
                Capture(window);
                PreviewCache.TryGetValue(window, out source);
            }
            if (source == null)
            {
                return;
            }

            BitmapSource toSend = source;
            if (!livePreview && reqW > 0 && reqH > 0)
            {
                toSend = Resize(source, reqW, reqH);
            }

            IntPtr hBitmap = CreateHBitmap(toSend);
            if (hBitmap == IntPtr.Zero)
            {
                return;
            }
            try
            {
                if (livePreview)
                {
                    DwmSetIconicLivePreviewBitmap(hwnd, hBitmap, IntPtr.Zero, 0);
                }
                else
                {
                    DwmSetIconicThumbnail(hwnd, hBitmap, 0);
                }
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }

        static BitmapSource Resize(BitmapSource source, int width, int height)
        {
            double scale = Math.Min((double)width / source.PixelWidth, (double)height / source.PixelHeight);
            var transformed = new TransformedBitmap(source, new ScaleTransform(scale, scale));
            transformed.Freeze();
            return transformed;
        }

        static IntPtr CreateHBitmap(BitmapSource source)
        {
            int width = source.PixelWidth;
            int height = source.PixelHeight;
            int stride = width * 4;
            byte[] pixels = new byte[height * stride];
            var converted = new FormatConvertedBitmap(source, PixelFormats.Bgra32, null, 0);
            converted.CopyPixels(pixels, stride, 0);

            var info = new BITMAPINFO();
            info.biSize = Marshal.SizeOf<BITMAPINFO>();
            info.biWidth = width;
            info.biHeight = -height;
            info.biPlanes = 1;
            info.biBitCount = 32;
            info.biCompression = 0;

            IntPtr bits;
            IntPtr hBitmap = CreateDIBSection(IntPtr.Zero, ref info, 0, out bits, IntPtr.Zero, 0);
            if (hBitmap != IntPtr.Zero && bits != IntPtr.Zero)
            {
                Marshal.Copy(pixels, 0, bits, pixels.Length);
            }
            return hBitmap;
        }

        static void SetAppUserModelId(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero)
            {
                return;
            }
            try
            {
                Guid iid = typeof(IPropertyStore).GUID;
                if (SHGetPropertyStoreForWindow(hwnd, ref iid, out IPropertyStore store) != 0 || store == null)
                {
                    return;
                }
                var key = new PROPERTYKEY(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
                var pv = PROPVARIANT.FromString(AppUserModelId);
                try
                {
                    store.SetValue(ref key, ref pv);
                    store.Commit();
                }
                finally
                {
                    pv.Dispose();
                    Marshal.ReleaseComObject(store);
                }
            }
            catch
            {
            }
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        static extern int SetCurrentProcessExplicitAppUserModelID(string AppID);

        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("dwmapi.dll")]
        static extern int DwmInvalidateIconicBitmaps(IntPtr hwnd);

        [DllImport("dwmapi.dll")]
        static extern int DwmSetIconicThumbnail(IntPtr hwnd, IntPtr hBitmap, int flags);

        [DllImport("dwmapi.dll")]
        static extern int DwmSetIconicLivePreviewBitmap(IntPtr hwnd, IntPtr hBitmap, IntPtr ptClient, int flags);

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateDIBSection(IntPtr hdc, ref BITMAPINFO pbmi, uint usage, out IntPtr ppvBits, IntPtr hSection, uint offset);

        [DllImport("shell32.dll")]
        static extern int SHGetPropertyStoreForWindow(IntPtr hwnd, ref Guid riid, out IPropertyStore propertyStore);

        [StructLayout(LayoutKind.Sequential)]
        struct BITMAPINFO
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct PROPERTYKEY
        {
            public Guid fmtid;
            public uint pid;
            public PROPERTYKEY(Guid fmtid, uint pid)
            {
                this.fmtid = fmtid;
                this.pid = pid;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PROPVARIANT : IDisposable
        {
            ushort vt;
            ushort wReserved1;
            ushort wReserved2;
            ushort wReserved3;
            IntPtr pointerValue;

            const ushort VT_LPWSTR = 31;

            public static PROPVARIANT FromString(string value)
            {
                return new PROPVARIANT
                {
                    vt = VT_LPWSTR,
                    pointerValue = Marshal.StringToCoTaskMemUni(value)
                };
            }

            public void Dispose()
            {
                if (pointerValue != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pointerValue);
                    pointerValue = IntPtr.Zero;
                }
            }
        }

        [ComImport]
        [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IPropertyStore
        {
            void GetCount(out uint cProps);
            void GetAt(uint iProp, out PROPERTYKEY pkey);
            void GetValue(ref PROPERTYKEY key, out PROPVARIANT pv);
            void SetValue(ref PROPERTYKEY key, ref PROPVARIANT pv);
            void Commit();
        }
    }
}
