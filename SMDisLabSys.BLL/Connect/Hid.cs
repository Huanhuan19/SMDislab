using System;
using System.Linq;
using System.Threading.Tasks;
using HidLibrary;

namespace SMDisLabSys.BLL.Connect
{
    public class Hid
    {
        private volatile bool _keepReading;
        private Task _readTask;
        private readonly object _sync = new object();

        public HidLibrary.HidDevice _device;

        /// <summary>
        /// 打开指定 VID/PID 的第 hidNo 个 HID 设备并启动连续读。
        /// 可重复调用；调用前会先关闭已有连接。
        /// </summary>
        public bool CreatHid(UInt16 vID, UInt16 pID, int hidNo)
        {
            lock (_sync)
            {
                CloseInternal();

                var devices = HidDevices.Enumerate(vID, pID).ToList();
                if (devices.Count < hidNo)
                {
                    return false;
                }

                _device = devices[hidNo - 1];
                if (_device == null)
                {
                    return false;
                }

                _device.OpenDevice();
                if (!_device.IsConnected)
                {
                    _device = null;
                    return false;
                }

                _keepReading = true;
                var device = _device;
                _readTask = Task.Run(() => ReadContinuously(device));
                return true;
            }
        }

        public void Close()
        {
            lock (_sync)
            {
                CloseInternal();
            }
        }

        private void CloseInternal()
        {
            _keepReading = false;
            try
            {
                _device?.CloseDevice();
            }
            catch
            {
                // ignore close errors during hot-plug
            }

            try
            {
                _readTask?.Wait(500);
            }
            catch
            {
                // ignore
            }

            _device = null;
            _readTask = null;
        }

        private void ReadContinuously(IHidDevice device)
        {
            while (_keepReading)
            {
                try
                {
                    if (!device.IsConnected)
                    {
                        break;
                    }

                    var report = device.ReadReport();
                    if (!_keepReading)
                    {
                        break;
                    }

                    if (report != null)
                    {
                        ProcessReport(report);
                    }

                    Task.Delay(10).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"读取异常: {ex.Message}");
                    break;
                }
            }
        }

        private void ProcessReport(HidReport report)
        {
            var data = report.Data;
            if (data != null && data.Length > 0)
            {
                OnDeceiveDataChanged(new DeceiveDataArgs() { ReportBuff = data });
            }
        }

        public event DeceiveDataHandler DeceiveValueChanged = null;

        public void OnDeceiveDataChanged(DeceiveDataArgs e)
        {
            if (DeceiveValueChanged != null)
            {
                DeceiveValueChanged(this, e);
            }
        }

        public void SendBuffer(byte[] sendBuffer)
        {
            var device = _device;
            if (device != null && device.IsConnected)
            {
                HidReport report = new HidReport(sendBuffer.Length)
                {
                    Data = sendBuffer
                };
                device.WriteReport(report);
            }
        }
    }
}
