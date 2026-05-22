using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HPSF;
using SMDisLabSys.BLL.SensorKeyValue;

namespace SMDisLabSys.BLL.SensorOper
{
    public class SensorCollection
    {
        public static SensorCollection Instance = new SensorCollection();

        #region Props
        List<SensorDefine> _sensorDefines = new List<SensorDefine>();
        /// <summary>
        /// 所有的定义
        /// </summary>
        public List<SensorDefine> SensorDefines
        {
            get { return _sensorDefines; }
        }
        #endregion

        public void LoadSensor(string filename)
        {
            try
            {
                System.IO.StreamReader reader = System.IO.File.OpenText(filename);
                Parse(reader.ReadToEnd());
                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="value">序列化字符串</param>
        void Parse(string value)
        {
            KeyValue keyValue = new KeyValue();
            keyValue.Parse(value);
            _sensorDefines.Clear();
            for (int i = 0; i < keyValue.Count; i++)
            {
                _sensorDefines.Add(new SensorDefine(keyValue.GetValueByKey(i.ToString())));
            }
        }

        /// <summary>
        /// 根据识别码和内部序号获取传感器定义
        /// </summary>
        /// <param name="sensorID">识别码</param>
        /// <param name="sensorIndex">内部编号</param>
        /// <returns>传感器定义，如果找不到返回默认值（非null）</returns>
        public SensorDefine GetSensorDefineBySensorID_SensorIndex(byte sensorID, int sensorIndex)
        {
            SensorDefine sensorDefine = new SensorDefine();
            int index = GetIndexBySensorID_SensorIndex(sensorID, sensorIndex);
            if (index >= 0 && index < _sensorDefines.Count)
            {
                sensorDefine = _sensorDefines[index];
            }
            return sensorDefine;
        }
        /// <summary>
        /// 根据识别码和内部序号获取传感器索引
        /// </summary>
        /// <param name="sensorID">识别码</param>
        /// <param name="sensorIndex">内部编号</param>
        /// <returns></returns>
        public int GetIndexBySensorID_SensorIndex(byte sensorID, int sensorIndex)
        {
            int index = -1;
            for (int i = 0; i < _sensorDefines.Count; i++)
            {
                if (_sensorDefines[i].SensorID == sensorID && _sensorDefines[i].SensorIndex == sensorIndex)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
