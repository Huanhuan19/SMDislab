using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMDisLabSys.BLL
{
    public class DataEventArgs : EventArgs
    {
        byte[] _datas;
        int _count;
        byte _portIndex;
        int _index;
        float _value;
        CommandDefine _commandDefine;
        public DataEventArgs(byte[] datas, int count, byte portIndex,int index,CommandDefine commandDefine,float value)
        {
            _datas = new byte[datas.Length];
            datas.CopyTo(_datas, 0);
            _count = count;
            _portIndex = portIndex;
            _index = index;
            _commandDefine = commandDefine;
            _value = value;
        }
        public byte[] Datas
        {
            get { return _datas; }
        }
        public int Count
        {
            get { return _count; }
        }
        public byte PortIndex
        {
            get { return _portIndex; }

        }
        public int Index
        {
            get { return _index; }
        }
        public CommandDefine CommandDefine
        {
            get { return _commandDefine; }
        }
        public float Value
        {
            get { return _value; }
        }
    }
    public delegate void DataHandler(object sender, DataEventArgs e);

    public class SourceDataEventArgs : EventArgs
    {
        byte[] _datas;
        int _count;
        public SourceDataEventArgs(byte[] datas, int count)
        {
            _datas = new byte[datas.Length];
            datas.CopyTo(_datas, 0);
            _count = count;
        }
        public byte[] Buffer
        {
            get { return _datas; }
        }
        public int Count
        {
            get { return _count; }
        }
    }
    public delegate void SourceDataHandler(object sender, SourceDataEventArgs e);

    public class ValueEventArgs : EventArgs
    {
        float _value = 0f;
        int _index;

        public ValueEventArgs(float value, int index)
        {
            _index = index;
            _value = value;
        }
        public int Index
        {
            get { return _index; }

        }
        public float Value
        {
            get { return _value; }
        }
    }
    public delegate void ValueHandler(object sender, ValueEventArgs e);

    public class ConnectEventArgs : EventArgs
    {
        bool _connected;
        public ConnectEventArgs(bool connected)
        {
            _connected = connected;
        }
        public bool Connected
        {
            get { return _connected; }
        }
    }
    public delegate void ConnectedHandler(object sender,ConnectEventArgs e );

    public class StartStopEventArgs : EventArgs
    {
        bool _isStart;
        public StartStopEventArgs(bool isStart)
        {
            _isStart = isStart;
        }
        public bool IsStart
        {
            get { return _isStart; }
        }
    }
    public delegate void StartStopHandler(object sender,StartStopEventArgs e );

    public class ShiftEventArgs : EventArgs
    {
        int _index;
        byte _shift;
        public ShiftEventArgs(int index, byte shift)
        {
            _index = index;
            _shift = shift;
        }
        public int Index
        {
            get { return _index; }
        }
        public byte Shift
        {
            get { return _shift; }
        }
    }
    public delegate void ShiftHandler(object sender, ShiftEventArgs e);

    public class BleWarchingArgs : EventArgs
    {
        public BluetoothInfo btInfo;
        public BleWarchingArgs(BluetoothInfo info)
        {
            btInfo = info;
        }
    }
    public delegate void BleWarchingHandler(object sender, BleWarchingArgs e);

    public class BleDeleteArgs : EventArgs
    {
        public SenserMsg SensorInfo;
        public BleDeleteArgs(SenserMsg info)
        {
            SensorInfo = info;
        }
    }
    public delegate void BleDeleteHandler(object sender, BleDeleteArgs e);

    public class DeceiveDataArgs : EventArgs
    {
        public  byte[] ReportBuff;
    }
    public delegate void DeceiveDataHandler(object sender, DeceiveDataArgs e);

    public delegate void AddSensorDataHandler(object sender, SenserMsg e);
}
