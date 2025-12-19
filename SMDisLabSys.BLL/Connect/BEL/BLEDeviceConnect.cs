using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMDisLabSys.BLL
{
    public class BLEDeviceConnect
    {
        string _serviceGuid = "0000ffe0-0000-1000-8000-00805f9b34fb";//蓝牙模块的uuid，应该是不会变的，默认值
        string _writeCharacteristicGuid = "0000ffe2-0000-1000-8000-00805f9b34fb";//蓝牙模块的write，应该是不会变的，默认值
        string _notifyCharacteristicGuid = "0000ffe1-0000-1000-8000-00805f9b34fb";//蓝牙模块的notify，应该是不会变的，默认值
        public BluetoothLECode bluetooth;
        BluetoothInfo bt = new BluetoothInfo();
        public BLEDeviceConnect(byte pIndex, string pMAC, string pBLEName, string pAddress)
        {
            try
            {
                DataUpToUIIndex = 0;

                Frequency = 10;// set 10Hz 
                SkipCount = 9;

                index = pIndex;
                BLEMAC = pMAC;
                BLEName = pBLEName;
                BLEAdresse = pAddress;

                bt.MAC = BLEMAC;
                bt.Adresse = BLEAdresse;

                bluetooth = new BluetoothLECode(_serviceGuid, _writeCharacteristicGuid, _notifyCharacteristicGuid);
                bluetooth.ValueChanged += Bluetooth_ValueChanged;
                if (!string.IsNullOrEmpty(BLEMAC))
                {
                    OnBLEAdded(BLEMAC);
                }
                if (pAddress.Contains("WCY"))
                {
                    IsNewSensor = true;
                }

            }
            catch (Exception exc)
            {

                throw;
            }

        }

        public string BLEName;
        public string BLEMAC;
        public string BLEAdresse;
        public byte index;
        public float K;
        public float B;
        public bool IsNewSensor = false;//新传感器是WCY开头的

        public int Frequency;
        public int SkipCount;
        public int DataUpToUIIndex = 0;

        public bool BLEIsConnected = false;

        private async void OnBLEAdded(string pBLEMAC)
        {
            await bluetooth.SelectDevice(pBLEMAC);
            //await bluetooth.SelectDeviceFromIdAsync(pBLEMAC);
        }
        private void Bluetooth_ValueChanged(MsgType type, byte[] pdata, BluetoothInfo btInfo)
        {
            BLEIsConnected = true;// ble 连接成功后下发 读取指令
            if (type == MsgType.BLEData)
            {
                if (pdata != null && pdata.Length > 0)
                {
                    OnDeceiveDataChanged(new DeceiveDataArgs() { ReportBuff = pdata });
                }
            }
            else if (type == MsgType.SendOK)
            {

            }
            else if (type == MsgType.AddBluetooth)
            {
                bt.Adresse = btInfo.Adresse;
                bt.MAC = btInfo.MAC;
                BleWarchingArgs bleInfo = new BleWarchingArgs(bt);
                OnBleWarchingChanged(bleInfo);
            }
            else if (type == MsgType.DeleteBluetooth)
            {
                bt.Adresse = BLEAdresse;
                bt.MAC = BLEMAC;
                SenserMsg sm = new SenserMsg() { index = index, bluetooth = bt };
                BleDeleteArgs bleInfoDelete = new BleDeleteArgs(sm) { };
                OnBleDeleteChanged(bleInfoDelete);
            }
        }
        public void SendCommand(byte[] sendBuffer)
        {
            bluetooth.Write(sendBuffer);
        }

        public int TotalFreq;

        //public DataDefine DataDefineFromBLEBuffer(byte[] buffer)
        //{
        //    int dataCount = 3;
        //    if (buffer.Length == 22|| buffer.Length == 4002)// voice sensor:22 is bettery count,4002 is data count
        //    {
        //        dataCount = 2;
        //    }
        //    DataDefine ddf = new DataDefine();
        //    ddf.index = index;
        //    ddf.connectName = BLEName;
        //    switch ((ReceiveFirstSign)buffer[0])
        //    {
        //        case ReceiveFirstSign.BatteryLevel:
        //            ddf.sensorID = buffer[1];
        //            ddf.receiveSign = ReceiveFirstSign.BatteryLevel;
        //            ddf.values = ValueBuffer(buffer.Skip(2).Take(buffer.Length - 2).ToArray(), 3, 1, 0);
        //            break;
        //        case ReceiveFirstSign.BatteryLevelCurrentTypeSensorsA1:
        //        case ReceiveFirstSign.BatteryLevelCurrentTypeSensorsB1:
        //        case ReceiveFirstSign.BatteryLevelCurrentTypeSensorsC1:
        //            TotalFreq = 10000;
        //            SkipCount = (int)(TotalFreq / Frequency);
        //            ddf.sensorID = buffer[1];
        //            ddf.receiveSign = ReceiveFirstSign.BatteryLevel;
        //            ddf.values = ValueBuffer(buffer.Skip(2).Take(buffer.Length - 2).ToArray(), dataCount, 1, 0);
        //            break;
        //        case ReceiveFirstSign.Common:
        //            ddf.sensorID = buffer[1];
        //            ddf.receiveSign = ReceiveFirstSign.Common;
        //            ddf.values = ValueBuffer(buffer.Skip(2).Take(buffer.Length - 2).ToArray(), 3, K, B);
        //            break;
        //        case ReceiveFirstSign.CurrentTypeSensorsA0:
        //        case ReceiveFirstSign.CurrentTypeSensorsB0:
        //        case ReceiveFirstSign.CurrentTypeSensorsC0:
        //            ddf.sensorID = buffer[1];
        //            ddf.receiveSign = ReceiveFirstSign.Common;
        //            if (buffer[1] == 8)// voice sensor not skip 
        //            {
        //                SkipCount = 1;
        //                ddf.values = ValueBuffer(buffer.Skip(2).Take(buffer.Length - 2).ToArray(), dataCount, K, B, true);
        //            }
        //            else
        //            {
        //                ddf.values = ValueBuffer(buffer.Skip(2).Take(buffer.Length - 2).ToArray(), dataCount, K, B);
        //            }
        //            break;
        //        default:
        //            break;
        //    }
        //    return ddf;
        //}
        public void DisposeBLE()
        {
            bluetooth.Dispose();
        }
        public List<float> ValueBuffer(byte[] dataBuffer, int EachValueLen, float k, float b, bool IsVoice = false)
        {
            List<float> values = new List<float>();
            for (int i = 0; i < dataBuffer.Length; i += EachValueLen)
            {
                var buffer = dataBuffer.Skip(i).Take(EachValueLen);
                float value = ByteToLong(buffer.ToArray(), k, b, IsVoice);
                values.Add(value);
            }
            return values;
        }
        public float ByteToLong(byte[] dataBuffer, float k, float b, bool IsVoice)
        {
            float value = 0;
            if (dataBuffer.Length == 3)
            {
                if (dataBuffer[0] < 128)
                {
                    value = (dataBuffer[0] * 65536 + dataBuffer[1] * 256 + dataBuffer[2]) * k + b;
                }
                else
                    value = -1 * (((dataBuffer[0] - 128) * 65536 + dataBuffer[1] * 256 + dataBuffer[2]) * k) + b;
            }
            else if (dataBuffer.Length == 2)
            {
                if (IsVoice)
                {
                    if (dataBuffer[0] < 128)
                    {
                        value = (dataBuffer[0] * 256 + dataBuffer[1]) * k + b;
                    }
                    else
                        value = (dataBuffer[0] * 256 + dataBuffer[1] - 65536) * k + b;
                }
                else
                {
                    if (dataBuffer[0] < 128)
                    {
                        value = (dataBuffer[0] * 256 + dataBuffer[1]) * k + b;
                    }
                    else
                        value = -1 * (((dataBuffer[0] - 128) * 256 + dataBuffer[1]) * k) + b;
                }
            }
            return value;
        }

        public event AddSensorDataHandler AddSensorDataChanged = null;
        public void OnAddSensorDataChanged(SenserMsg e)
        {
            if (AddSensorDataChanged != null)
            {
                AddSensorDataChanged(this, e);
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

        public event BleWarchingHandler BleWarchingChanged = null;
        public void OnBleWarchingChanged(BleWarchingArgs e)
        {
            if (BleWarchingChanged != null)
            {
                BleWarchingChanged(this, e);
            }
        }

        public event BleDeleteHandler BleDeleteChanged = null;
        public void OnBleDeleteChanged(BleDeleteArgs e)
        {
            if (BleDeleteChanged != null)
            {
                BleDeleteChanged(this, e);
            }
        }

        public void StartBluetoothSearch()
        {
            bluetooth.StartBleDeviceWatcher();

        }
    }
}
