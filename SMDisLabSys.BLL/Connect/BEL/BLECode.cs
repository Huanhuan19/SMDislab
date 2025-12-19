using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMDisLabSys.Common.DataConvert;
using SMDisLabSys.Common;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Security.Cryptography;

namespace SMDisLabSys.BLL
{
    public class BluetoothLECode
    {
        //存储检测的设备MAC。
        public string CurrentDeviceMAC { get; set; }
        //存储检测到的设备。
        public BluetoothLEDevice CurrentDevice { get; set; }
        //存储检测到的主服务。
        public GattDeviceService CurrentService { get; set; }
        //存储检测到的写特征对象。
        public GattCharacteristic CurrentWriteCharacteristic { get; set; }
        //存储检测到的通知特征对象。
        public GattCharacteristic CurrentNotifyCharacteristic { get; set; }

        public string ServiceGuid { get; set; }

        public string WriteCharacteristicGuid { get; set; }
        public string NotifyCharacteristicGuid { get; set; }


        private const int CHARACTERISTIC_INDEX = 0;
        //特性通知类型通知启用
        private const GattClientCharacteristicConfigurationDescriptorValue CHARACTERISTIC_NOTIFICATION_TYPE = GattClientCharacteristicConfigurationDescriptorValue.Notify;


        private Boolean asyncLock = false;

        private DeviceWatcher deviceWatcher;



        //定义一个委托
        public delegate void eventRun(MsgType type, byte[] data, BluetoothInfo btInfo = null);
        //定义一个事件
        public event eventRun ValueChanged;

        //添加一个判断连接上的标志
        public bool IsConnected = false;

        public BluetoothLECode(string serviceGuid, string writeCharacteristicGuid, string notifyCharacteristicGuid)
        {
            ServiceGuid = serviceGuid;
            WriteCharacteristicGuid = writeCharacteristicGuid;
            NotifyCharacteristicGuid = notifyCharacteristicGuid;
        }

        public void StartBleDeviceWatcher()
        {
            try
            {


                string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };
                string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
                string[] requestedProperties2 = { "System.Devices.Aep.DeviceAddress" };

                deviceWatcher =
                        DeviceInformation.CreateWatcher(
                            aqsAllBluetoothLEDevices,
                            requestedProperties,
                            DeviceInformationKind.AssociationEndpoint);
                //deviceWatcher = DeviceInformation.CreateWatcher(DeviceClas);
                //deviceWatcher = DeviceInformation.CreateWatcher(aqsAllBluetoothLEDevices);
                // deviceWatcher = DeviceInformation.CreateWatcher();
                // Register event handlers before starting the watcher.
                deviceWatcher.Added += DeviceWatcher_Added;
                deviceWatcher.Stopped += DeviceWatcher_Stopped;
                deviceWatcher.Start();
                //string msg = "自动发现设备中..";

                //ValueChanged(MsgType.NotifyTxt, msg);
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            //string msg = "自动发现设备停止";
            //ValueChanged(MsgType.NotifyTxt, msg);
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (args.Name.Length < 3)
            {
                return;
            }
            LogMgr.Instance.Info($"查询到 {args.Name} 蓝牙");
            switch (args.Name.Substring(0, 3))
            {
                //case "A5":
                //case "B5":
                //case "C5":
                //case "A0":
                //case "B0":
                //case "C0":
                //case "WC"://2021新协议 WCY开头
                case "SHM":
                    ValueChanged(MsgType.AddBluetooth, null, new BluetoothInfo() { Adresse = args.Name, MAC = args.Id });
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 按MAC地址查找系统中配对设备
        /// </summary>
        /// <param name="MAC"></param>
        public async Task SelectDevice(string MAC)
        {
            CurrentDeviceMAC = MAC;
            CurrentDevice = null;
            await Matching(MAC);
        }
        /// <summary>
        /// 按MAC地址直接组装设备ID查找设备
        /// </summary>
        /// <param name="MAC"></param>
        /// <returns></returns>
        public async Task SelectDeviceFromIdAsync(string MAC)
        {
            CurrentDeviceMAC = MAC;
            CurrentDevice = null;
            BluetoothAdapter.GetDefaultAsync().Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    BluetoothAdapter mBluetoothAdapter = asyncInfo.GetResults();
                    byte[] _Bytes1 = BitConverter.GetBytes(mBluetoothAdapter.BluetoothAddress);//ulong转换为byte数组
                    Array.Reverse(_Bytes1);
                    string macAddress = BitConverter.ToString(_Bytes1, 2, 6).Replace('-', ':').ToLower();
                    string Id = "BluetoothLE#BluetoothLE" + macAddress + "-" + MAC;
                    await Matching(Id);
                }

            };

        }

