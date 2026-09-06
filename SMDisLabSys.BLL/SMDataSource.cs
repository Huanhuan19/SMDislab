using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using SMDisLabSys.BLL.Connect;
using SMDisLabSys.BLL.Protocol;
using SMDisLabSys.BLL.RealData;
using SMDisLabSys.Common;
using SMDisLabSys.Common.DataConvert;
using SMDisLabSys.Model;
using Windows.Storage.Streams;
using static SMDisLabSys.BLL.RealData.RealDataBLE;

namespace SMDisLabSys.BLL
{
    public class SMDataSource
    {
        public static SMDataSource Instance = new SMDataSource();

        #region 蓝牙


        BLEDeviceConnect ble;
        BLEDeviceConnect ble1;
        BLEDeviceConnect ble2;
        BLEDeviceConnect ble3;
        BLEDeviceConnect ble4;

        public List<BluetoothInfo> BluetoothList = new List<BluetoothInfo>();//All the history bluetooth device

        List<BluetoothInfo> btListHistory = new List<BluetoothInfo>();//select bluetooth
        List<SenserMsg> sensorBuffer = new List<SenserMsg>();

        bool IsStartBLE = false;//定义蓝牙功能 搜索传感器
        public void BLEStart()//2021蓝牙和USB 分开初始化  再from中调用 不是win10的电脑也可以用
        {
            // BLE Init
            BLEDispose();
            InitBLE();
            IsStartBLE = true;
        }
        void InitBLE()
        {
            Thread.Sleep(500);
            ble = new BLEDeviceConnect(0, "", "", "");
            ble.BleWarchingChanged += Ble_BleWarchingChanged;

            StartBluetoothSearch();

            //BluetoothConnect(btListHistory);
        }
        public void StartBluetoothSearch()
        {
            if (ble != null)
            {
                //bluetoothList.Clear();
                ble.StartBluetoothSearch();
            }
        }

        private void Ble_BleWarchingChanged(object sender, BleWarchingArgs e)
        {
            IsAndWarchInfo(e.btInfo);
        }
        void IsAndWarchInfo(BluetoothInfo bt)
        {
            BluetoothInfo btInfo = (from o in BluetoothList where o.Adresse == bt.Adresse && o.MAC == bt.MAC select o).ToList().FirstOrDefault();
            if (btInfo == null)
            {
                btInfo = new BluetoothInfo();
                btInfo.Adresse = bt.Adresse;
                btInfo.MAC = bt.MAC;
                BluetoothList.Add(btInfo);
            }
        }

        void BLEDispose()
        {
            foreach (var item in sensorBuffer)
            {
                switch (item.connectName)
                {
                    case "ble1":
                        ble1.bluetooth.Dispose();
                        break;
                    case "ble2":
                        ble2.bluetooth.Dispose();
                        break;
                    case "ble3":
                        ble3.bluetooth.Dispose();
                        break;
                    case "ble4":
                        ble4.bluetooth.Dispose();
                        break;
                }
            }
        }

        public void BluetoothConnect(List<BluetoothInfo> btList)
        {
            btListHistory = btList;
            Task t = Task.Run(() =>
            {
                BluetoothInitConnect(btListHistory);
            });
            //InitBlePara();
        }
        void ConnectLog(List<BluetoothInfo> btList)
        {
            foreach (var item in btList)
            {
                LogMgr.Instance.Info($"连接蓝牙设备：{item.Adresse}");
            }
        }
        public void BluetoothInitConnect(List<BluetoothInfo> btList)
        {
            ConnectLog(btList);
            BLEDispose();
            Thread.Sleep(1000);
            sensorBuffer.RemoveAll(o => o.connectName.Contains("ble"));
            byte usbCount = 0;
            for (int i = 0; i < btList.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        ble1 = new BLEDeviceConnect(usbCount, btList[i].MAC, $"ble1", btList[i].Adresse);
                        ble1.DeceiveValueChanged += Ble1_DeceiveValueChanged;

                        //ble1.BleDeleteChanged += Ble1_BleDeleteChanged;
                        //ble1.AddSensorDataChanged += AddSensorDataChanged;
                        //Thread.Sleep(100000);
                        break;
                    case 1:
                        ble2 = new BLEDeviceConnect((byte)(usbCount + 1), btList[i].MAC, $"ble2", btList[i].Adresse);
                        ble2.DeceiveValueChanged += Ble2_DeceiveValueChanged;
                        //ble1.BleDeleteChanged += Ble1_BleDeleteChanged;
                        //ble1.AddSensorDataChanged += AddSensorDataChanged;
                        //Thread.Sleep(100000);
                        break;

                    default:
                        break;
                }

            }
        }

        private void Ble1_DeceiveValueChanged(object sender, DeceiveDataArgs e)
        {
            var buffer = e.ReportBuff;
            ParseHelper.Instance.Parse(buffer, ConnectTypeEnum.BLE, 1, ble1.BLEAdresse);
        }
        private void Ble2_DeceiveValueChanged(object sender, DeceiveDataArgs e)
        {
            var buffer = e.ReportBuff;
            ParseHelper.Instance.Parse(buffer, ConnectTypeEnum.BLE, 2, ble1.BLEAdresse);
        }

        public void SendCommandBle(byte[] sendBuffer)
        {
            ble1.SendCommand(sendBuffer);
        }


        #endregion

        #region USB
        public Hid hid1 = new Hid();
        public Hid hid2 = new Hid();
        public void HidStart()
        {
            foreach (var item in PublicStaticInfo.Instance.VIDPIDList)
            {
                var vidpid = item.Split(":");
                hid1.CreatHid(Convert.ToUInt16(vidpid[0], 16), Convert.ToUInt16(vidpid[1], 16), 1);//VID 1FC9;PID 000B
                if (hid1._device != null)
                {
                    if (hid1._device.IsConnected)
                    {
                        hid1.DeceiveValueChanged += Hid1_DeceiveValueChanged;
                        break;
                    }
                }
            }
            foreach (var item in PublicStaticInfo.Instance.VIDPIDList)
            {
                var vidpid = item.Split(":");
                hid2.CreatHid(Convert.ToUInt16(vidpid[0], 16), Convert.ToUInt16(vidpid[1], 16), 2);//VID 1FC9;PID 000B
                if (hid2._device != null)
                {
                    if (hid2._device.IsConnected)
                    {
                        hid2.DeceiveValueChanged += Hid2_DeceiveValueChanged;
                        break;
                    }
                }
            }
            //hid1.CreatHid(0x0483, 0x5710);
            //hid1.CreatHid(0x1FC9, 0xB);//VID 1FC9;PID 000B
            //if (hid1._device != null)
            //{
            //    if (hid1._device.IsConnected)
            //    {
            //        hid1.DeceiveValueChanged += Hid1_DeceiveValueChanged;
            //    }
            //}
        }

        public bool HidConnected()
        {
            if (hid1._device != null)
            {
                return hid1._device.IsConnected;
            }
            else
            {
                return false;
            }
        }
        private void Hid1_DeceiveValueChanged(object sender, DeceiveDataArgs e)
        {
            ParseHelper.Instance.Parse(e.ReportBuff, ConnectTypeEnum.USB, 1);
        }
        private void Hid2_DeceiveValueChanged(object sender, DeceiveDataArgs e)
        {
            ParseHelper.Instance.Parse(e.ReportBuff, ConnectTypeEnum.USB, 2);
        }
        public void SendCommand(byte[] buffer)
        {
            hid1.SendBuffer(buffer);
        }
        #endregion
    }
}
