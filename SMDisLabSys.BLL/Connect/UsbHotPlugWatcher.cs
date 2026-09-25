using System;
using System.Windows;
using System.Windows.Interop;
using SMDisLabSys.Common;

namespace SMDisLabSys.BLL.Connect
{
    /// <summary>
    /// 监听 Windows USB/设备节点变化，触发回调以重新初始化 HID。
    /// </summary>
    public sealed class UsbHotPlugWatcher
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVNODES_CHANGED = 0x0007;

        private HwndSource _hwndSource;
        private Window _window;
        private Action _onDeviceChanged;
        private bool _started;

        public void Start(Window window, Action onDeviceChanged)
        {
            if (window == null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            _onDeviceChanged = onDeviceChanged ?? throw new ArgumentNullException(nameof(onDeviceChanged));
            _window = window;

            if (_started)
            {
                return;
            }

            _started = true;

            var helper = new WindowInteropHelper(window);
            if (helper.Handle != IntPtr.Zero)
            {
                Attach(helper.Handle);
            }
            else
            {
                window.SourceInitialized += Window_SourceInitialized;
            }

            window.Closed += Window_Closed;
        }

        public void Stop()
        {
            if (_window != null)
            {
                _window.SourceInitialized -= Window_SourceInitialized;
                _window.Closed -= Window_Closed;
            }

            if (_hwndSource != null)
            {
                _hwndSource.RemoveHook(WndProc);
                _hwndSource = null;
            }

            _started = false;
            _window = null;
            _onDeviceChanged = null;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (_window == null)
            {
                return;
            }

            var handle = new WindowInteropHelper(_window).Handle;
            if (handle != IntPtr.Zero)
            {
                Attach(handle);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void Attach(IntPtr hwnd)
        {
            if (_hwndSource != null)
            {
                return;
            }

            _hwndSource = HwndSource.FromHwnd(hwnd);
            _hwndSource?.AddHook(WndProc);
            LogMgr.Instance.Info("USB热插拔监听已启动");
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                int eventType = wParam.ToInt32();
                if (eventType == DBT_DEVICEARRIVAL
                    || eventType == DBT_DEVICEREMOVECOMPLETE
                    || eventType == DBT_DEVNODES_CHANGED)
                {
                    try
                    {
                        _onDeviceChanged?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        LogMgr.Instance.Error("USB热插拔回调异常", ex);
                    }
                }
            }

            return IntPtr.Zero;
        }
    }
}