        private async Task Matching(string Id)
        {

            try
            {
                BluetoothLEDevice.FromIdAsync(Id).Completed = async (asyncInfo, asyncStatus) =>
                {
                    if (asyncStatus == AsyncStatus.Completed)
                    {
                        BluetoothLEDevice bleDevice = asyncInfo.GetResults();
                        //在当前设备变量中保存检测到的设备。
                        CurrentDevice = bleDevice;
                        await Connect();

                    }
                };
            }
            catch (Exception e)
            {
                string msg = "没有发现设备" + e.ToString();
                //ValueChanged(MsgType.NotifyTxt, msg);
                StartBleDeviceWatcher();
            }

        }

        private async Task Connect()
        {
            string msg = "正在连接设备<" + CurrentDeviceMAC + ">..";
            //ValueChanged(MsgType.NotifyTxt, msg);
            CurrentDevice.ConnectionStatusChanged += CurrentDevice_ConnectionStatusChanged;
            await SelectDeviceService();

        }


        /// <summary>
        /// 主动断开连接
        /// </summary>
        /// <returns></returns>
        public void Dispose()
        {

            CurrentDeviceMAC = null;
            CurrentService?.Dispose();
            CurrentDevice?.Dispose();
            CurrentDevice = null;
            CurrentService = null;
            CurrentWriteCharacteristic = null;
            CurrentNotifyCharacteristic = null;
            IsConnected = false;
            //ValueChanged(MsgType.NotifyTxt, "主动断开连接");

        }

        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            if (sender.ConnectionStatus == BluetoothConnectionStatus.Disconnected && CurrentDeviceMAC != null)
            {
                string msg = "设备已断开,自动重连";
                //ValueChanged(MsgType.NotifyTxt, msg);
                if (!asyncLock)
                {
                    ValueChanged(MsgType.DeleteBluetooth, null);
                    asyncLock = true;
                    CurrentDevice.Dispose();
                    CurrentDevice = null;
                    CurrentService = null;
                    CurrentWriteCharacteristic = null;
                    CurrentNotifyCharacteristic = null;
                    // SelectDeviceFromIdAsync(CurrentDeviceMAC);
                    // SelectDevice(CurrentDeviceMAC);
                }

            }
            else
            {
                string msg = "设备已连接";
                //ValueChanged(MsgType.NotifyTxt, msg);
            }
        }
        /// <summary>
        /// 按GUID 查找主服务
        /// </summary>
        /// <param name="characteristic">GUID 字符串</param>
        /// <returns></returns>
        public async Task SelectDeviceService()
        {
            Guid guid = new Guid(ServiceGuid);
            CurrentDevice.GetGattServicesForUuidAsync(guid).Completed = (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    try
                    {
                        GattDeviceServicesResult result = asyncInfo.GetResults();
                        string msg = "主服务=" + CurrentDevice.ConnectionStatus;
                        //ValueChanged(MsgType.NotifyTxt, msg);
                        if (result.Services.Count > 0)
                        {
                            CurrentService = result.Services[CHARACTERISTIC_INDEX];
                            if (CurrentService != null)
                            {
                                asyncLock = true;
                                GetCurrentWriteCharacteristic();
                                GetCurrentNotifyCharacteristic();

                            }
                        }
                        else
                        {
                            msg = "没有发现服务,自动重试中";
                            //ValueChanged(MsgType.NotifyTxt, msg);
                            SelectDeviceService();
                        }
                    }
                    catch (Exception e)
                    {
                        //ValueChanged(MsgType.NotifyTxt, "没有发现服务,自动重试中");
                        SelectDeviceService();

                    }
                }
            };
        }


        /// <summary>
        /// 设置写特征对象。
        /// </summary>
        /// <returns></returns>
        public async Task GetCurrentWriteCharacteristic()
        {

            string msg = "";
            Guid guid = new Guid(WriteCharacteristicGuid);
            CurrentService.GetCharacteristicsForUuidAsync(guid).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCharacteristicsResult result = asyncInfo.GetResults();
                    msg = "特征对象=" + CurrentDevice.ConnectionStatus;
                    //ValueChanged(MsgType.NotifyTxt, msg);
                    if (result.Characteristics.Count > 0)
                    {
                        CurrentWriteCharacteristic = result.Characteristics[CHARACTERISTIC_INDEX];
                        msg = "WriteTure";
                        // ValueChanged(MsgType.NotifyTxt, msg);
                        IsConnected = true;
                    }
                    else
                    {
                        //msg = "没有发现特征对象,自动重试中";
                        //ValueChanged(MsgType.NotifyTxt, msg);
                        await GetCurrentWriteCharacteristic();
                        IsConnected = false;
                    }
                }
            };
        }




        /// <summary>
        /// 发送数据接口
        /// </summary>
        /// <param name="characteristic"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void Write(byte[] data)
        {
            if (CurrentWriteCharacteristic != null)
            {
                CurrentWriteCharacteristic.WriteValueAsync(CryptographicBuffer.CreateFromByteArray(data), GattWriteOption.WriteWithResponse);
            }

        }

        /// <summary>
        /// 设置通知特征对象。
        /// </summary>
        /// <returns></returns>
        public async Task GetCurrentNotifyCharacteristic()
        {
            string msg = "";
            Guid guid = new Guid(NotifyCharacteristicGuid);
            CurrentService.GetCharacteristicsForUuidAsync(guid).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCharacteristicsResult result = asyncInfo.GetResults();
                    msg = "特征对象=" + CurrentDevice.ConnectionStatus;

                    if (result.Characteristics.Count > 0)
                    {
                        CurrentNotifyCharacteristic = result.Characteristics[CHARACTERISTIC_INDEX];
                        CurrentNotifyCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;
                        CurrentNotifyCharacteristic.ValueChanged += Characteristic_ValueChanged;
                        await EnableNotifications(CurrentNotifyCharacteristic);
                        ValueChanged(MsgType.SendOK, null);
                    }
                    else
                    {
                        //msg = "没有发现特征对象,自动重试中";
                        //ValueChanged(MsgType.NotifyTxt, msg);
                        await GetCurrentNotifyCharacteristic();
                    }
                }
            };
        }

        /// <summary>
        /// 设置特征对象为接收通知对象
        /// </summary>
        /// <param name="characteristic"></param>
        /// <returns></returns>
        public async Task EnableNotifications(GattCharacteristic characteristic)
        {
            string msg = "收通知对象=" + CurrentDevice.ConnectionStatus;
            //ValueChanged(MsgType.NotifyTxt, msg);

            characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(CHARACTERISTIC_NOTIFICATION_TYPE).Completed = async (asyncInfo, asyncStatus) =>
            {
                if (asyncStatus == AsyncStatus.Completed)
                {
                    GattCommunicationStatus status = asyncInfo.GetResults();
                    if (status == GattCommunicationStatus.Unreachable)
                    {
                        msg = "设备不可用";
                        // ValueChanged(MsgType.NotifyTxt, msg);
                        if (CurrentNotifyCharacteristic != null && !asyncLock)
                        {
                            await EnableNotifications(CurrentNotifyCharacteristic);
                        }
                    }
                    asyncLock = false;
                    msg = "设备连接状态" + status;
                    //ValueChanged(MsgType.NotifyTxt, msg);
                }
            };
        }
        List<byte> readList = new List<byte>();
        List<byte[]> readListS = new List<byte[]>();
        bool first = true;
        DateTime tt;
        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            //if (first)
            //{
            //    first = false;
            //    tt = DateTime.Now;
            //}
            byte[] data;

            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out data);
            LogMgr.Instance.Info($"收到数据 {BufferConvertHelper.BufferTo16Str(data)}");
            readListS.Add(data);
            lock (readList)
            {
                readList.AddRange(data);
                if (CheckList(readList))
                {
                    //readList = readList.Skip(9).ToList();
                    ValueChanged(MsgType.BLEData, readList.ToArray());
                    readList.Clear();
                }
            }
        }
        bool CheckList(List<byte> frame)
        {
            while (readList[0] != 0xce && (readList[1]) != 0xcb)
            {
                readList.RemoveAt(0);
                if (readList.Count < 51)
                {
                    return false;
                }
            }
            return true;
        }
        //ReceiveDataCount CheckList(List<byte> frame)
        //{
        //    if (frame.Count > 0)
        //    {
        //        switch ((ReceiveFirstSign)frame[0])
        //        {
        //            case ReceiveFirstSign.Common:
        //                if (frame.Count >= (byte)ReceiveDataCount.Common_Data)
        //                    return ReceiveDataCount.Common_Data;
        //                else
        //                    return ReceiveDataCount.NULL;
        //            case ReceiveFirstSign.BatteryLevel:
        //                if (frame.Count >= (byte)ReceiveDataCount.Common_Data)
        //                    return ReceiveDataCount.Common_Data;
        //                else
        //                    return ReceiveDataCount.NULL;
        //            case ReceiveFirstSign.BatteryLevelCurrentTypeSensorsA1:
        //            case ReceiveFirstSign.BatteryLevelCurrentTypeSensorsB1:
        //            case ReceiveFirstSign.BatteryLevelCurrentTypeSensorsC1:
        //                if (frame.Count >= (byte)ReceiveDataCount.BatteryLevelCurrentTypeSensors_data)
        //                    return ReceiveDataCount.BatteryLevelCurrentTypeSensors_data;
        //                else
        //                    return ReceiveDataCount.NULL;
        //            case ReceiveFirstSign.CurrentTypeSensorsA0://only add voice sensor,not include current or orther. 
        //            case ReceiveFirstSign.CurrentTypeSensorsB0:
        //            case ReceiveFirstSign.CurrentTypeSensorsC0:
        //                if (frame.Count >= (int)ReceiveDataCount.VoiceData_Count)
        //                    return ReceiveDataCount.VoiceData_Count;
        //                else
        //                    return ReceiveDataCount.NULL;
        //            default:
        //                frame.RemoveAt(0);
        //                CheckList(frame);
        //                break;
        //        }
        //    }
        //    return ReceiveDataCount.NULL;
        //}

        public static string ByteArrayToString(byte[] pArray, int pBase = 16, string pSeperator = " ")
        {
            if (pArray == null) return "";
            return ByteArrayToString(pArray, 0, pArray.Length - 1, pBase, pSeperator);
        }
        /// <summary>
        /// index count from 0
        /// </summary>
        public static string ByteArrayToString(byte[] pArray, int pFromIndex, int pEndIndex, int pBase, string pSeperator)
        {
            if (pArray == null || pArray.Length < 1) return "";
            StringBuilder sb = new StringBuilder((pEndIndex - pFromIndex) * 2 + (pEndIndex - pFromIndex + 1) * pSeperator.Length);
            string format = "X2";
            if (pBase == 10)
                format = "D2";
            for (int i = pFromIndex; i <= pEndIndex; i++)
            {
                sb.Append(pArray[i].ToString(format) + pSeperator);
            }
            return sb.ToString();
        }

    }



    public enum MsgType
    {
        NotifyTxt,
        SendOK,
        BLEData,
        AddBluetooth,
        DeleteBluetooth,
    }
    public class BluetoothInfo
    {
        string _Adresse;
        string _MAC;
        public string Adresse
        {
            get { return _Adresse; }
            set { _Adresse = value; }
        }
        public string MAC
        {
            get { return _MAC; }
            set { _MAC = value; }
        }
    }
}