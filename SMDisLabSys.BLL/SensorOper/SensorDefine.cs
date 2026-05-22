using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMDisLabSys.BLL.SensorKeyValue;

namespace SMDisLabSys.BLL.SensorOper
{
    public class SensorDefine
    {
        public SensorDefine()
        {
            LoadDefault();
        }
        /// <summary>
        /// 传感器定义
        /// </summary>
        /// <param name="manufacturerID">厂商识别码</param>
        /// <param name="name">内部名称</param>
        /// <param name="caption">外部名称</param>
        /// <param name="sensorID">识别码</param>
        /// <param name="typeID">类型码</param>
        /// <param name="decimalcount">有效小数位数</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="minValue">最小值</param>
        /// <param name="unit">单位名称</param>
        /// <param name="calibration">校正值</param>
        /// <param name="modeID">工作方式码</param>
        /// <param name="sensorIndex">内部编号</param>
        public SensorDefine(int manufacturerID, string name, string caption, int sensorID, int typeID, int decimalcount, double maxValue, double minValue, string unit, double calibration, int modeID, int sensorIndex)
        {
            Initialize(manufacturerID, name, caption, sensorID, typeID, decimalcount, maxValue, minValue, unit, calibration,modeID,sensorIndex);
        }
        /// <summary>
        /// 传感器定义
        /// </summary>
        /// <param name="value">序列化字符串</param>
        public SensorDefine(string value)
        {
            LoadDefault();
            Parse(value);
        }
        #region Props
        int _manufacturerID;
        string _name;
        string _caption;
        int _sensorID;
        int _typeID;
        int _decimal;
        double _maxValue;
        double _minValue;
        string _unit;
        double _calibration;
        int _modeID;
        int _sensorIndex;
        List<ShiftDefine> _shifttDefines = new List<ShiftDefine>();
        bool _available;
        double _k, _b;
        double _maxFrequency ;
        /// <summary>
        /// 最大工作频率
        /// </summary>
        public double MaxFrequency
        {
            get { return _maxFrequency; }
            set { _maxFrequency = value; }
        }
        /// <summary>
        /// 按照线性插值计算测量值，Y=KX+B中的K
        /// </summary>
        public double K
        {
            get { return _k; }
            set { _k = value; }
        }
        /// <summary>
        /// 按照线性插值计算测量值，Y=KX+B中的B
        /// </summary>
        public double B
        {
            get { return _b; }
            set { _b = value; }
        }
        /// <summary>
        /// 厂商识别码
        /// </summary>
        public int ManufacturerID
        {
            get { return _manufacturerID; }
            set { _manufacturerID = value; }
        }
        /// <summary>
        /// 内部名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        /// <summary>
        /// 外部名称
        /// </summary>
        public string Caption
        {
            get { return _caption; }
            set { _caption = value; }
        }
        /// <summary>
        /// 识别码
        /// </summary>
        public int SensorID
        {
            get { return _sensorID; }
            set { _sensorID = value; }
        }
        /// <summary>
        /// 类型码
        /// </summary>
        public int TypeID
        {
            get { return _typeID; }
            set { _typeID = value; }
        }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Decimal
        {
            get { return _decimal; }
            set { _decimal = value; }
        }
        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }
        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            get { return _minValue; }
            set { _minValue = value; }
        }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        /// <summary>
        /// 校正值
        /// </summary>
        public double Calibration
        {
            get { return _calibration; }
            set { _calibration = value; }
        }
        /// <summary>
        /// 工作模式码
        /// </summary>
        public int ModeID
        {
            get { return _modeID; }
            set { _modeID = value; }
        }
        /// <summary>
        /// 在同一个SensorID下面，可以有多个SensorIndex，也就是一个探头多个数据，从0（Default）开始
        /// </summary>
        public int SensorIndex
        {
            get { return _sensorIndex; }
            set { _sensorIndex = value; }
        }
        /// <summary>
        /// 单位定义集，默认为空
        /// </summary>
        public List<ShiftDefine> ShiftDefines
        {
            get { return _shifttDefines; }
        }
        /// <summary>
        /// 是否是有效传感器
        /// </summary>
        public bool Available
        {
            get { return _available; }
            set { _available = value; }
        }
        #endregion

        #region Methods
        void LoadDefault()
        {
            _manufacturerID = -1;
            _name = "";
            _caption = "";
            _sensorID = 0x00;
            _typeID = 0;
            _decimal = 4;
            _maxValue = 1;
            _minValue = 0;
            _unit = "";
            _calibration = 0;
            _modeID = 1;
            _sensorIndex = 0;
            _shifttDefines.Clear();
            _available = false;
            _k = 1;
            _b = 0;
            _maxFrequency = 20000;
        }
        void Initialize(int manufacturerID, string name, string caption, int sensorID, int typeID, int decimalcount, double maxValue, double minValue, string unit, double calibration, int modeID, int sensorIndex)
        {
            _manufacturerID = manufacturerID;
            _name = name;
            _caption = caption;
            _sensorID = sensorID;
            _typeID = typeID;
            _decimal = decimalcount;
            _maxValue = maxValue;
            _minValue = minValue;
            _unit = unit;
            _calibration = calibration;
            _modeID = modeID;
            _sensorIndex = sensorIndex;
            _shifttDefines.Clear();
            _available = true;
            _k = 1;
            _b = 0;
            _maxFrequency = 20000;
        }
        /// <summary>
        /// 获取该档位最大值
        /// </summary>
        /// <param name="shiftIndex">档位编码</param>
        /// <returns>最大值</returns>
        public double GetMaxValue(byte shiftIndex)
        {
            double value = _maxValue;
            for (int i = 0; i < _shifttDefines.Count; i++)
            {
                if (_shifttDefines[i].ShiftIndex == shiftIndex)
                {
                    value = _shifttDefines[i].MaxValue;
                    break;
                }
            }
            return value;
        }
        /// <summary>
        /// 获取该档位最小值
        /// </summary>
        /// <param name="shiftIndex">单位编码</param>
        /// <returns>最小值</returns>
        public double GetMinValue(byte shiftIndex)
        {
            double value = _minValue;
            for (int i = 0; i < _shifttDefines.Count; i++)
            {
                if (_shifttDefines[i].ShiftIndex == shiftIndex)
                {
                    value = _shifttDefines[i].MinValue;
                    break;
                }
            }
            return value;
        }
        /// <summary>
        /// 获取该档位校正值
        /// </summary>
        /// <param name="shiftIndex">档位编码</param>
        /// <returns>校正值</returns>
        public double GetCalibration(byte shiftIndex)
        {
            double value = _calibration;
            for (int i = 0; i < _shifttDefines.Count; i++)
            {
                if (_shifttDefines[i].ShiftIndex == shiftIndex)
                {
                    value = _shifttDefines[i].Calibration;
                    break;
                }
            }
            return value;
        }
        #endregion
        
        #region Serialize
        /// <summary>
        /// 档位序列化
        /// </summary>
        /// <param name="list">档位列表</param>
        /// <returns>序列化字符串</returns>
        public static string ShiftDefines2Str(List<ShiftDefine> list)
        {
            KeyValue keyValue = new KeyValue();
            for (int i = 0; i < list.Count; i++)
            {
                keyValue.Add(i.ToString(), list[i].ToString());
            }
            return keyValue.ToString();
        }
        /// <summary>
        /// 档位反序列化
        /// </summary>
        /// <param name="value">序列化字符串</param>
        /// <returns>档位列表</returns>
        public static List<ShiftDefine>  ShiftDefinesParse(string value)
        {
            KeyValue keyValue = new KeyValue();
            keyValue.Parse(value);
            List<ShiftDefine> list = new List<ShiftDefine>();
            for (int i = 0; i < keyValue.Count; i++)
            {
                list.Add( new ShiftDefine(keyValue.GetValueByKey(i.ToString())));
            }
            return list;
        }
        /// <summary>
        /// 传感器定义序列化
        /// </summary>
        /// <returns>序列化字符串</returns>
        public override string ToString()
        {
            KeyValue keyValue = new KeyValue();
            keyValue.Add("ManufacturerID", _manufacturerID.ToString());
            keyValue.Add("Name",_name);
            keyValue.Add("Caption",_caption);
            keyValue.Add("SensorID",_sensorID.ToString());
            keyValue.Add("TypeID",_typeID.ToString());
            keyValue.Add("Decimal",_decimal.ToString());
            keyValue.Add("MaxValue",_maxValue.ToString());
            keyValue.Add("MinValue",_minValue.ToString());
            keyValue.Add("Unit",_unit);
            keyValue.Add("Calibration",_calibration.ToString());
            keyValue.Add("ModeID",_modeID.ToString());
            keyValue.Add("SensorIndex",_sensorIndex.ToString());
            keyValue.Add("ShiftDefines", ShiftDefines2Str(_shifttDefines));
            keyValue.Add("Available",_available.ToString());
            keyValue.Add("KValue", _k.ToString());
            keyValue.Add("BValue", _b.ToString());
            keyValue.Add("MaxFrequency", _maxFrequency.ToString());
            return keyValue.ToString();
        }
        /// <summary>
        /// 传感器定义反序列化
        /// </summary>
        /// <param name="value">序列化字符串</param>
        public void Parse(string value)
        {
            KeyValue keyValue = new KeyValue();
            keyValue.Parse(value);
            int.TryParse(keyValue.GetValueByKey("ManufacturerID"), out _manufacturerID);
            _name = keyValue.GetValueByKey("Name");
            _caption = keyValue.GetValueByKey("Caption");
            int.TryParse(keyValue.GetValueByKey("SensorID"), out _sensorID);
            int.TryParse(keyValue.GetValueByKey("TypeID"), out _typeID);
            int.TryParse(keyValue.GetValueByKey("Decimal"), out _decimal);
            double.TryParse(keyValue.GetValueByKey("MaxValue"), out _maxValue);
            double.TryParse(keyValue.GetValueByKey("MinValue"), out _minValue);
            _unit = keyValue.GetValueByKey("Unit");
            double.TryParse(keyValue.GetValueByKey("Calibration"), out _calibration);
            int.TryParse(keyValue.GetValueByKey("ModeID"), out _modeID);
            int.TryParse(keyValue.GetValueByKey("SensorIndex"), out _sensorIndex);
            _shifttDefines = ShiftDefinesParse(keyValue.GetValueByKey("ShiftDefines"));
            bool.TryParse(keyValue.GetValueByKey("Available"), out _available);
            double.TryParse(keyValue.GetValueByKey("KValue"), out _k);
            double.TryParse(keyValue.GetValueByKey("BValue"), out _b);
            double.TryParse(keyValue.GetValueByKey("MaxFrequency"), out _maxFrequency);
            if (_maxFrequency <= 0)
            {
                _maxFrequency = 20000;
            }
        }
        #endregion
    }
}
