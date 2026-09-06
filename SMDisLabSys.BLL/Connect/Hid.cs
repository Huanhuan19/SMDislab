using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.HumanInterfaceDevice;
using HidLibrary;
using NPOI.SS.Formula.Functions;

namespace SMDisLabSys.BLL.Connect
{
    public class Hid
    {
        private static bool _keepReading = true;
        public HidLibrary.HidDevice _device;
        public void CreatHid(UInt16 vID, UInt16 pID, int hidNo)
        {
            Creet(vID, pID, hidNo);
        }
        async void Creet(UInt16 vID, UInt16 pID, int hidNo)
        {
            // 获取所有连接的HID设备
            var devices = HidDevices.Enumerate(vID, pID);
            if (devices.Any())
            {
                if (devices.Count() < hidNo)
                {
                    return;
                }
                _device = devices.ElementAt(hidNo - 1);
                if (_device != null)
                {
                    _device.OpenDevice();

                    // 使用异步读取
                    var readTask = Task.Run(() => ReadContinuously(_device));
                    await readTask;
                }
            }
        }
        private void ReadContinuously(IHidDevice device)
        {
            while (true)
            {
                try
                {
                    // 同步读取
                    var report = device.ReadReport();
                    if (report != null)
                    {
                        // 处理数据
                        ProcessReport(report);
                    }

                    // 短暂延迟避免CPU占用过高
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
            if (_device.IsConnected)
            {
                HidReport report = new HidReport(sendBuffer.Length)
                {
                    Data = sendBuffer
                };
                _device.WriteReport(report);
            }
        }
    }
}
