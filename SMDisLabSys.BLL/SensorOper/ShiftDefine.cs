using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMDisLabSys.BLL.SensorOper
{
    public class ShiftDefine
    {
        public ShiftDefine()
        {
            LoadDefault();
        }
        /// <summary>
        /// 档位定义
        /// </summary>
        /// <param name="shiftIndex">档位编码</param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <param name="calibration">校正值</param>
        public ShiftDefine(byte shiftIndex, double minValue, double maxValue, double calibration)
        {
            _shiftIndex = shiftIndex;
            _minValue = minValue;
            _maxValue = maxValue;
            _calibration = calibration;
            _k = 1;
            _b = 0;
        }
        /// <summary>
        /// 档位定义
        /// </summary>
        /// <param name="value">序列化字符串</param>
        public ShiftDefine(string value)
        {
            LoadDefault();
            Parse(value);
        }
        #region Props
        byte _shiftIndex;
        double _minValue;
        double _maxValue;
        double _calibration;
        double _k, _b;
        /// <summary>
        /// 档位编码
        /// </summary>
        public byte ShiftIndex
        {
            get { return _shiftIndex; }
            set { _shiftIndex = value; }
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
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            get { return _maxValue; }
            set { _maxValue = value; }
        }
        /// <summary>
        /// 校正值
        /// </summary>
        public double Calibration
        {
            get { return _calibration; }
            set { _calibration = value; }
        }
        #endregion

        #region Serialize
        void LoadDefault()
        {
            _shiftIndex = 0x00;
            _minValue = 0;
            _maxValue = 0;
            _calibration = 0;
            _k = 1;
            _b = 0;
        }
        /// <summary>
        /// 档位序列化
        /// </summary>
        /// <returns>序列化字符串</returns>
        public override string ToString()
        {
            string[] strList = new string[6];
            int i=  0;
            strList[i++] = _shiftIndex.ToString();
            strList[i++] = _minValue.ToString();
            strList[i++] = _maxValue.ToString();
            strList[i++] = _calibration.ToString();
            strList[i++] = _k.ToString();
            strList[i++] = _b.ToString();
            return string.Join("|", strList);
        }
        /// <summary>
        /// 档位反序列化
        /// </summary>
        /// <param name="value">序列化字符串</param>
        public void Parse(string value)
        {
            string[] strList = value.Split('|');
            if (strList.Length >= 4)
            {
                int i = 0;
                byte.TryParse(strList[i++], out _shiftIndex);
                double.TryParse(strList[i++], out _minValue);
                double.TryParse(strList[i++], out _maxValue);
                double.TryParse(strList[i++], out _calibration);
                if (strList.Length >= 6)
                {
                    double.TryParse(strList[i++], out _k);
                    double.TryParse(strList[i++], out _b);
                }
            }
        }
        #endregion
    }
}
