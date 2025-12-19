using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using SMDisLabSys.BLL.RealData;
using SMDisLabSys.Common;
using SMDisLabSys.Common.DataConvert;
using static SMDisLabSys.BLL.RealData.RealDataBLE;

namespace SMDisLabSys.BLL
{
    public class SMDataSource
    {
        public static SMDataSource Instance = new SMDataSource();

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
        public void InitBLE()
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
            //ble1 = null;
            //ble2 = null;
            //ble3 = null;
            //ble4 = null;
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

                    default:
                        break;
                }

            }
        }

        private void Ble1_DeceiveValueChanged(object sender, DeceiveDataArgs e)
        {
            var buffer = e.ReportBuff;
            var value1 = BufferConvertHelper.BytesToInt(buffer.Skip(10).Take(2).Reverse().ToArray());
            var value2 = BufferConvertHelper.BytesToInt(buffer.Skip(12).Take(2).Reverse().ToArray());

            DataParseBLEEventArgs dataParseBLE = new DataParseBLEEventArgs();
            dataParseBLE.BLEAddress = ble1.BLEAdresse;
            dataParseBLE.ParamList = new List<double>() { Math.Round(value1 * 0.01, 2), Math.Round(value2 * 0.1, 1) };
            RealDataBLE.Instance.UpdataBLEData(dataParseBLE);
        }

        //public void InitBlePara()
        //{
        //    IsRecordBle1 = true;
        //    Ble1DataCount = 0;
        //    readDataIndex = 0;
        //    BleSkipPoint = 0;

        //    IsRecordBle2 = true;
        //    Ble2DataCount = 0;
        //    readDataIndex2 = 0;
        //    Ble2SkipPoint = 0;

        //    int sleepCount = 40;
        //    while (sleepCount > 0)
        //    {
        //        if (ble1 != null && ble1.bluetooth.IsConnected && ble1.IsNewSensor)
        //        {
        //            ble1.SendCommand(NewCommandType.GetDeviceInfo());//新协议下发命令  旧的传感器收到命令就卡住了
        //            break;
        //        }
        //        else
        //        {
        //            sleepCount--;
        //            Thread.Sleep(500);
        //        }
        //    }
        //}
    }
}
